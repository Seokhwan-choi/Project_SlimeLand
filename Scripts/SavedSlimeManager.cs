using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace MLand
{
    [Serializable]
    public class SavedSlimeInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        string mId;
        [SerializeField]
        double mStackedExp;
        [SerializeField]
        List<int> mReceivedReward;
        [SerializeField]
        string[] mCostumes;
        
        public ObscuredString Id;
        public ObscuredDouble StackedExp;
        public List<ObscuredInt> ReceivedReward;
        public ObscuredString[] Costumes;
        public int Level => GetSlimeLevel();
        public bool IsMaxLevel => Level == MLand.GameData.SlimeFriendShipLevelUpData.Max(x => x.level);
        public CharacterData Data => MLand.GameData.SlimeData.TryGet(Id);
        public void Init(string id)
        {
            Id = id;
            StackedExp = 0;
            ReceivedReward = new List<ObscuredInt>();
            Costumes = new ObscuredString[(int)CostumeType.Count];
        }

        public void Normalize()
        {
            if (Costumes == null)
            {
                Costumes = new ObscuredString[(int)CostumeType.Count];
            }
            else
            {
                if (Costumes.Length != (int)CostumeType.Count)
                {
                    Costumes = new ObscuredString[(int)CostumeType.Count];
                }
            }
        }

        public void StackExp(double exp)
        {
            StackedExp += exp;
        }

        public void EquipCostume(CostumeType type, string id)
        {
            if (Costumes != null)
            {
                Costumes[(int)type] = id;
            }
        }

        public void UnEquipCostume(CostumeType type)
        {
            if (Costumes != null)
            {
                Costumes[(int)type] = string.Empty;
            }
        }

        public bool IsReceivedLevelReward(int level)
        {
            return ReceivedReward.Contains(level);
        }

        public SlimeLevelUpRewardData ReceiveLevelReward(int level)
        {
            SlimeLevelUpRewardData rewardData = DataUtil.GetLevelUpRewardData(Data.level, level);
            if (rewardData == null)
                return null;

            if (CanReceiveLevelReward(level) == false)
                return null;

            if (ReceivedReward.Contains(level))
                return null;

            ReceivedReward.Add(level);

            return rewardData;
        }

        public bool AnyCanReceiveLevelReward()
        {
            foreach(var data in GetAllLevelUpRewardData())
            {
                if (IsReceivedLevelReward(data.level) == false)
                    return true;
            }

            return false;
        }

        public bool CanReceiveLevelReward(int level)
        {
            var datas = GetAllLevelUpRewardData();
            if (datas == null || datas.Count() <= 0)
                return false;

            var data = datas.Where(x => x.level == level).FirstOrDefault();
            if (data == null)
                return false;

            return IsReceivedLevelReward(level) == false;
        }

        int GetSlimeLevel()
        {
            int level = 1;

            foreach (SlimeFriendShipLevelUpData data in MLand.GameData.SlimeFriendShipLevelUpData)
            {
                if (data.requireStackedExp <= StackedExp)
                {
                    if (level <= data.level)
                    {
                        level = data.level;
                    }
                }
            }

            return level;
        }

        public float GetLevelRewardByDecreaseCoolTime()
        {
            return GetAllLevelUpRewardData()
                .Where(x => x.decreaseSlimeCoreDropCoolTime > 0)
                .Where(x => IsReceivedLevelReward(x.level))
                .Sum(x => x.decreaseSlimeCoreDropCoolTime);
        }

        public double GetLevelRewardByIncreaseAmount()
        {
            return GetAllLevelUpRewardData()
                .Where(x => x.increaseSlimeCoreDropAmount > 0)
                .Where(x => IsReceivedLevelReward(x.level))
                .Sum(x => x.increaseSlimeCoreDropAmount);
        }

        IEnumerable<SlimeLevelUpRewardData> GetAllLevelUpRewardData()
        {
            return DataUtil.GetAllLevelUpReward(Data.level, Level);
        }

        public void RandomizeCryptoKey()
        {
            Id?.RandomizeCryptoKey();
            StackedExp.RandomizeCryptoKey();

            if(ReceivedReward != null && ReceivedReward.Count > 0)
            {
                for (int i = 0; i < ReceivedReward.Count; ++i)
                {
                    var reward = ReceivedReward[i];

                    reward.RandomizeCryptoKey();

                    ReceivedReward[i] = reward;
                }
            }

            if (Costumes != null)
            {
                foreach(var costume in Costumes)
                {
                    costume.RandomizeCryptoKey();
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (Id != null)
                mId = Id;

            mStackedExp = StackedExp;

            if (ReceivedReward != null)
            {
                mReceivedReward = ReceivedReward.Select(x => (int)x).ToList();
            }

            if (Costumes != null)
            {
                mCostumes = Costumes.Select(x => (string)x).ToArray();
            }
        }

        public void OnAfterDeserialize()
        {
            Id = mId;
            StackedExp = mStackedExp;
            if (mReceivedReward != null)
            {
                ReceivedReward = mReceivedReward.Select(x => (ObscuredInt)x).ToList();
            }

            if (mCostumes != null)
            {
                Costumes = mCostumes.Select(x => (ObscuredString)x).ToArray();
            }
        }
    }

    [Serializable]
    public class SavedSlimeManager
    {
        public DailyCounter GoldSlimeSpawnCounter;
        public List<SavedSlimeInfo> SlimeInfoList; 
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (SlimeInfoList == null)
            {
                SlimeInfoList = new List<SavedSlimeInfo>();
            }
            else
            {
                foreach(var slimeInfo in SlimeInfoList)
                {
                    slimeInfo.Normalize();
                }
            }

            if (GoldSlimeSpawnCounter == null)
                GoldSlimeSpawnCounter = new DailyCounter();
        }

        public void RandomizeKey()
        {
            GoldSlimeSpawnCounter?.RandomizeKey();

            if (SlimeInfoList != null)
            {
                foreach(var slimeInfo in SlimeInfoList)
                {
                    slimeInfo.RandomizeCryptoKey();
                }
            }
        }

        public bool IsUnlockedSlime(string slimeId)
        {
            return GetSlimeInfo(slimeId) != null;
        }

        public void AddSlime(string slimeId)
        {
            if (IsUnlockedSlime(slimeId))
            {
                Debug.LogError("이미 추가된 슬라임을 또 추가하려고 했다.");

                SoundPlayer.PlayErrorSound();

                return;
            }

            var slimeInfo = new SavedSlimeInfo();
            slimeInfo.Init(slimeId);

            SlimeInfoList.Add(slimeInfo);
        }

        public void RemoveSlime(string slimeId)
        {
            if (IsUnlockedSlime(slimeId) == false)
            {
                Debug.LogError("추가된 적 없는 슬라임을 지우려고 함");

                SoundPlayer.PlayErrorSound();

                return;
            }

            SlimeInfoList.Remove(GetSlimeInfo(slimeId));
        }

        public int GetSlimeLevel(string slimeId)
        {
            SavedSlimeInfo slimeInfo = GetSlimeInfo(slimeId);

            return slimeInfo?.Level ?? 1;
        }

        public void StackFriendShipExp(string id, double exp)
        {
            SavedSlimeInfo slimeInfo = GetSlimeInfo(id);

            if (slimeInfo != null)
            {
                slimeInfo.StackExp(exp);

                if (slimeInfo.IsMaxLevel)
                {
                    // 최고의 친구 ( 베스트 프랜드 ) 업적 확인
                    MLand.SavePoint.CheckAchievements(AchievementsType.BestFriend);
                }
            }
        }

        public double GetStackedFriendShipExp(string id)
        {
            return GetSlimeInfo(id)?.StackedExp ?? 0;
        }

        public IEnumerable<ObscuredString> GetAllSlimeIds()
        {
            return SlimeInfoList.Select(x => x.Id);
        }

        public float GetLevelUpRewardCoolTime(string slimeId)
        {
            return GetSlimeInfo(slimeId)?.GetLevelRewardByDecreaseCoolTime() ?? 0f;
        }

        public double GetLevelUpRewardAmount(string slimeId)
        {
            return GetSlimeInfo(slimeId)?.GetLevelRewardByIncreaseAmount() ?? 0f;
        }

        public SavedSlimeInfo GetSlimeInfo(int index)
        {
            if (SlimeInfoList != null)
            {
                if (SlimeInfoList.Count > index && index >= 0)
                {
                    return SlimeInfoList[index];
                }
            }

            return null;
        }

        public SavedSlimeInfo GetSlimeInfo(string slimeId)
        {
            if (SlimeInfoList != null)
            {
                return SlimeInfoList.Where(x => x.Id == slimeId).FirstOrDefault();
            }

            return null;
        }

        public bool StackGoldSlimeSpawnCount()
        {
            int maxCount = MLand.GameData.GoldSlimeCommonData.maxDailyCount;

            return GoldSlimeSpawnCounter.StackCount(maxCount);
        }

        public bool AnyCanReceiveLevelReward()
        {
            foreach(var slimeInfo in SlimeInfoList)
            {
                if (slimeInfo.AnyCanReceiveLevelReward())
                    return true;
            }

            return false;
        }

        public bool AnyCanReceiveLevelReward(string id)
        {
            var slimeInfo = GetSlimeInfo(id);
            if (slimeInfo == null)
                return false;

            return slimeInfo.AnyCanReceiveLevelReward();
        }

        public bool CanReceiveLevelReward(string id, int level)
        {
            var slimeInfo = GetSlimeInfo(id);
            if (slimeInfo == null)
                return false;

            return slimeInfo.CanReceiveLevelReward(level);
        }

        public int GetSlimeIndex(string slimeId)
        {
            var slimeInfo = GetSlimeInfo(slimeId);
            if (slimeInfo != null)
            {
                return SlimeInfoList.IndexOf(slimeInfo);
            }
            else
            {
                return 0;
            }
        }
    }
}