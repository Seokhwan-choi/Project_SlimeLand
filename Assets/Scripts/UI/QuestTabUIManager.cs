using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace MLand
{
    enum QuestTab
    {
        DailyQuest,
        StepQuest,
    }

    class QuestTabUIManager
    {
        readonly float[] FrameMovePos = new float[] { 0, 340 };

        Image mImageButtonFrame;

        GameObject[] mNewDots;

        QuestTab mCurTab;
        List<QuestTabUI> mQuestTabList;
        public void Init(Popup_QuestUI parent)
        {
            GameObject parentObj = parent.gameObject;

            mImageButtonFrame = parentObj.FindComponent<Image>("Image_ButtonFrame");

            int tabCount = Enum.GetValues(typeof(QuestTab)).Length;
            mNewDots = new GameObject[tabCount];
            for (int i = 0; i < tabCount; ++i)
            {
                QuestTab tab = (QuestTab)i;

                Button button = parentObj.FindComponent<Button>($"TabButton_{tab}");

                button.SetButtonAction(() => ChangeTab(tab));

                mNewDots[i] = parentObj.FindGameObject($"{tab}_NewDot");
            }

            InitTabList(parentObj);

            ShowTab(QuestTab.DailyQuest);

            RefreshNewDot();
        }

        public void OnUpdate()
        {
            QuestTabUI curTab = mQuestTabList[(int)mCurTab];

            curTab?.OnUpdate();
        }

        public void RefreshNewDot()
        {
            bool anyDailyQuestCanReceive = MLand.SavePoint.DailyQuest.AnyQuestCanReceiveReward();
            bool anyStepQuestCanReceive = MLand.SavePoint.StepQuest.AnyQuestCanReceiveReward();

            mNewDots[(int)QuestTab.DailyQuest].SetActive(anyDailyQuestCanReceive);
            mNewDots[(int)QuestTab.StepQuest].SetActive(anyStepQuestCanReceive);

            foreach(var questTab in mQuestTabList)
            {
                questTab.Refresh();
            }
        }

        void InitTabList(GameObject parent)
        {
            mQuestTabList = new List<QuestTabUI>();
            InitTab<Quest_DailyQuestUI>(parent, QuestTab.DailyQuest);
            InitTab<Quest_StepQuestUI>(parent, QuestTab.StepQuest);
        }

        void InitTab<T>(GameObject parent, QuestTab tab) where T : QuestTabUI
        {
            GameObject questTabObj = parent.FindGameObject($"{tab}");

            T questTab = questTabObj.GetOrAddComponent<T>();

            questTab.Init(this);

            questTabObj.SetActive(false);

            mQuestTabList.Add(questTab);
        }

        public void ChangeTab(QuestTab tab)
        {
            if (mCurTab == tab)
                return;

            HideTab(mCurTab);

            mCurTab = tab;

            ShowTab(mCurTab);

            MoveToButtonFrame();
        }

        void MoveToButtonFrame()
        {
            float endValue = FrameMovePos[(int)mCurTab];

            mImageButtonFrame.rectTransform.DOAnchorPosX(endValue, 0.25f)
                .SetAutoKill(false);
        }

        void ShowTab(QuestTab tab)
        {
            QuestTabUI showTab = mQuestTabList[(int)tab];

            showTab.gameObject.SetActive(true);

            showTab.OnTabEnter();
        }

        void HideTab(QuestTab tab)
        {
            QuestTabUI hideTab = mQuestTabList[(int)tab];

            hideTab.OnTabLeave();

            hideTab.gameObject.SetActive(false);
        }
    }
}

