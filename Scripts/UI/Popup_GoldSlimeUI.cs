using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace MLand
{
    class Popup_GoldSlimeUI : PopupBase
    {
        double mAmount;
        Action mOnReceive;
        TextMeshProUGUI mTextDesc;
        List<GoldSlime_SlimeCoreItemUI> mSlimeCoreItems;
        public void Init(double amount, Action onReceive)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.GetName(MLand.GameData.GoldSlimeCommonData.id));

            mAmount = amount;
            mOnReceive = onReceive;
            mSlimeCoreItems = new List<GoldSlime_SlimeCoreItemUI>();
            mTextDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            GameObject slimeCoresParent = gameObject.FindGameObject("SlimeCores");
            for(int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;
                GameObject slimeCoreItemObj = slimeCoresParent.FindGameObject($"{type}");
                GoldSlime_SlimeCoreItemUI slimeCoreItem = slimeCoreItemObj.GetOrAddComponent<GoldSlime_SlimeCoreItemUI>();
                slimeCoreItem.Init(type, () => OnSelectSlimeCore(type));

                mSlimeCoreItems.Add(slimeCoreItem);
            }

            SetButtonAction();
            PlayTextDescMotion(StringTableUtil.Get("GoldSlimeMessage_FindMe"));
            PlayGoldSlimeShowingMotion();
        }

        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            base.Close(immediate, hideMotion);

            MLand.CameraManager.ResetFollowInfo();
        }

        void PlayTextDescMotion(string text)
        {
            mTextDesc.DORewind();

            mTextDesc.text = string.Empty;

            mTextDesc.DOText(text, 1f);
        }

        void PlayGoldSlimeShowingMotion()
        {
            // 황금 슬라임 등장 연출!
            Image imgGoldSlime = gameObject.FindComponent<Image>("Image_GoldSlime");

            MonsterLandUtil.PlayUpAppearMotion(imgGoldSlime.rectTransform, -300f);
        }

        void RefreshTextSelect(ElementalType type)
        {
            StringParam param = new StringParam("type", StringTableUtil.GetName($"{type}"));
            param.AddParam("amount", mAmount.ToAlphaString());

            string message = StringTableUtil.Get("GoldSlimeMessage_SelectSlimeCore", param);

            PlayTextDescMotion(message);
        }

        void OnSelectSlimeCore(ElementalType type)
        {
            RefreshTextSelect(type);

            foreach(var item in mSlimeCoreItems)
            {
                if (item.Type == type)
                    item.SetSelected(true);
                else
                    item.SetSelected(false);
            }
        }

        ElementalType GetSelectedType()
        {
            foreach(var item in mSlimeCoreItems)
            {
                if (item.IsSelected)
                    return item.Type;
            }

            return ElementalType.Water;
        }

        void SetButtonAction()
        {
            var buttonNormal = gameObject.FindComponent<Button>("Button_WatchAd");

            buttonNormal.SetButtonAction(ShowAd);
        }
        void ShowAd()
        {
            var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
            if (removeAdProduct != null)
            {
                if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                {
                    OnGiveSlimeCoreReward();
                }
                else
                {
                    ConfirmWatchAd();
                }
            }
            else
            {
                ConfirmWatchAd();
            }

            void ConfirmWatchAd()
            {
                string title = StringTableUtil.Get("Title_Confirm");
                string desc = StringTableUtil.GetDesc("ConfirmGoldSlimeReward");

                MonsterLandUtil.ShowAdConfirmPopup(title, desc, OnGiveSlimeCoreReward);
            }
        }

        void OnGiveSlimeCoreReward()
        {
            ElementalType giveType = GetSelectedType();

            MLand.GameManager.AddSlimeCore(giveType, mAmount);

            double[] slimeCores = new double[(int)ElementalType.Count];

            slimeCores[(int)giveType] = mAmount;

            // 보상 팝업
            var rewardData = new RewardData()
            {
                slimeCores = slimeCores
            };

            MonsterLandUtil.ShowRewardPopup(rewardData);

            mOnReceive?.Invoke();

            // 황금 사냥꾼 업적 확인
            MLand.SavePoint.CheckAchievements(AchievementsType.GoldHunter);

            Close();
        }
    }

    class GoldSlime_SlimeCoreItemUI : MonoBehaviour
    {
        Image mImgSelected;
        bool mIsSelected;
        ElementalType mType;
        public bool IsSelected => mIsSelected;
        public ElementalType Type => mType;
        public void Init(ElementalType type, Action onSelectAction)
        {
            mType = type;
            mImgSelected = gameObject.FindComponent<Image>("Image_Selected");

            SetSelected(false);

            Image imgSlimeCore = gameObject.FindComponent<Image>("Image_SlimeCore");
            imgSlimeCore.sprite = MLand.Atlas.GetCurrencySprite($"SlimeCore_{type}");

            Button button = gameObject.FindComponent<Button>("Button_Select");
            button.SetButtonAction(() =>
            {
                onSelectAction?.Invoke();
            });
        }

        public void SetSelected(bool active)
        {
            mIsSelected = active;

            mImgSelected.gameObject.SetActive(active);
        }
    }
}