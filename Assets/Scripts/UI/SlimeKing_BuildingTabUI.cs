using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class SlimeKing_BuildingTabUI : SlimeKingTabUI
    {
        ScrollToTarget mScrollToTarget;
        BuildingTabUIManager mTabUIManager;
        public override void Init()
        {
            mTabUIManager = new BuildingTabUIManager();
            mTabUIManager.Init(gameObject);
        }

        public override void OnTabEnter()
        {
            Refresh();
        }

        public override void Refresh()
        {
            mTabUIManager.Refresh();
        }

        public override void Localize()
        {
            mTabUIManager.Localize();
        }

        public override void MoveToTab(string id)
        {
            var buildingData = MLand.GameData.BuildingData.TryGet(id);

            mTabUIManager.ChangeTab(buildingData?.elementalType ?? ElementalType.Water);
        }

        public override void ScrollToTarget(string id)
        {
            //BuildingData buildingData = MLand.GameData.BuildingData.TryGet(id);
            //if (buildingData == null)
            //    return;

            //SlimeKing_BuildingTab_ElementListUI list = mTabUIManager.GetElementList(buildingData.elementalType);
            //if (list == null)
            //    return;

            //var tm = list.GetComponent<RectTransform>();
            //if (tm == null)
            //    return;

            //mScrollToTarget.Scroll(tm);
        }

        public SlimeKing_BuildingTab_ElementListUI GetElementList(ElementalType type)
        {
            return mTabUIManager.GetElementList(type);
        }
    }

    class BuildingTabUIManager
    {
        readonly float[] FrameMovePos = new float[] { -320, -85, 100, 320 };

        ElementalType mCurTab;
        Image mImageButtonFrame;
        Dictionary<ElementalType, Button> mTabButtons;
        Dictionary<ElementalType, SlimeKing_BuildingTab_ElementListUI> mBuildingTabDics;
        public void Init(GameObject go)
        {
            mTabButtons = new Dictionary<ElementalType, Button>();
            mBuildingTabDics = new Dictionary<ElementalType, SlimeKing_BuildingTab_ElementListUI>();
            mImageButtonFrame = go.FindComponent<Image>("Image_ButtonFrame");
            mCurTab = ElementalType.Water;

            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                GameObject building = go.FindGameObject($"{type}BuildingTabUI");
                var elementListUI = building.GetOrAddComponent<SlimeKing_BuildingTab_ElementListUI>();
                elementListUI.Init(type);
                elementListUI.SetActive(mCurTab == type);

                Button button = go.FindComponent<Button>($"TabButton_{type}");

                button.SetButtonAction(() => ChangeTab(type));

                mTabButtons.Add(type, button);
                mBuildingTabDics.Add(type, elementListUI);
            }
        }

        public void Refresh()
        {
            foreach (var elementListUI in mBuildingTabDics.Values)
            {
                elementListUI.Refresh();
            }
        }

        public void Localize()
        {
            foreach (var elementListUI in mBuildingTabDics.Values)
            {
                elementListUI.Localize();
            }
        }

        public SlimeKing_BuildingTab_ElementListUI GetElementList(ElementalType type)
        {
            return mBuildingTabDics.TryGet(type);
        }

        public void ChangeTab(ElementalType tab)
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

        void ShowTab(ElementalType tab)
        {
            SlimeKing_BuildingTab_ElementListUI showTab = mBuildingTabDics.TryGet(tab);

            showTab.SetActive(true);

            showTab.Refresh();
        }

        void HideTab(ElementalType tab)
        {
            SlimeKing_BuildingTab_ElementListUI hideTab = mBuildingTabDics.TryGet(tab);

            hideTab.SetActive(false);
        }
    }
}