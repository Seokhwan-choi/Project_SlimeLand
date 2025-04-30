using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using DG.Tweening;

namespace MLand
{
    enum CheapShopTab
    {
        None,
        SlimeCore,
        Gold,
        Gem,
        Box,
    }

    class CheapShopTabUIManager
    {
        readonly float[] FrameMovePos = new float[] { -320, -85, 100, 320 };

        Image mImageButtonFrame;
        PopupStatus_CheapShopUI mParent;

        CalculatorUI mCalculator;
        CheapShopTab mCurTab;
        Dictionary<CheapShopTab, Button> mTabButtons;
        Dictionary<CheapShopTab, CheapShopTabUI> mCheapShopTabs;
        public CheapShopTab CurTab => mCurTab;
        public void Init(PopupStatus_CheapShopUI parent)
        {
            mParent = parent;

            GameObject parentObj = parent.gameObject;

            mImageButtonFrame = parentObj.FindComponent<Image>("Image_ButtonFrame");

            GameObject calculatorObj = parentObj.FindGameObject("Calculator");

            mCalculator = calculatorObj.GetOrAddComponent<CalculatorUI>();
            mCalculator.Init();

            mTabButtons = new Dictionary<CheapShopTab, Button>();

            for(int i = 1; i < 5; ++i)
            {
                CheapShopTab tab = (CheapShopTab)i;

                Button button = parentObj.FindComponent<Button>($"TabButton_{tab}");

                button.SetButtonAction(() => ChangeTab(tab));

                mTabButtons.Add(tab, button);
            }

            InitTabList(parentObj);

            mCurTab = CheapShopTab.SlimeCore;

            ShowTab(mCurTab, playSound: false);
        }

        public Button GetTabButton(CheapShopTab tab)
        {
            return mTabButtons.TryGet(tab);
        }

        public void OnUpdate()
        {
            CheapShopTabUI curTab = mCheapShopTabs.TryGet(mCurTab);

            curTab?.OnUpdate();
        }

        public void Localize()
        {
            foreach(var tab in mCheapShopTabs.Values)
            {
                tab.Localize();
            }
        }

        void InitTabList(GameObject parent)
        {
            mCheapShopTabs = new Dictionary<CheapShopTab, CheapShopTabUI>();
            InitTab<CheapShop_SlimeCoreShopUI>(parent, CheapShopTab.SlimeCore);
            InitTab<CheapShop_GoldShopUI>(parent, CheapShopTab.Gold);
            InitTab<CheapShop_GemShopUI>(parent, CheapShopTab.Gem);
            InitTab<CheapShop_BoxShopUI>(parent, CheapShopTab.Box);
        }

        void InitTab<T>(GameObject parent, CheapShopTab tab) where T : CheapShopTabUI
        {
            string name = $"CheapShop_{tab}Shop";

            GameObject cheapShopTabObj = parent.FindGameObject(name);

            T cheapShopTab = cheapShopTabObj.GetOrAddComponent<T>();

            cheapShopTab.Init(this);

            cheapShopTabObj.SetActive(tab == CheapShopTab.SlimeCore);

            mCheapShopTabs.Add(tab, cheapShopTab);
        }

        public void ShowCalculator(UnityAction<double> onEnterButtonAction, Func<double> getNumberMaxValueFunc)
        {
            mCalculator.Show(onEnterButtonAction, getNumberMaxValueFunc);
        }

        public void ChangeTab(CheapShopTab tab)
        {
            if (mCurTab == tab)
                return;

            HideTab(mCurTab);

            mCurTab = tab;

            ShowTab(mCurTab, MLand.Lobby.InTutorial() == false);

            MoveToButtonFrame();
        }

        void MoveToButtonFrame()
        {
            float endValue = FrameMovePos[(int)mCurTab - 1];

            mImageButtonFrame.rectTransform.DOAnchorPosX(endValue, 0.25f)
                .SetAutoKill(false);
        }

        void ShowTab(CheapShopTab tab, bool playSound)
        {
            CheapShopTabUI showTab = mCheapShopTabs.TryGet(tab);

            showTab?.gameObject.SetActive(true);

            showTab?.OnTabEnter();

            mParent.PlaySpeechBallonMotion(tab, playSound);
        }

        void HideTab(CheapShopTab tab)
        {
            CheapShopTabUI hideTab = mCheapShopTabs.TryGet(tab);

            hideTab?.OnTabLeave();

            hideTab?.gameObject.SetActive(false);
        }
    }
}