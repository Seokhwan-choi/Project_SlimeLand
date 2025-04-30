using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Quest_StepQuestUI : QuestTabUI
    {
        Dictionary<string, StepQuestItemUI> mItemDics;
        StepQuest StepQuest => MLand.SavePoint.StepQuest;
        public override void Init(QuestTabUIManager parent)
        {
            base.Init(parent);

            RefreshStepQuestList();
        }

        public override void Refresh()
        {
            base.Refresh();

            foreach(var item in mItemDics.Values)
            {
                item.Refresh();
            }
        }

        void RefreshStepQuestList()
        {
            GameObject content = gameObject.FindGameObject("Content");

            content.AllChildObjectOff();

            mItemDics = new Dictionary<string, StepQuestItemUI>();

            foreach (StepQuestInfo info in StepQuest.StepQuestInfoDics.Values)
            {
                GameObject itemObj = Util.InstantiateUI("StepQuestItem", content.transform);

                itemObj.Localize();

                StepQuestItemUI itemUI = itemObj.GetOrAddComponent<StepQuestItemUI>();

                itemUI.Init(info);

                mItemDics.Add(info.Id, itemUI);
            }
        }
    }
}