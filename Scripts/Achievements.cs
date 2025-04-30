using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    // 큰 틀은 퀘스트랑 다르지 않다..
    [Serializable]
    public class Achievements : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> mKeyList;
        [SerializeField]
        List<AchievementsInfo> mInfoList;
        [SerializeField]
        List<string> mAlreadyShowPopupKeys;

        public List<ObscuredString> AlreadyShowPopupKeys;
        public Dictionary<ObscuredString, AchievementsInfo> AchievementsDics;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (AchievementsDics == null)
                AchievementsDics = new Dictionary<ObscuredString, AchievementsInfo>();

            if (AlreadyShowPopupKeys == null)
                AlreadyShowPopupKeys = new List<ObscuredString>();

            foreach (AchievementsData data in MLand.GameData.AchievementsData.Values)
            {
                // 이미 추가된적이 있는 업적이라면 추가하지 말자
                if (AchievementsDics.ContainsKey(data.id))
                    continue;

                AchievementsInfo info = new AchievementsInfo();

                info.Init(data.id);

                AchievementsDics.Add(data.id, info);
            }
        }

        public bool ReceiveReward(string id)
        {
            AchievementsInfo info = AchievementsDics?.TryGet(id);
            if (info == null)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            if (info.IsReceivedReward)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            if (info.CanReceiveReward == false)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            info.IsReceivedReward = true;

            return true;
        }

        public bool CheckAchievemets(AchievementsType type, double count = 1)
        {
            IEnumerable<AchievementsInfo> achievementsArray = GetAchievementsByType(type);

            if (achievementsArray != null && achievementsArray.Count() > 0)
            {
                foreach (AchievementsInfo achievements in achievementsArray)
                {
                    achievements.StackRequireCount(count);

                    if (achievements.IsReceivedReward == false)
                    {
                        if (achievements.CanReceiveReward)
                        {
                            if (AlreadyShowPopupKeys.Contains(achievements.Id) == false)
                            {
                                MLand.Lobby.ShowAchievementsPopup(achievements.Id);

                                AlreadyShowPopupKeys.Add(achievements.Id);

                                MLand.GPGSBinder.UnlockAchievement(GetGPGId(achievements.Id));
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public AchievementsInfo GetAchievementsInfo(string id)
        {
            return AchievementsDics?.TryGet(id);
        }

        public bool AnyAchievementsCanReceiveReward()
        {
            if (AchievementsDics != null)
            {
                foreach (var questInfo in AchievementsDics.Values)
                {
                    if (questInfo.CanReceiveReward &&
                        questInfo.IsReceivedReward == false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        IEnumerable<AchievementsInfo> GetAchievementsByType(AchievementsType type)
        {
            if (AchievementsDics != null)
                return AchievementsDics.Values.Where(x => x.Type == type);
            else
                return null;
        }

        public void RandomizeKey()
        {
            if (AchievementsDics != null)
            {
                foreach (var achievements in AchievementsDics)
                {
                    achievements.Key.RandomizeCryptoKey();
                    achievements.Value.RandomizeKey();
                }
            }

            if (AlreadyShowPopupKeys != null)
            {
                foreach (var key in AlreadyShowPopupKeys)
                {
                    key.RandomizeCryptoKey();
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (mKeyList != null && mInfoList != null)
            {
                if (mKeyList.Count == mInfoList.Count)
                {
                    AchievementsDics = new Dictionary<ObscuredString, AchievementsInfo>();

                    for (int i = 0; i < mKeyList.Count; ++i)
                    {
                        var key = mKeyList[i];
                        var value = mInfoList[i];

                        AchievementsDics.Add(key, value);
                    }
                }
            }

            if (mAlreadyShowPopupKeys != null)
            {
                AlreadyShowPopupKeys = new List<ObscuredString>();

                foreach (var key in mAlreadyShowPopupKeys)
                {
                    AlreadyShowPopupKeys.Add(key);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (AchievementsDics != null)
            {
                mKeyList = AchievementsDics.Select(x => (string)x.Key).ToList();
                mInfoList = AchievementsDics.Select(x => x.Value).ToList();
            }

            if (AlreadyShowPopupKeys != null)
            {
                mAlreadyShowPopupKeys = AlreadyShowPopupKeys.Select(x => (string)x).ToList();
            }
        }

        string GetGPGId(string id)
        {
            if (id == "Achievements_LuckySymbol")
                return GPGSIds.Achievements_LuckySymbol;
            else if (id == "Achievements_CottonCandy")
                return GPGSIds.Achievements_CottonCandy;
            else if (id == "Achievements_BestFriend")
                return GPGSIds.Achievements_BestFriend;
            else if (id == "Achievements_SpaceOut_5")
                return GPGSIds.Achievements_SpaceOut_5;
            else if (id == "Achievements_SpaceOut_60")
                return GPGSIds.Achievements_SpaceOut_60;
            else if (id == "Achievements_SpaceOut_600")
                return GPGSIds.Achievements_SpaceOut_600;
            else if (id == "Achievements_PerfectAttendance")
                return GPGSIds.Achievements_PerfectAttendance;
            else if (id == "Achievements_HonorStudent")
                return GPGSIds.Achievements_HonorStudent;
            else if (id == "Achievements_Influencer")
                return GPGSIds.Achievements_Influencer;
            else if (id == "Achievements_GoldHunter")
                return GPGSIds.Achievements_GoldHunter;
            else if (id == "Achievements_EyeShopping")
                return GPGSIds.Achievements_EyeShopping;
            else if (id == "Achievements_SlimeCoreCollector")
                return GPGSIds.Achievements_SlimeCoreCollector;
            else
                return string.Empty;
        }
    }

    [Serializable]
    public class AchievementsInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        string mId;
        [SerializeField]
        double mRequireCount;
        [SerializeField]
        bool mIsReceivedReward;

        public ObscuredString Id;
        public ObscuredDouble RequireCount;
        public ObscuredBool IsReceivedReward;

        public bool CanReceiveReward => RequireCount >= MaxRequireCount;
        public AchievementsType Type => Data.type;
        public int MaxRequireCount => Data.requireCount;
        public AchievementsData Data => MLand.GameData.AchievementsData.TryGet(Id);
        public void Init(string id)
        {
            Id = id;
            RequireCount = 0;
            IsReceivedReward = false;
        }

        public void RandomizeKey()
        {
            Id.RandomizeCryptoKey();
            RequireCount.RandomizeCryptoKey();
            IsReceivedReward.RandomizeCryptoKey();
        }

        public void OnAfterDeserialize()
        {
            Id = mId;
            RequireCount = mRequireCount;
            IsReceivedReward = mIsReceivedReward;
        }

        public void OnBeforeSerialize()
        {
            if (Id != null)
                mId = Id;

            mRequireCount = RequireCount;
            mIsReceivedReward = IsReceivedReward;
        }

        public void StackRequireCount(double count = 1)
        {
            if (Data.requireType == AchievementsRequireType.Stack)
            {
                RequireCount += count;
                RequireCount = Math.Min(RequireCount, MaxRequireCount);
            }
            else
            {
                if (MaxRequireCount <= count)
                {
                    RequireCount = MaxRequireCount;
                }
            }
        }
    }
}