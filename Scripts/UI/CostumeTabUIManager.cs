using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace MLand
{
    enum CostumeTab
    {
        None,
        Face,
        Body,
        Acc,       // ¾Ç¼¼»ç¸® (Accessories)
    }

    class CostumeTabUIManager
    {
        readonly float[] FrameMovePos = new float[] { -220, 0, 220 };

        Image mImageButtonFrame;

        CostumeTab mCurTab;
        Dictionary<CostumeTab, Button> mTabButtons;
        Dictionary<CostumeTab, CostumeTabUI> mCostumeTabs;

        PopupStatus_CostumeUI mParent;
        public string CurSlimeId => mParent.CurSlimeId;
        public void Init(PopupStatus_CostumeUI parent)
        {
            mParent = parent;

            GameObject parentObj = parent.gameObject;

            mImageButtonFrame = parentObj.FindComponent<Image>("Image_ButtonFrame");

            mTabButtons = new Dictionary<CostumeTab, Button>();

            for (int i = 1; i < 4; ++i)
            {
                CostumeTab tab = (CostumeTab)i;

                Button button = parentObj.FindComponent<Button>($"TabButton_{tab}");

                button.SetButtonAction(() => ChangeTab(tab));

                TextMeshProUGUI textTabButton = button.gameObject.FindComponent<TextMeshProUGUI>($"Text_{tab}");
                textTabButton.text = StringTableUtil.Get($"UIString_{tab}");

                mTabButtons.Add(tab, button);
            }

            InitTabList(parentObj);

            mCurTab = CostumeTab.Face;

            ShowTab(mCurTab);
        }

        public void OnUpdate()
        {
            CostumeTabUI curTab = mCostumeTabs.TryGet(mCurTab);

            curTab?.OnUpdate();
        }

        public void Localize()
        {
            foreach(var tab in mCostumeTabs.Values)
            {
                tab.Localize();
            }
        }

        void InitTabList(GameObject parent)
        {
            mCostumeTabs = new Dictionary<CostumeTab, CostumeTabUI>();
            InitTab<Costume_FaceUI>(parent, CostumeTab.Face);
            InitTab<Costume_BodyUI>(parent, CostumeTab.Body);
            InitTab<Costume_AccUI>(parent, CostumeTab.Acc);
        }

        void InitTab<T>(GameObject parent, CostumeTab tab) where T : CostumeTabUI
        {
            string name = $"Costume_{tab}";

            GameObject costumeTabObj = parent.FindGameObject(name);

            T costumeTab = costumeTabObj.GetOrAddComponent<T>();

            costumeTab.Init(this);

            costumeTabObj.SetActive(tab == CostumeTab.Face);

            mCostumeTabs.Add(tab, costumeTab);
        }

        public CostumeTabUI GetCurTab()
        {
            return mCostumeTabs.TryGet(mCurTab);
        }

        public void ChangeTab(CostumeTab tab)
        {
            if (mCurTab == tab)
            {
                GetCurTab()?.OnTabEnter();
                return;
            }

            HideTab(mCurTab);

            mCurTab = tab;

            ShowTab(mCurTab);

            MoveToButtonFrame();
        }

        void MoveToButtonFrame()
        {
            float endValue = FrameMovePos[(int)mCurTab - 1];

            mImageButtonFrame.rectTransform.DOAnchorPosX(endValue, 0.25f)
                .SetAutoKill(false);
        }

        void ShowTab(CostumeTab tab)
        {
            CostumeTabUI showTab = mCostumeTabs.TryGet(tab);

            showTab?.gameObject.SetActive(true);

            showTab?.OnTabEnter();
        }

        void HideTab(CostumeTab tab)
        {
            CostumeTabUI hideTab = mCostumeTabs.TryGet(tab);

            hideTab?.OnTabLeave();

            hideTab?.gameObject.SetActive(false);
        }

        public void OnChangeCostume()
        {
            GetCurTab()?.OnTabEnter();

            mParent.RefreshSlimeCostumes();
        }
    }
}