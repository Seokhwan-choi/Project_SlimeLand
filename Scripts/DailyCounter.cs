using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class DailyCounter : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mLastUpdateDateNum;
        [SerializeField]
        int mStackedCount;

        public ObscuredInt LastUpdateDateNum;
        public ObscuredInt StackedCount;

        public void RandomizeKey()
        {
            LastUpdateDateNum.RandomizeCryptoKey();
            StackedCount.RandomizeCryptoKey();
        }

        public void OnBeforeSerialize()
        {
            mLastUpdateDateNum = LastUpdateDateNum;
            mStackedCount = StackedCount;
        }

        public void OnAfterDeserialize()
        {
            LastUpdateDateNum = mLastUpdateDateNum;
            StackedCount = mStackedCount;
        }

        public bool IsMaxCount(int maxCount)
        {
            Update();

            return StackedCount == maxCount;
        }

        public bool StackCount(int maxCount)
        {
            if (IsMaxCount(maxCount))
                return false;

            StackedCount += 1;

            return true;
        }

        public int GetRemainCount(int maxCount)
        {
            Update();

            return Mathf.Max(0, maxCount - StackedCount);
        }

        void Update()
        {
            int nowDateNum = TimeUtil.NowDateNum();

            int dateNumGap = LastUpdateDateNum - nowDateNum;
            if (dateNumGap > 2 * 10000)
            {
                Refresh();
            }

            if (LastUpdateDateNum < nowDateNum)
            {
                Refresh();
            }

            void Refresh()
            {
                LastUpdateDateNum = nowDateNum;

                StackedCount = 0;
            }
        }
    }
}