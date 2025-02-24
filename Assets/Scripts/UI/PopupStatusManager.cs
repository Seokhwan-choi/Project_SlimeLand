using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

namespace MLand
{
    public enum PopupStatus
    {
        SlimeKing,      // 슬라임 소환, 건물 건축 버프
        CheapShop,      // 싸다의 상점
        Costume,        // 코스튬

        None,
    }

    class PopupStatusManager
    {
        PopupStatus mCurPopup;
        GameObject mParent;
        GameObject mPopupStatusTouchBlock;
        List<PopupStatusUI> mPopupStatusList;
        public void Init(LobbyUI lobby)
        {
            mCurPopup = PopupStatus.None;
            mParent = lobby.Find("PopupStatusList");
            mPopupStatusTouchBlock = mParent.FindGameObject("PopupStatusTouchBlock");
            mPopupStatusTouchBlock.SetActive(false);
            mPopupStatusList = new List<PopupStatusUI>();
            InitPopup<PopupStatus_SlimeKingUI>(PopupStatus.SlimeKing);
            InitPopup<PopupStatus_CheapShopUI>(PopupStatus.CheapShop);
            InitPopup<PopupStatus_CostumeUI>(PopupStatus.Costume);
        }

        public void OnUpdate()
        {
            foreach(PopupStatusUI popup in mPopupStatusList)
            {
                if (mCurPopup == popup.PopupType)
                {
                    popup.OnUpdate();
                }
            }
        }

        public void InitPopup<T>(PopupStatus type) where T : PopupStatusUI
        {
            string name = $"PopupStatus_{type}";

            GameObject popupObj = mParent.Find(name, true);

            Debug.Assert(popupObj != null, $"{name}의 PopupUI가 없다.");

            T statusPopup = popupObj.GetOrAddComponent<T>();

            statusPopup.Init(type);

            popupObj.SetActive(false);

            mPopupStatusList.Add(statusPopup);
        }

        public T GetPopup<T>() where T : PopupStatusUI
        {
            return mPopupStatusList.FirstOrDefault(x => x is T) as T;
        }

        public void Refresh()
        {
            if (mCurPopup == PopupStatus.None)
                return;

            var showPopup = mPopupStatusList[(int)mCurPopup];

            showPopup.Refresh();
        }
        
        public void Localize()
        {
            foreach(var popupStatus in mPopupStatusList)
            {
                popupStatus.Localize();
            }
        }

        public void ChangeActivePopup(string popupName)
        {
            if (Enum.TryParse(popupName, out PopupStatus result))
            {
                ChangeActivePopup(result);
            }
        }

        public void HideCurPopup()
        {
            HidePopup(mCurPopup);

            mCurPopup = PopupStatus.None;

            mPopupStatusTouchBlock.SetActive(false);
        }

        public bool ChangeActivePopup(PopupStatus popup, int subIdx = 1)
        {
            if (mCurPopup == popup)
                return false;

            HidePopup(mCurPopup);

            mCurPopup = popup;

            ShowPopup(mCurPopup, subIdx);

            mPopupStatusTouchBlock.SetActive(true);

            return true;
        }

        void ShowPopup(PopupStatus popup, int subIdx)
        {
            if (popup == PopupStatus.None)
                return;

            PopupStatusUI showPopup = mPopupStatusList[(int)popup];

            showPopup.OnEnter(subIdx);

            showPopup.PlayShowMotion();
        }

        void HidePopup(PopupStatus popup)
        {
            if (popup == PopupStatus.None)
                return;

            PopupStatusUI hidePopup = mPopupStatusList[(int)popup];

            hidePopup.OnLeave();

            hidePopup.PlayHideMotion();
        }
    }
}