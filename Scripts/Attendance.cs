using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class Attendance : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mDay;
        [SerializeField]
        int mLastCheckDateNum;
        [SerializeField]
        List<int> mReceivedRewardDayList;

        public ObscuredInt Day;
        public ObscuredInt LastCheckDateNum;
        public List<ObscuredInt> ReceivedRewardDayList;
        int MaxDay => MLand.GameData.AttendanceCommonData.maxDay;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (ReceivedRewardDayList == null)
            {
                mReceivedRewardDayList = new List<int>();
                ReceivedRewardDayList = new List<ObscuredInt>();
            }
        }

        public void OnBeforeSerialize()
        {
            mDay = Day;
            mLastCheckDateNum = LastCheckDateNum;
            mReceivedRewardDayList = ReceivedRewardDayList.Select(x => (int)x).ToList();
        }

        public void OnAfterDeserialize()
        {
            Day = mDay;
            LastCheckDateNum = mLastCheckDateNum;
            ReceivedRewardDayList = mReceivedRewardDayList.Select(x => (ObscuredInt)x).ToList();
        }

        public void RandomizeKey()
        {
            Day.RandomizeCryptoKey();
            LastCheckDateNum.RandomizeCryptoKey();

            if (ReceivedRewardDayList != null)
            {
                for(int i = 0; i < ReceivedRewardDayList.Count; ++i)
                {
                    var value = ReceivedRewardDayList[i];

                    value.RandomizeCryptoKey();

                    ReceivedRewardDayList[i] = value;
                }
            }
        }

        public bool Check()
        {
            int nowDateNum = TimeUtil.NowDateNum();

            int dateNumGap = LastCheckDateNum - nowDateNum;
            if (dateNumGap > 2 * 10000)
            {
                Refresh();

                return true;
            }

            if (LastCheckDateNum < nowDateNum)
            {
                Refresh();

                return true;
            }

            void Refresh()
            {
                LastCheckDateNum = nowDateNum;

                // 출석부 초기화
                if (Day + 1 >= MaxDay + 1)
                {
                    ReceivedRewardDayList.Clear();
                }

                Day = Math.Max(1, (Day + 1) % (MaxDay + 1));
            }

            return false;
        }

        public bool ReceiveReward(int day)
        {
            if (IsReadyForReceiveReward(day) == false)
                return false;

            ReceivedRewardDayList.Add(day);

            return true;
        }

        public bool IsReadyForReceiveReward(int day)
        {
            bool isReady = Day >= day;
            if (isReady)
                isReady = IsAlreadyReceivedReward(day) == false;

            return isReady;
        }

        public bool AnyCanReceiveReward()
        {
            for(int day = 1; day <= MaxDay; ++day)
            {
                if (IsReadyForReceiveReward(day))
                    return true;
            }

            return false;
        }

        public bool IsAlreadyReceivedReward(int day)
        {
            return ReceivedRewardDayList.Contains(day);
        }

        public void Reset()
        {
            Day = 0;

            LastCheckDateNum = TimeUtil.NowDateNum();

            ReceivedRewardDayList = new List<ObscuredInt>();
        }
    }
}