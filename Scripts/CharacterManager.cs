using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    class CharacterManager
    {
        Transform mContainer;
        Character mGoldSlime;
        List<Character> mSlimeList;
        double[] mDropSlimeCores;
        public Character GoldSlime => mGoldSlime;
        public bool IsSpawnedGoldSlime => mGoldSlime != null;
        public void Init(Transform container)
        {
            mContainer = container;
            mSlimeList = new List<Character>();
            mDropSlimeCores = new double[(int)ElementalType.Count];

            ApplySavePoint();
        }

        void ApplySavePoint()
        {
            foreach(ObscuredString savedId in MLand.SavePoint.SlimeManager.GetAllSlimeIds())
            {
                Character slime = SpawnSlime(savedId);
                if (slime != null)
                {
                    slime.StartPathFinder(MonsterLandUtil.GetCanMovePos());
                }
            }
        }

        public void OnUpdate(float dt)
        {
            for(int i = 0; i < mDropSlimeCores.Length; ++i)
            {
                mDropSlimeCores[i] = 0;
            }

            foreach (Character character in mSlimeList)
            {
                character.OnUpdate(dt);

                if (character.CanSlimeCoreDrop)
                {
                    var result = character.DropSlimeCore();

                    mDropSlimeCores[(int)result.Item1] += result.Item2;
                }
            }

            if (mDropSlimeCores.Any(x => x > 0))
            {
                MLand.GameManager.AddSlimeCores(mDropSlimeCores);
            }

            mGoldSlime?.OnUpdate(dt);
        }

        void AddCharacter(Character addCharacter)
        {
            mSlimeList.Add(addCharacter);
        }

        // =====================================
        // 슬라임 관련
        // =====================================
        public bool IsReadyForSpawnSlime(string id)
        {
            // 이미 소환되어 있는지 확인
            if (IsSpawnedSlime(id))
                return false;

            // 소환할 수 있는 조건을 만족했는지 확인
            if (IsSatisfiedCondition(id) == false)
                return false;

            return true;
        }

        public bool IsSpawnedSlime(string id)
        {
            return mSlimeList.Select(x => x.Id).Contains(id);
        }

        public bool IsSatisfiedCondition(string id)
        {
            // 특정 건물이 이미 건축되어 있는지 확인한다.
            CharacterData data = MLand.GameData.SlimeData.TryGet(id);
            if (data.precendingBuilding.IsValid())
            {
                bool satisfied = MLand.GameManager.IsUnlockedBuilding(data.precendingBuilding);
                if (satisfied == false)
                    return false;

                int buildingLevel = MLand.GameManager.GetBuildingLevel(data.precendingBuilding);
                int requireLevel = data.precendingBuildingLevel;

                satisfied = buildingLevel >= requireLevel;
                if (satisfied == false)
                    return false;
            }

            if (data.precendingSlime.IsValid())
            {
                // 선행 슬라임이 소환되어 있는지 확인
                bool satisfied = IsSpawnedSlime(data.precendingSlime);
                if (satisfied == false)
                    return false;
            }

            // 조건이 없다면 무조건 만족
            return true;
        }

        public Character SpawnSlime(string slimeId, string objName = "Slime")
        {
            GameObject slimeObj = Util.Instantiate($"Slime/{objName}", mContainer);

            Slime slime;

            if (slimeId != MLand.GameData.GoldSlimeCommonData.id)
            {
                slime = slimeObj.GetOrAddComponent<Slime>();

                AddCharacter(slime);
            }
            else
            {
                slime = slimeObj.GetOrAddComponent<GoldSlime>();

                mGoldSlime = slime;
            }

            slime.Init(slimeId);
            slime.PlaySpawnMotion();

            return slime;
        }
        
        public bool RemoveGoldSlime()
        {
            if (mGoldSlime != null)
            {
                // 방구 생성
                MLand.ParticleManager.Aquire("SlimeSpawn", pos: mGoldSlime.transform.position);

                SoundPlayer.PlayDisappearGoldSlime();

                mGoldSlime.OnRelease();
                mGoldSlime = null;

                return true;
            }

            return false;
        }

        public void UpgradeFriendShip(string slimeId, string friendShipItemId)
        {

        }

        public Slime GetSlime(string slimeId)
        {
            if (slimeId == MLand.GameData.GoldSlimeCommonData.id)
            {
                return mGoldSlime as Slime;
            }
            else
            {
                Character slime = mSlimeList.Find(x => x.Id == slimeId);

                return slime as Slime;
            }
        }

        public void ForcedCommands(ActionType actionType)
        {
            foreach(Character target in mSlimeList)
            {
                target.Action.PlayAction(actionType);
            }
        }

        Character FindClosestCharacter(Character finder, IEnumerable<Character> targets)
        {
            float minDistance = float.MaxValue;
            Character closestCharacter = null;

            foreach(Character target in targets)
            {
                float distance = Util.GetDistanceBy2D(finder.transform.position, target.transform.position);
                if ( minDistance > distance )
                {
                    minDistance = distance;
                    closestCharacter = target;
                }
            }

            return closestCharacter;
        }

        public Slime FindClosestSlime(Character warrior)
        {
            Character closestSlime = FindClosestCharacter(warrior, mSlimeList);

            return closestSlime as Slime;
        }

        public Slime GetNextSlime(Slime curSlime)
        {
            return GetNextSlime(curSlime, mSlimeList);
        }

        Slime GetNextSlime(Slime curSlime, List<Character> slimeList)
        {
            Character nextSlime = null;

            if (slimeList != null)
            {
                int curSlimeIndex = slimeList.IndexOf(curSlime);
                int nextSlimeIndex = (curSlimeIndex + 1) % slimeList.Count;

                nextSlime = slimeList[nextSlimeIndex];
            }

            return nextSlime as Slime;
        }

        public Slime GetPrevSlime(Slime curSlime)
        {
            return GetPrevSlime(curSlime, mSlimeList);
        }

        Slime GetPrevSlime(Slime curSlime, List<Character> slimeList)
        {
            Character nextSlime = null;

            if (slimeList != null)
            {
                int curSlimeIndex = slimeList.IndexOf(curSlime);

                int prevSlimeIndex = curSlimeIndex - 1;
                prevSlimeIndex = prevSlimeIndex < 0 ? slimeList.Count - 1 : prevSlimeIndex;

                nextSlime = slimeList[prevSlimeIndex];
            }

            return nextSlime as Slime;
        }

        public Character GetMaxLevelSlime()
        {
            int maxLevel = 0;
            Slime maxLevelSlime = null;

            foreach(var slime in mSlimeList)
            {
                if ( maxLevel < slime.Data.level )
                {
                    maxLevel = slime.Data.level;
                    maxLevelSlime = slime as Slime;
                }
            }

            return maxLevelSlime;
        }

        // 버프는 반영하지 않는다.
        public double GetSlimeCoreDropAmountForMinute(int minute)
        {
            double totalAmount = 0;

            foreach(Character slime in mSlimeList)
            {
                double dropAmount = slime.Data.slimeCoreDropAmount;
                float dropCoolTime = slime.Data.slimeCoreDropCoolTime;

                // 1분에 약 몇번 드롭하는지 계산한다. 소숫점은 중요하지 않다.
                int dropCountInOneMinute = Mathf.RoundToInt(TimeUtil.SecondsInMinute / dropCoolTime);

                int dropCount = minute * dropCountInOneMinute;

                totalAmount += (dropAmount * dropCount);
            }

            return totalAmount;
        }

        public double GetSlimeCoreDropAmountForMinute(ElementalType type, int minute)
        {
            double totalAmount = 0;

            foreach (Character slime in mSlimeList.Where(x => x.Data.elementalType == type))
            {
                double dropAmount = slime.Data.slimeCoreDropAmount;
                float dropCoolTime = slime.Data.slimeCoreDropCoolTime;

                // 1분에 약 몇번 드롭하는지 계산한다. 소숫점은 중요하지 않다.
                int dropCountInOneMinute = Mathf.RoundToInt(TimeUtil.SecondsInMinute / dropCoolTime);

                int dropCount = minute * dropCountInOneMinute;

                totalAmount += (dropAmount * dropCount);
            }

            return totalAmount;
        }

        //// =====================================
        //// 용사 관련
        //// =====================================
        //public void AddWarrior(string warriorId, Vector3 pos)
        //{
        //    GameObject warriorObj = MLand.ObjectPool.AcquireObject($"Warrior/{warriorId}", mContainer);

        //    Debug.Assert(warriorObj != null, $"{warriorId} Obj가 없다.");

        //    Warrior warrior = warriorObj.GetOrAddComponent<Warrior>();
        //    warrior.Init(warriorId);
        //    warrior.SetPosition(pos);
        //    warrior.PlaySpawnMotion();

        //    AddCharacter(warrior);
        //}

        //public void RemoveAllWarrior()
        //{
        //    var removeList = new List<Character>();

        //    foreach(Character warrior in mWarriorList)
        //    {
        //        if (warrior.IsDead == false)
        //        {
        //            removeList.Add(warrior);
        //        }
        //    }

        //    foreach(Character remove in removeList)
        //    {
        //        RemoveCharacter(remove);
        //    }
        //}

        //public void RemoveWarrior(string warriorId)
        //{
        //    var warrior = mWarriorList.Find(x => x.Id == warriorId);

        //    Debug.Assert(warrior != null, $"{warrior} 존재 하지 않는 용사를 지우려고 했습니다.");

        //    if (warrior != null)
        //    {
        //        RemoveCharacter(warrior);
        //    }
        //}

        //public Warrior FindClosestWarrior(Character Slime)
        //{
        //    Character cloestWarrior = FindClosestCharacter(Slime, mWarriorList);

        //    return cloestWarrior as Warrior;
        //}

        //public bool IsAllWarriorDead()
        //{
        //    bool allDead = true;

        //    foreach (var warrior in mWarriorList)
        //    {
        //        if (warrior.IsDead == false)
        //        {
        //            allDead = false;
        //            break;
        //        }
        //    }

        //    return allDead;
        //}
    }
}