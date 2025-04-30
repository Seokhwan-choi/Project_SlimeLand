using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class StepQuest : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> mAlreadShowMessageKeys;
        [SerializeField]
        List<string> mStepQuestKeys;
        [SerializeField]
        List<StepQuestInfo> mStepQuestInfos;

        public List<ObscuredString> AlreadShowMessageKeys;
        public Dictionary<ObscuredString, StepQuestInfo> StepQuestInfoDics;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (StepQuestInfoDics == null)
            {
                mStepQuestKeys = new List<string>();
                mStepQuestInfos = new List<StepQuestInfo>();

                StepQuestInfoDics = new Dictionary<ObscuredString, StepQuestInfo>();

                // 아이디 목록을 추출하고 업적을 생성하자
                foreach (string id in MLand.GameData.StepQuestData.Select(x => x.id).Distinct())
                {
                    StepQuestInfo info = new StepQuestInfo();

                    info.Init(id);

                    StepQuestInfoDics.Add(id, info);
                }
            }
            else
            {
                foreach(string id in MLand.GameData.StepQuestData.Select(x => x.id).Distinct())
                {
                    var info = StepQuestInfoDics.TryGet(id);
                    if (info == null)
                    {
                        info = new StepQuestInfo();

                        info.Init(id);

                        StepQuestInfoDics.Add(id, info);
                    }
                    else
                    {
                        if (info.IsLastStep == false)
                        {
                            if (info.IsReceivedRewardStep(info.CurrentStep))
                            {
                                info.NextStep();
                            }
                        }
                    }
                }
            }

            if (AlreadShowMessageKeys == null)
            {
                mAlreadShowMessageKeys = new List<string>();
                AlreadShowMessageKeys = new List<ObscuredString>();
            }
        }

        public bool ReceiveCurrentStepReward(string id)
        {
            var info = StepQuestInfoDics.TryGet(id);

            return info.ReceiveCurrentStepReward();
        }

        public void RandomizeKey()
        {
            if (StepQuestInfoDics != null)
            {
                foreach (var info in StepQuestInfoDics.Values)
                    info.RandomizeKey();
            }
        }

        public bool Stack(QuestType type, int count = 1)
        {
            IEnumerable<StepQuestInfo> infos = GetStepQuestInfos(type);
            if (infos != null && infos.Count() > 0)
            {
                foreach (StepQuestInfo info in infos)
                {
                    info.Stack(count);

                    int currentStep = info.CurrentStep;

                    if (info.IsReceivedRewardStep(currentStep) == false)
                    {
                        if (info.IsSatisfiedStack())
                        {
                            string key = $"{info.Id}_{currentStep}";

                            if (AlreadShowMessageKeys.Contains(key) == false)
                            {
                                MLand.Lobby.AddToastMessage(info.Id, currentStep);

                                AlreadShowMessageKeys.Add(key);

                                SoundPlayer.PlayQuestClear();
                            }
                        }
                    }
                }
                    

                return true;
            }

            return false;
        }

        public bool AnyQuestCanReceiveReward()
        {
            foreach(var questInfo in StepQuestInfoDics.Values)
            {
                if (questInfo.IsSatisfiedStack() &&
                    questInfo.IsFinishAllSteps() == false)
                {
                    return true;
                }
            }

            return false;
        }

        IEnumerable<StepQuestInfo> GetStepQuestInfos(QuestType type)
        {
            foreach(StepQuestInfo info in StepQuestInfoDics.Values)
            {
                if (info.IsFinishAllSteps())
                    continue;

                if (info.Type == type)
                    yield return info;
            }
        }

        public void OnBeforeSerialize()
        {
            if (StepQuestInfoDics != null)
            {
                mStepQuestKeys = StepQuestInfoDics.Select(x => (string)x.Key).ToList();
                mStepQuestInfos = StepQuestInfoDics.Select(x => x.Value).ToList();
            }
        }

        public void OnAfterDeserialize()
        {
            if (mStepQuestKeys != null && mStepQuestInfos != null)
            {
                if (mStepQuestKeys.Count == mStepQuestInfos.Count)
                {
                    StepQuestInfoDics = new Dictionary<ObscuredString, StepQuestInfo>();

                    for(int i = 0; i < mStepQuestKeys.Count; ++i)
                    {
                        string key = mStepQuestKeys[i];
                        StepQuestInfo value = mStepQuestInfos[i];

                        StepQuestInfoDics.Add(key, value);
                    }
                }
            }
        }
    }

    [Serializable]
    public class StepQuestInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        string mId;
        [SerializeField]
        int mTypeInt;
        [SerializeField]
        int mStackedCount;
        [SerializeField]
        int mCurrentStep;
        [SerializeField]
        List<int> mReceivedStepList;

        public ObscuredString Id;
        public ObscuredInt TypeInt;
        public ObscuredInt StackedCount;
        public ObscuredInt CurrentStep;
        public List<ObscuredInt> ReceivedStepList;
        public QuestType Type => (QuestType)(int)TypeInt;
        public StepQuestData CurrentData => DataUtil.GetStepQuestData(Id, CurrentStep);
        public int MaxStep => DataUtil.GetStepQuestMaxStep(Id);
        public bool IsLastStep => MaxStep == CurrentStep;
        public void Init(string id)
        {
            Id = id;
            CurrentStep = 1;
            StackedCount = 0;
            TypeInt = (int)CurrentData.type;
            ReceivedStepList = new List<ObscuredInt>();
        }

        public void Stack(int count)
        {
            StackedCount += count;
        }

        public bool IsReceivedRewardStep(int step)
        {
            return ReceivedStepList.Contains(step);
        }

        public bool IsFinishAllSteps()
        {
            return ReceivedStepList.Contains(MaxStep);
        }

        public bool IsSatisfiedStack()
        {
            return CurrentData.requireStackCount <= StackedCount;
        }

        public bool ReceiveCurrentStepReward()
        {
            // 모든 스탭이 마무리되었는데 받을 수 없다.
            if (IsFinishAllSteps())
                return false;

            // 현재 스탭 보상을 이미 받았다.
            if (IsReceivedRewardStep(CurrentStep))
                return false;

            // 조건을 충족하지 못하였다.
            if (IsSatisfiedStack() == false)
                return false;
            
            // 현재 스탭 보상 받았음을 저장
            ReceivedStepList.Add(CurrentStep);
            // 다음 스탭으로
            NextStep();

            return true;
        }

        public void NextStep()
        {
            CurrentStep = Math.Min(CurrentStep + 1, MaxStep);
        }

        public void RandomizeKey()
        {
            if (Id != null)
                Id.RandomizeCryptoKey();

            CurrentStep.RandomizeCryptoKey();
            StackedCount.RandomizeCryptoKey();
            TypeInt.RandomizeCryptoKey();

            if (ReceivedStepList != null)
            {
                foreach (ObscuredInt step in ReceivedStepList)
                    step.RandomizeCryptoKey();
            }
        }

        public void OnAfterDeserialize()
        {
            Id = mId;
            TypeInt = mTypeInt;
            StackedCount = mStackedCount;
            CurrentStep = mCurrentStep;

            if (mReceivedStepList != null)
            {
                ReceivedStepList = mReceivedStepList.Select(x => (ObscuredInt)x).ToList();
            }
        }

        public void OnBeforeSerialize()
        {
            if (Id != null)
                mId = Id;

            mTypeInt = TypeInt;
            mStackedCount = StackedCount;
            mCurrentStep = CurrentStep;

            if (ReceivedStepList != null)
            {
                mReceivedStepList = ReceivedStepList.Select(x => (int)x).ToList();
            }
        }
    }
}