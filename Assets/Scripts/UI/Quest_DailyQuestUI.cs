using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MLand
{
    class Quest_DailyQuestUI : QuestTabUI
    {
        float mTimeInterval;
        TextMeshProUGUI mTextRemainTime;
        DailyQuest DailyQuest => MLand.SavePoint.DailyQuest;
        public override void Init(QuestTabUIManager parent)
        {
            base.Init(parent);

            mTextRemainTime = gameObject.FindComponent<TextMeshProUGUI>("Text_UpdateRemainTime");

            DailyQuest.Update();

            Refresh();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            mTimeInterval -= Time.deltaTime;
            if (mTimeInterval <= 0)
            {
                mTimeInterval = 1f;

                RefreshRemainTimeText();

                if (DailyQuest.Update())
                {
                    MLand.Lobby.RefreshNewDot();

                    MLand.SavePoint.Save();
                }
            }
        }

        void RefreshRemainTimeText()
        {
            var remainTime = TimeUtil.RemainSecondsToNextDay();
            var remainTimeStr = TimeUtil.GetTimeStr(remainTime);
            var param = new StringParam("time", remainTimeStr);
            var message = StringTableUtil.Get("Quest_UpdateRemainTime", param);

            mTextRemainTime.text = message;
        }

        public override void Refresh()
        {
            int index = 0;

            foreach (DailyQuestInfo questInfo in DailyQuest.Quests.Values)
            {
                if (questInfo.Type != QuestType.DailyquestClear)
                {
                    GameObject questObj = gameObject.FindGameObject($"Quest_{index + 1}");

                    DailyQuestItemUI questItem = questObj.GetOrAddComponent<DailyQuestItemUI>();

                    questItem.Init(this, questInfo);
                }
                else
                {
                    GameObject questAllClearObj = gameObject.FindGameObject("QuestAllClear");

                    DailyQuestItemUI questItem = questAllClearObj.GetOrAddComponent<DailyQuestItemUI>();

                    questItem.Init(this, questInfo);
                }

                index++;
            }
        }
    }
}