using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.AI;

namespace MLand
{
    class BuildingManager
    {
        GameObject mParent;
        List<Building> mBuildingList;
        double[] mDropSlimeCores;
        NavMeshSurface2d mNavMeshSurface;
        GameObject[] mFieldHideClouds;
        public void Init()
        {
            GameObject mNavMeshObj = GameObject.Find("NavMesh_Background");
            mNavMeshSurface = mNavMeshObj.GetComponent<NavMeshSurface2d>();
            mParent = mNavMeshObj.FindGameObject("Buildings");
            mBuildingList = new List<Building>();
            mFieldHideClouds = new GameObject[(int)ElementalType.Count];
            mDropSlimeCores = new double[(int)ElementalType.Count];

            ApplySavePoint();
        }

        void ApplySavePoint()
        {
            foreach (string id in MLand.SavePoint.BuildingManager.BuildingLevels.Keys)
            {
                UnlockBuilding(id);
            }

            for(int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                mFieldHideClouds[i] = mParent.FindGameObject($"{type}_FieldHide_Cloud");
                mFieldHideClouds[i].SetActive(!MLand.SavePoint.BuildingManager.IsExpandedField(type));
            }
        }

        public void OnUpdate(float dt)
        {
            for(int i = 0; i < mDropSlimeCores.Length; ++i)
            {
                mDropSlimeCores[i] = 0;
            }

            foreach(var building in mBuildingList)
            {
                building.OnUpdate(dt);

                if (building.CanSlimeCoreDrop)
                {
                    var result = building.DropSlimeCore();

                    mDropSlimeCores[(int)result.Item1] += result.Item2;
                }
            }

            if (mDropSlimeCores.Any(x => x > 0))
            {
                MLand.GameManager.AddSlimeCores(mDropSlimeCores);
            }
        }

        public bool IsMaxLevelBuilding(string id)
        {
            int level = MLand.SavePoint.GetBuildingLevel(id);
            int maxLevel = MLand.GameData.BuildingUpgradeData.Where(x => x.id == id).Max(x => x.level);

            return level >= maxLevel;
        }

        public bool IsReadyForUpgradeBuilding(string id)
        {
            // 지어져있는지 확인
            if (IsUnlockedBuilding(id) == false)
                return false;

            // 지을 수 있는 조건을 만족했는지 확인
            if (IsSatisfiedUpgradeCondition(id) == false)
                return false;

            return true;
        }

        public bool IsReadyForUnlockBuilding(string id)
        {
            // 이미 지어져있는지 확인
            if (IsUnlockedBuilding(id))
                return false;

            // 지을 수 있는 조건을 만족했는지 확인
            if (IsSatisfiedUnlockCondition(id) == false)
                return false;

            return true;
        }

        public bool IsUnlockedBuilding(string id)
        {
            return mBuildingList.Select(x => x.Id).Contains(id);
        }

        public bool IsSatisfiedUnlockCondition(string id)
        {
            BuildingUnlockData data = MLand.GameData.BuildingUnlockData.TryGet(id);

            Debug.Assert(data != null, $"{id}의 BuildingUnlockData가 존재하지 않음");

            return IsSatisfiedCondition(data.precendingBuilding, data.precendingBuildingLevel);
        }

        public void RefreshExpandField()
        {
            var navMeshObj = mNavMeshSurface.gameObject;

            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                var expandFieldObj = navMeshObj.FindGameObject($"Expand_{type}");
                var collider = expandFieldObj.FindComponent<EdgeCollider2D>("Tilemap_Field");

                collider.enabled = MLand.SavePoint.BuildingManager.IsExpandedField(type);
            }
        }

        public void BuildNavMesh()
        {
            mNavMeshSurface.BuildNavMesh();
        }

        public bool IsSatisfiedUpgradeCondition(string id)
        {
            int buildngLevel = MLand.SavePoint.GetBuildingLevel(id);
            int nextLevel = buildngLevel + 1;

            BuildingUpgradeData data = DataUtil.GetBuildingUpgradeData(id, nextLevel);

            if (data == null)
                return false;
            else
                return IsSatisfiedCondition(data.precendingBuilding, data.precendingBuildingLevel);
        }

        public bool IsSatisfiedCondition(string condition, int requireLevel)
        {
            if (condition.IsValid())
            {
                Building building = mBuildingList.Find(x => x.Id == condition);
                if (building == null)
                {
                    return false;
                }
                else
                {
                    return building.Level >= requireLevel;
                }
            }
            else
            {
                return true;
            }
        }

        public Building OnUnlockBuilding(string id)
        {
            Building building = UnlockBuilding(id);

            if (building != null)
            {
                building.PlayTweenScaleMotion();

                MLand.SavePoint.CheckQuests(QuestType.BuildBuilding);

                mNavMeshSurface.BuildNavMesh();
            }

            return building;
        }

        Building UnlockBuilding(string id)
        {
            BuildingData data = MLand.GameData.BuildingData.TryGet(id);
            BuildingUnlockData unlockData = MLand.GameData.BuildingUnlockData.TryGet(id);

            Debug.Assert(data != null, $"{id}의 BuildingData가 존재하지 않음");
            Debug.Assert(unlockData != null, $"{id}의 BuildingUnlockData가 존재하지 않음");

            GameObject buildingObj = mParent.FindGameObject($"{unlockData.objName}");

            if (data.isCentralBuilding)
            {
                CentralBuilding building = buildingObj.GetOrAddComponent<CentralBuilding>();
                building.Init(data);

                mBuildingList.Add(building);

                return building;
            }
            else
            {
                Building building = buildingObj.GetOrAddComponent<Building>();
                building.Init(data);

                mBuildingList.Add(building);

                return building;
            }
        }

        public Building OnUpgradeBuilding(string id)
        {
            Building building = UpgradeBuilding(id);

            RefreshExpandField();

            mNavMeshSurface.BuildNavMesh();

            return building;
        }

        Building UpgradeBuilding(string id)
        {
            Building building = GetBuilding(id);
            if (building != null)
            {
                building.OnLevelUp();
            }
            else
            {
                Debug.LogError($"Id : {id}, Building이 존재하지 않음");
            }

            return building;
        }

        public Building GetBuilding(string building)
        {
            return mBuildingList.Find(x => x.Id == building);
        }

        public double GetSlimeCoreDropAmountForMinute(int minute)
        {
            double totalAmount = 0;

            foreach (Building building in mBuildingList)
            {
                double dropAmount = building.StatData.slimeCoreDropAmount;
                float dropCoolTime = building.StatData.slimeCoreDropCoolTime;

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

            foreach (Building building in mBuildingList.Where(x => x.Type == type))
            {
                double dropAmount = building.StatData.slimeCoreDropAmount;
                float dropCoolTime = building.StatData.slimeCoreDropCoolTime;

                // 1분에 약 몇번 드롭하는지 계산한다. 소숫점은 중요하지 않다.
                int dropCountInOneMinute = Mathf.RoundToInt(TimeUtil.SecondsInMinute / dropCoolTime);

                int dropCount = minute * dropCountInOneMinute;

                totalAmount += (dropAmount * dropCount);
            }

            return totalAmount;
        }

        // 순서대로 물,풀,불,땅
        Vector2[] cloudMovePos = new Vector2[] {
            new Vector2(-26f, 34.5f), new Vector2(24.5f, 34.5f),
            new Vector2(6.75f,2f), new Vector2(-6.65f, 0f) };
        public IEnumerator PlayFieldHideCloudFadeMotion(ElementalType type)
        {
            MLand.CameraManager.PlayHideCloudZoomRoutine();

            yield return new WaitForSeconds(0.5f);

            var cloud = mFieldHideClouds[(int)type];

            var endValue = cloudMovePos[(int)type];

            // 구름 움직이고
            cloud.transform.DOLocalMove(endValue, 5f)
                .SetAutoKill(false)
                .OnComplete(() => cloud.SetActive(false));

            foreach (var spriteRenderer in cloud.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.DOFade(0f, 5f)
                    .SetAutoKill(false);
            }

            SoundPlayer.PlayHideCloud();

            yield return new WaitForSeconds(2f);

            MLand.CameraManager.PlayResetZoomRoutine();
        }
    }
}