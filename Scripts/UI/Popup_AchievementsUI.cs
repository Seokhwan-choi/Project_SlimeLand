using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MLand
{
    class Popup_AchievementsUI : PopupBase
    {
        Dictionary<string, AchievementsItemUI> mItemDics;
        public void Init()
        {
            SetTitleText(StringTableUtil.Get("Title_Achievements"));
            SetUpCloseAction();

            Button buttonInfo = gameObject.FindComponent<Button>("Button_Info");
            buttonInfo.SetButtonAction(ShowInfoPopup);

            var list = gameObject.FindGameObject("List");

            mItemDics = new Dictionary<string, AchievementsItemUI>();

            foreach(var id in MLand.SavePoint.Achievements.AchievementsDics.Keys)
            {
                GameObject itemObj = Util.InstantiateUI("AchievementsItemUI", list.transform);

                itemObj.Localize();

                AchievementsItemUI item = itemObj.GetOrAddComponent<AchievementsItemUI>();

                item.Init(this, id);

                mItemDics.Add(id, item);
            }

            RefreshNewDot();

            if (SavePointBitFlags.AchievementsHelpShow.IsOff())
            {
                SavePointBitFlags.AchievementsHelpShow.Set(true);

                ShowInfoPopup();

                MLand.Lobby.RefreshNewDot();
            }
        }

        public void RefreshNewDot()
        {
            foreach(var item in mItemDics.Values)
            {
                item.RefreshObj();
            }
        }

        void ShowInfoPopup()
        {
            var popup = MLand.PopupManager.CreatePopup<Popup_AchievementsInfoUI>();

            popup.Init();
        }
    }

    class AchievementsItemUI : MonoBehaviour
    {
        GameObject mNewDotObj;
        GameObject mActiveObj;
        GameObject mInActiveObj;

        string mId;
        public string Id => mId;
        public void Init(Popup_AchievementsUI parent, string id)
        {
            mActiveObj = gameObject.FindGameObject("Active");
            mInActiveObj = gameObject.FindGameObject("InActive");
            mNewDotObj = mActiveObj.FindGameObject("NewDot");

            mId = id;

            RefreshObj();

            AchievementsData data = MLand.GameData.AchievementsData.TryGet(id);

            Button buttonShowDetail = gameObject.FindComponent<Button>("Button_ShowDetail");
            buttonShowDetail.SetButtonAction(() => ShowDetail(parent.RefreshNewDot));

            Sprite icon = MLand.Atlas.GetUISprite(data.spriteImg);

            Image buttonImg = buttonShowDetail.GetComponent<Image>();
            buttonImg.sprite = icon;

            Image imgMask = gameObject.FindComponent<Image>("Mask");
            imgMask.sprite = icon;
        }

        public void RefreshObj()
        {
            AchievementsInfo info = MLand.SavePoint.Achievements.GetAchievementsInfo(mId);

            mActiveObj.SetActive(info.CanReceiveReward);
            mInActiveObj.SetActive(!info.CanReceiveReward);
            mNewDotObj.SetActive(info.CanReceiveReward && info.IsReceivedReward == false);
        }

        void ShowDetail(Action onTakeReward)
        {
            var popup = MLand.PopupManager.CreatePopup<Popup_AchievementsDescriptionUI>();

            popup.Init(mId, onTakeReward);
        }
    }
}