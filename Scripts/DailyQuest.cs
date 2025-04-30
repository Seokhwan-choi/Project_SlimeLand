using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class DailyQuest : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mLastUpdateNum;
        [SerializeField]
        List<string> mAlreadyShowMessageQuestKeys;
        [SerializeField]
        List<string> mQuestKeyList;
        [SerializeField]
        List<DailyQuestInfo> mQuestInfoList;

        public ObscuredInt LastUpdateDateNum;
        public List<ObscuredString> AlreadyShowMessageQuestKeys;
        public Dictionary<ObscuredString, DailyQuestInfo> Quests;

        public bool Update()
        {
            int nowDateNum = TimeUtil.NowDateNum();

            int dateNumGap = LastUpdateDateNum - nowDateNum;
            if (dateNumGap > 2 * 10000)
            {
                Refresh();

                return true;
            }

            // 하루가 지나면 퀘스트를 모두 밀어버린다.
            if (LastUpdateDateNum < nowDateNum)
            {
                Refresh();

                return true;
            }

            void Refresh()
            {
                LastUpdateDateNum = nowDateNum;

                CreateNewQuests();
            }

            return false;
        }

        public bool ReceiveReward(string id)
        {
            DailyQuestInfo questInfo = Quests?.TryGet(id);
            if (questInfo == null)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            if (questInfo.IsReceivedReward)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            if (questInfo.CanReceiveReward == false)
            {
                SoundPlayer.PlayErrorSound();

                return false;
            }

            questInfo.IsReceivedReward = true;

            return true;
        }

        public bool CheckQuest(QuestType type, int count = 1)
        {
            IEnumerable<DailyQuestInfo> quests = GetQuestByType(type);
            
            if (quests != null && quests.Count() > 0)
            {
                foreach(DailyQuestInfo quest in quests)
                {
                    quest.StackRequireCount(count);

                    if (quest.IsReceivedReward == false)
                    {
                        if (quest.CanReceiveReward)
                        {
                            if (AlreadyShowMessageQuestKeys.Contains(quest.Id) == false)
                            {
                                MLand.Lobby.AddToastMessage(quest.Id);

                                AlreadyShowMessageQuestKeys.Add(quest.Id);
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
            if (Quests != null)
            {
                foreach (var questInfo in Quests.Values)
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

        void CreateNewQuests()
        {
            Quests = new Dictionary<ObscuredString, DailyQuestInfo>();
            AlreadyShowMessageQuestKeys = new List<ObscuredString>();

            Dictionary<string, DailyQuestData>.ValueCollection orgDatas = MLand.GameData.DailyQuestData.Values;

            // 일일 퀘스트 완료 퀘스트는 일단 목록에서 제외해준다.
            IEnumerable<DailyQuestData> shuffleDatas = DataUtil.GetShuffleDatas(orgDatas)
                .Where(x => x.questType != QuestType.DailyquestClear);

            foreach (DailyQuestData data in shuffleDatas)
            {
                // 이미 추가된적이 있는 퀘스트 타입이라면 무시하자
                var quest = GetQuestByType(data.questType);
                if (quest != null && quest.Count() > 0)
                    continue;

                if (data.questType == QuestType.FindGoldSlime)
                {
                    if (SavePointBitFlags.Tutorial_6_GoldSlime.IsOff())
                        continue;
                }
                else if (data.questType == QuestType.VisitExpensiveShop)
                {
                    if (SavePointBitFlags.Tutorial_5_ExpensiveShop.IsOff())
                        continue;
                }

                DailyQuestInfo newQuestInfo = new DailyQuestInfo();

                //if (Quests.Count == 0)
                //{
                //    newQuestInfo.Init("DailyQuest_PhotoShare_1");

                //    Quests.Add("DailyQuest_PhotoShare_1", newQuestInfo);
                //}
                //else
                {
                    newQuestInfo.Init(data.id);

                    Quests.Add(data.id, newQuestInfo);
                }

                // 최대갯수까지 퀘스트를 모두 채워 넣었으면
                if (Quests.Count >= MLand.GameData.DailyQuestCommonData.maxQuestCount)
                {
                    // 퀘스트 모두 완료 퀘스트를 넣어주자
                    var questClearData = DataUtil.GetDailyQuestDatasByType(QuestType.DailyquestClear).FirstOrDefault();
                    if (questClearData != null)
                    {
                        DailyQuestInfo info = new DailyQuestInfo();

                        info.Init(questClearData.id);

                        Quests.Add(questClearData.id, info);
                    }

                    break;
                }
            }
        }

        IEnumerable<DailyQuestInfo> GetQuestByType(QuestType type)
        {
            if (Quests != null)
                return Quests.Values.Where(x => x.Type == type);
            else
                return null;
        }

        public void RandomizeKey()
        {
            LastUpdateDateNum.RandomizeCryptoKey();

            if (Quests != null)
            {
                foreach (var quest in Quests)
                {
                    quest.Key.RandomizeCryptoKey();
                    quest.Value.RandomizeKey();
                }
            }
            
            if (AlreadyShowMessageQuestKeys != null)
            {
                foreach(var key in AlreadyShowMessageQuestKeys)
                {
                    key.RandomizeCryptoKey();
                }
            }
        }

        public void OnAfterDeserialize()
        {
            LastUpdateDateNum = mLastUpdateNum;

            if (mQuestKeyList != null && mQuestInfoList != null)
            {
                if (mQuestKeyList.Count == mQuestInfoList.Count)
                {
                    Quests = new Dictionary<ObscuredString, DailyQuestInfo>();

                    for (int i = 0; i < mQuestKeyList.Count; ++i)
                    {
                        var key = mQuestKeyList[i];
                        var value = mQuestInfoList[i];

                        Quests.Add(key, value);
                    }
                }
            }

            if (mAlreadyShowMessageQuestKeys != null)
            {
                AlreadyShowMessageQuestKeys = new List<ObscuredString>();

                foreach(var key in mAlreadyShowMessageQuestKeys)
                {
                    AlreadyShowMessageQuestKeys.Add(key);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            mLastUpdateNum = LastUpdateDateNum;

            if (Quests != null)
            {
                mQuestKeyList = Quests.Select(x => (string)x.Key).ToList();
                mQuestInfoList = Quests.Select(x => x.Value).ToList();
            }

            if (AlreadyShowMessageQuestKeys != null)
            {
                mAlreadyShowMessageQuestKeys = AlreadyShowMessageQuestKeys.Select(x => (string)x).ToList();
            }
        }
    }

    [Serializable]
    public class DailyQuestInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        string mId;
        [SerializeField]
        int mRequireCount;
        [SerializeField]
        bool mIsReceivedReward;

        public ObscuredString Id;
        public ObscuredInt RequireCount;
        public ObscuredBool IsReceivedReward;

        public bool CanReceiveReward => RequireCount >= MaxRequireCount;
        public QuestType Type => Data.questType;
        public int MaxRequireCount => Data.requireCount;
        public DailyQuestData Data => MLand.GameData.DailyQuestData.TryGet(Id);
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

        public void StackRequireCount(int count = 1)
        {
            RequireCount += count;
            RequireCount = Math.Min(RequireCount, MaxRequireCount);
        }
    }
}