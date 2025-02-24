using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MLand
{
    enum SlimeKingTab
    {
        None,
        Slime,
        Building,
    }

    class SlimeKingTabUIManager
    {
        readonly float[] FrameMovePos = new float[]{ 0, 340 };

        Image mImageButtonFrame;
        PopupStatus_SlimeKingUI mParent;

        SlimeKingTab mCurTab;
        Dictionary<SlimeKingTab, SlimeKingTabUI> mSlimeKingTabs;
        public SlimeKingTab CurTab => mCurTab;
        public void Init(PopupStatus_SlimeKingUI parent)
        {
            mParent = parent;

            GameObject parentObj = parent.gameObject;

            mImageButtonFrame = parentObj.FindComponent<Image>("Image_ButtonFrame");

            InitTabs(parentObj);
            InitTabButtonAction(parentObj);
        }

        void InitTabButtonAction(GameObject parent)
        {
            Button tabButton_Slime = parent.FindComponent<Button>("TabButton_Slime");
            Button tabButton_Building = parent.FindComponent<Button>("TabButton_Building");

            tabButton_Slime.SetButtonAction(() => { ChangeTab(SlimeKingTab.Slime); });
            tabButton_Building.SetButtonAction(() => { ChangeTab(SlimeKingTab.Building); });
        }

        void InitTabs(GameObject parent)
        {
            mSlimeKingTabs = new Dictionary<SlimeKingTab, SlimeKingTabUI>();
            InitTab<SlimeKing_SlimeTabUI>(parent, SlimeKingTab.Slime);
            InitTab<SlimeKing_BuildingTabUI>(parent, SlimeKingTab.Building);
        }

        void InitTab<T>(GameObject parent, SlimeKingTab tabType) where T : SlimeKingTabUI
        {
            string name = typeof(T).Name;

            GameObject tabObj = parent.Find(name, true);

            Debug.Assert(tabObj != null, $"PopupStatus_KingSlimeUI의 {name}가 존재하지 않음");

            T tab = tabObj.GetOrAddComponent<T>();

            tab.Init();

            tabObj.SetActive(tabType == SlimeKingTab.Slime);

            mSlimeKingTabs.Add(tabType, tab);
        }

        public SlimeKingTabUI GetTab(SlimeKingTab tab)
        {
            return mSlimeKingTabs.TryGet(tab);
        }

        public bool ChangeTab(SlimeKingTab tab)
        {
            if (mCurTab == tab)
                return false;

            HideTab(mCurTab);

            mCurTab = tab;

            ShowTab(mCurTab, MLand.Lobby.InTutorial() == false);

            MoveToButtonFrame();

            return true;
        }

        public void MoveToTab(string id)
        {
            SlimeKingTabUI showTab = mSlimeKingTabs.TryGet(mCurTab);

            showTab?.MoveToTab(id);
        }

        public void RefreshCurTab()
        {
            SlimeKingTabUI showTab = mSlimeKingTabs.TryGet(mCurTab);

            showTab?.Refresh();
        }

        public void Localize()
        {
            foreach(var tab in mSlimeKingTabs.Values)
            {
                tab.Localize();
            }
        }

        public void ScrollToTarget(string targetId)
        {
            SlimeKingTabUI showTab = mSlimeKingTabs.TryGet(mCurTab);

            showTab?.ScrollToTarget(targetId);
        }

        void MoveToButtonFrame()
        {
            float endValue = FrameMovePos[(int)mCurTab - 1];

            mImageButtonFrame.rectTransform.DOAnchorPosX(endValue, 0.25f)
                .SetAutoKill(false);
        }

        void ShowTab(SlimeKingTab tab, bool playSound)
        {
            SlimeKingTabUI showTab = mSlimeKingTabs.TryGet(mCurTab);

            showTab?.gameObject.SetActive(true);

            showTab?.OnTabEnter();

            mParent.PlaySpeechBallonMotion(tab, playSound);
        }

        void HideTab(SlimeKingTab tab)
        {
            SlimeKingTabUI hideTab = mSlimeKingTabs.TryGet(mCurTab);

            hideTab?.OnTabLeave();

            hideTab?.gameObject.SetActive(false);
        }
    }
}