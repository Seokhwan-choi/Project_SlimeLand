using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using System;


namespace MLand
{
    [Serializable]
    public class OfflineRewardManager : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mLevel;
        [SerializeField]
        int mLastUpdateTime;

        public ObscuredInt Level;             // 단계
        public ObscuredInt LastUpdateTime;
        public void Init()
        {
            Level = 1;
            LastUpdateTime = 0;
        }

        public void Normalize()
        {
            if (Level == 0)
                Level = 1;
        }

        public void RandomizeCryptoKey()
        {
            Level.RandomizeCryptoKey();
            LastUpdateTime.RandomizeCryptoKey();
        }

        public void OnBeforeSerialize()
        {
            mLevel = Level;
            mLastUpdateTime = LastUpdateTime;
        }

        public void OnAfterDeserialize()
        {
            Level = mLevel;
            LastUpdateTime = mLastUpdateTime;
        }

        public void OnUpdate()
        {
            LastUpdateTime = TimeUtil.Now;
        }

        public void TakeReward()
        {
            LastUpdateTime = TimeUtil.Now;
        }

        public bool LevelUp()
        {
            if (IsMaxLevel())
                return false;

            Level = Mathf.Min(Level + 1, GetMaxLevel());

            return true;
        }

        public bool HaveRewardToReceive()
        {
            if (LastUpdateTime == 0)
                return false;

            // 기본 튜토리얼 아직 진행 안되었으면 무시
            if (SavePointBitFlags.Tutorial_4_MiniGame.IsOff())
                return false;

            int rewardValue = CalcRewardTimeForMinute();

            return rewardValue >= 1;
        }

        int GetMaxMinute()
        {
            var levelData = MLand.GameData.OfflineRewardLevelUpData.Where(x => x.level == Level).FirstOrDefault();

            return MLand.GameData.OfflineRewardCommonData.defaultMaxMinute + (levelData?.addTimeForMinute ?? 0);
        }

        public bool IsMaxReward()
        {
            return CalcRewardTimeForMinute() >= GetMaxMinute();
        }

        int GetMaxLevel()
        {
            return MLand.GameData.OfflineRewardLevelUpData.Max(x => x.level);
        }

        public bool IsMaxLevel()
        {
            return GetMaxLevel() <= Level;
        }

        int GetTimeValue()
        {
            return TimeUtil.Now - LastUpdateTime;
        }

        public int CalcRewardTimeForMinute()
        {
            int timeValue = GetTimeValue();
            if (timeValue > 0)
            {
                int rewardTimeForMinute = timeValue / TimeUtil.SecondsInMinute;

                rewardTimeForMinute = Mathf.Min(rewardTimeForMinute, GetMaxMinute());

                return rewardTimeForMinute;
            }
            else
            {
                // 음수가 나오거나 0은 이상한거다.
                return 0;
            }
        }
    }
}