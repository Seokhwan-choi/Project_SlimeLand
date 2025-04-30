using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Linq;
using System;

namespace MLand
{
    [Serializable]
    public struct BuffInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mBuffTypeInt;
        [SerializeField]
        int mBuffDurationEndTime;
        [SerializeField]
        int mBuffCoolTimeEndTime;

        public BuffType Type => (BuffType)(int)BuffTypeInt;
        public ObscuredInt BuffTypeInt;
        public ObscuredInt BuffDurationEndTime;   // 버프가 지속시간 종료시간
        public ObscuredInt BuffCoolTimeEndTime;   // 버프 쿨타임 종료시간
        public DailyCounter DailyCounter;         // 하루 횟수 제한
        public bool IsActive => BuffDurationEndTime > 0 && BuffDurationEndTime > TimeUtil.Now;
        public float BuffValue => MLand.GameData.BuffData.TryGet(Type).buffValue;
        public int MaxCount => MLand.GameData.BuffData.TryGet(Type).maxDailyCount;

        public void Normalize()
        {
            if (DailyCounter == null)
            {
                DailyCounter = new DailyCounter();
            }
        }

        public bool IsEnoughDailyCounter()
        {
            return DailyCounter?.IsMaxCount(MaxCount) == false;
        }

        public void SetBuffDurationEndTime(int durationTime)
        {
            BuffDurationEndTime = TimeUtil.Now + durationTime;
        }
        public void SetBuffCoolTimeEndTime(int coolTime)
        {
            BuffCoolTimeEndTime = TimeUtil.Now + coolTime;
        }

        public void ClearBuffDurationEnd()
        {
            BuffDurationEndTime = 0;
        }

        public void ClearBuffCoolTimeEnd()
        {
            BuffCoolTimeEndTime = 0;
        }

        public void OnBeforeSerialize()
        {
            mBuffTypeInt = BuffTypeInt;
            mBuffDurationEndTime = BuffDurationEndTime;
            mBuffCoolTimeEndTime = BuffCoolTimeEndTime;
        }

        public void OnAfterDeserialize()
        {
            BuffTypeInt = mBuffTypeInt;
            BuffDurationEndTime = mBuffDurationEndTime;
            BuffCoolTimeEndTime = mBuffCoolTimeEndTime;
        }

        public void RandomizeKey()
        {
            BuffTypeInt.RandomizeCryptoKey();
            BuffDurationEndTime.RandomizeCryptoKey();
            BuffCoolTimeEndTime.RandomizeCryptoKey();
            DailyCounter?.RandomizeKey();
        }

        public void ActiveBuff(int intDuration)
        {
            if (DailyCounter?.StackCount(MaxCount) ?? false)
            {
                SetBuffDurationEndTime(intDuration);
                ClearBuffCoolTimeEnd();
            }
        }
    }

    [Serializable]
    public class SavedBuffManager
    {
        public List<BuffInfo> BuffList;
        public void Normalize()
        {
            if (BuffList == null)
                BuffList = new List<BuffInfo>();

            if (BuffList != null)
            {
                foreach(BuffType type in MLand.GameData.BuffData.Keys)
                {
                    if (BuffList.Count(x => x.Type == type) <= 0)
                    {
                        BuffList.Add(new BuffInfo()
                        {
                            BuffTypeInt = (int)type,
                            DailyCounter = new DailyCounter()
                        });
                    }
                    else
                    {
                        var buffInfo = BuffList.Where(x => x.Type == type).First();

                        buffInfo.Normalize();

                        ChangeBuffInfo(buffInfo);
                    }
                }
            }
        }

        public void RandomizeKey()
        {
            if (BuffList != null)
            {
                for(int i = 0; i < BuffList.Count; ++i)
                {
                    BuffList[i].RandomizeKey();
                }
            }
        }

        public void ChangeBuffInfo(BuffInfo buffInfo)
        {
            for(int i = 0; i < BuffList.Count; ++i)
            {
                if (BuffList[i].Type == buffInfo.Type)
                {
                    BuffList[i] = buffInfo;

                    break;
                }
            }
        }

        public bool IsEnoughActiveCount(BuffType buffType)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                if (BuffList[i].Type == buffType)
                {
                    BuffInfo buff = BuffList[i];

                    return buff.IsEnoughDailyCounter();
                }
            }

            return false;
        }

        public void ActiveBuff(BuffType buffType, float duration)
        {
            for(int i = 0; i < BuffList.Count; ++i)
            {
                if ( BuffList[i].Type == buffType )
                {
                    int intDuration = Mathf.RoundToInt(duration);

                    BuffInfo buff = BuffList[i];

                    if (buff.IsEnoughDailyCounter())
                    {
                        buff.ActiveBuff(intDuration);

                        BuffList[i] = buff;

                        break;
                    }
                }
            }
        }

        public void SetCoolTimeBuff(BuffType buffType, float coolTime)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                if (BuffList[i].Type == buffType)
                {
                    int intCoolTime = Mathf.RoundToInt(coolTime);

                    BuffInfo buff = BuffList[i];

                    buff.ClearBuffDurationEnd();
                    buff.SetBuffCoolTimeEndTime(intCoolTime);

                    BuffList[i] = buff;
                }
            }
        }

        public void ClearBuff(BuffType type)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                if (BuffList[i].Type == type)
                {
                    BuffInfo buff = BuffList[i];

                    buff.ClearBuffDurationEnd();
                    buff.ClearBuffCoolTimeEnd();

                    BuffList[i] = buff;
                }
            }
        }

        public float GetBuffValue(BuffType type)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                if (BuffList[i].Type == type &&
                    BuffList[i].IsActive)
                    return BuffList[i].BuffValue;
            }

            return 1f;
        }

        public BuffInfo GetBuffInfo(BuffType type)
        {
            return BuffList.Where(x => x.Type == type).FirstOrDefault();
        }
    }

    static class BuffInfoExt
    {

    }
}