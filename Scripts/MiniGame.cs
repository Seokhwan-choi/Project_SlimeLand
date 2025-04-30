using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class MiniGame : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<int> mTypeList;
        [SerializeField]
        List<DailyCounter> mDailyCounterList;

        [SerializeField]
        List<int> mWatchAdTypeList;
        [SerializeField]
        List<DailyCounter> mWatchAdCounterList;

        public Dictionary<ObscuredInt, DailyCounter> DailyCounterDics;
        public Dictionary<ObscuredInt, DailyCounter> WatchAdCounterDics;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (DailyCounterDics == null)
            {
                mTypeList = new List<int>();
                mDailyCounterList = new List<DailyCounter>();
                DailyCounterDics = new Dictionary<ObscuredInt, DailyCounter>();
                WatchAdCounterDics = new Dictionary<ObscuredInt, DailyCounter>();

                for (int i = 0; i < (int)MiniGameType.Count; ++i)
                {
                    DailyCounterDics.Add(i, new DailyCounter());
                    WatchAdCounterDics.Add(i, new DailyCounter());
                }
            }
            else if (WatchAdCounterDics == null || WatchAdCounterDics.Count <= 0)
            {
                for (int i = 0; i < (int)MiniGameType.Count; ++i)
                {
                    WatchAdCounterDics.Add(i, new DailyCounter());
                }
            }
        }

        public void RandomizeKey()
        {
            if (DailyCounterDics != null)
            {
                foreach (var dailyCounter in DailyCounterDics)
                {
                    dailyCounter.Key.RandomizeCryptoKey();
                    dailyCounter.Value.RandomizeKey();
                }
            }
            
            if (WatchAdCounterDics != null)
            {
                foreach ( var dailyCounter in WatchAdCounterDics)
                {
                    dailyCounter.Key.RandomizeCryptoKey();
                    dailyCounter.Value.RandomizeKey();
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (DailyCounterDics != null)
            {
                mTypeList = DailyCounterDics.Keys.Select(x => (int)x).ToList();
                mDailyCounterList = DailyCounterDics.Values.ToList();
            }

            if (WatchAdCounterDics != null)
            {
                mWatchAdTypeList = WatchAdCounterDics.Keys.Select(x => (int)x).ToList();
                mWatchAdCounterList = WatchAdCounterDics.Values.ToList();
            }
        }

        public void OnAfterDeserialize()
        {
            if (mTypeList != null && mDailyCounterList != null)
            {
                if (mTypeList.Count == mDailyCounterList.Count)
                {
                    DailyCounterDics = new Dictionary<ObscuredInt, DailyCounter>();

                    for (int i = 0; i < mTypeList.Count; ++i)
                    {
                        int key = mTypeList[i];
                        DailyCounter value = mDailyCounterList[i];

                        DailyCounterDics.Add(key, value);
                    }
                }
            }

            if (mWatchAdTypeList != null && mWatchAdCounterList != null)
            {
                if (mWatchAdTypeList.Count == mWatchAdCounterList.Count)
                {
                    WatchAdCounterDics = new Dictionary<ObscuredInt, DailyCounter>();

                    for (int i = 0; i < mWatchAdTypeList.Count; ++i)
                    {
                        int key = mWatchAdTypeList[i];
                        DailyCounter value = mWatchAdCounterList[i];

                        WatchAdCounterDics.Add(key, value);
                    }
                }
            }
        }

        public bool StackCount(MiniGameType miniGame)
        {
            int typeInt = (int)miniGame;

            int maxCount = GetDailyFreePlayCount(miniGame);

            DailyCounter dailyCounter = DailyCounterDics.TryGet(typeInt);
            if ( dailyCounter.StackCount(maxCount) )
            {
                return true;
            }

            return false;
        }

        public bool IsMaxFreePlayCount(MiniGameType miniGame)
        {
            int typeInt = (int)miniGame;

            DailyCounter dailyCounter = DailyCounterDics.TryGet(typeInt);

            int maxCount = GetDailyFreePlayCount(miniGame);

            return dailyCounter.IsMaxCount(maxCount);
        }

        public int GetRemainFreePlayCount(MiniGameType miniGame)
        {
            if (IsMaxFreePlayCount(miniGame))
                return 0;

            int typeInt = (int)miniGame;

            var dailyCounter = DailyCounterDics.TryGet(typeInt);
            if (dailyCounter == null)
                return 0;

            int maxCount = GetDailyFreePlayCount(miniGame);

            return maxCount - dailyCounter.StackedCount;
        }

        int GetDailyFreePlayCount(MiniGameType miniGame)
        {
            return miniGame == MiniGameType.ElementalCourses ?
                MLand.GameData.MiniGameElementalCoursesData.dailyFreePlayCount :
                MLand.GameData.MiniGameTicTacToeData.dailyFreePlayCount;
        }


        public bool StackWatchAdCount(MiniGameType miniGame)
        {
            int typeInt = (int)miniGame;

            int maxCount = GetWatchAdCount(miniGame);

            DailyCounter dailyCounter = WatchAdCounterDics.TryGet(typeInt);
            if (dailyCounter.StackCount(maxCount))
            {
                return true;
            }

            return false;
        }

        public bool IsMaxWatchAdCount(MiniGameType miniGame)
        {
            int typeInt = (int)miniGame;

            DailyCounter dailyCounter = WatchAdCounterDics.TryGet(typeInt);

            int maxCount = GetWatchAdCount(miniGame);

            return dailyCounter.IsMaxCount(maxCount);
        }

        public int GetRemainWatchAdCount(MiniGameType miniGame)
        {
            if (IsMaxWatchAdCount(miniGame))
                return 0;

            int typeInt = (int)miniGame;

            var dailyCounter = WatchAdCounterDics.TryGet(typeInt);
            if (dailyCounter == null)
                return 0;

            int maxCount = GetWatchAdCount(miniGame);

            return maxCount - dailyCounter.StackedCount;
        }

        public int GetWatchAdCount(MiniGameType miniGame)
        {
            return miniGame == MiniGameType.ElementalCourses ?
                MLand.GameData.MiniGameElementalCoursesData.dailyFreePlayCount :
                MLand.GameData.MiniGameTicTacToeData.dailyFreePlayCount;
        }
    }
}