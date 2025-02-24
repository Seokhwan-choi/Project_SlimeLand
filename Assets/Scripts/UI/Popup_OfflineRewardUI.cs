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
    class Popup_OfflineRewardUI : PopupBase
    {
        Dictionary<ElementalType, OfflineRewardItemUI> mItems;
        public void Init()
        {
            TextMeshProUGUI textSpeech = gameObject.FindComponent<TextMeshProUGUI>("Text_Speech");
            textSpeech.text = string.Empty;
            textSpeech.DOText(StringTableUtil.Get("OfflineReward_KingSlimeSpeech"), 1f);

            SetTitleText(StringTableUtil.Get("Title_OfflineReward"));

            InitTime();
            InitItems();
            InitButtons();
        }

        public override void OnBackButton(bool immediate = false, bool hideMotion = true)
        {

        }

        void InitTime()
        {
            int rewardTimeForMinute = MLand.SavePoint.OfflineRewardManager.CalcRewardTimeForMinute();

            int hour = rewardTimeForMinute / TimeUtil.MinutesInHour;
            rewardTimeForMinute -= hour * TimeUtil.MinutesInHour;

            StringParam param;
            string timeStr;
            TextMeshProUGUI textOfflineTime = gameObject.FindComponent<TextMeshProUGUI>("Text_OfflineTime");

            if (hour > 0)
            {
                param = new StringParam("hour", hour.ToString());
                param.AddParam("minute", rewardTimeForMinute.ToString());

                timeStr = StringTableUtil.Get("OfflineReward_WorkTimeHourAndMinute", param);
            }
            else
            {
                param = new StringParam("minute", rewardTimeForMinute.ToString());

                timeStr = StringTableUtil.Get("OfflineReward_WorkTimeMinute", param);
            }

            textOfflineTime.text = timeStr;

            Image imgMax = gameObject.FindComponent<Image>("Image_Max");

            imgMax.gameObject.SetActive(MLand.SavePoint.OfflineRewardManager.IsMaxReward());
        }

        void InitItems()
        {
            mItems = new Dictionary<ElementalType, OfflineRewardItemUI>();

            int value = MLand.SavePoint.OfflineRewardManager.CalcRewardTimeForMinute();

            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                double typeAmount = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(type, value);

                var go = gameObject.FindGameObject($"{type}");

                OfflineRewardItemUI itemUI = go.GetOrAddComponent<OfflineRewardItemUI>();

                // 실제 방치하는 것과 오프라인의 효율이 완전히 같으면 안된다.
                // 데이터에 정의된 값만큼 수집 양을 조절한다.
                typeAmount = Math.Round(typeAmount * MLand.GameData.OfflineRewardCommonData.defaultValue);

                itemUI.Init(type, typeAmount);

                mItems.Add(type, itemUI);
            }
        }

        void InitButtons()
        {
            var buttonTakeReward = gameObject.FindComponent<Button>("Button_TakeReward");
            buttonTakeReward.SetButtonAction(() => OnTakeReward(false));

            var buttonTakeAdReward = gameObject.FindComponent<Button>("Button_TakeAdReward");
            buttonTakeAdReward.SetButtonAction(() => OnTakeReward(true));
        }

        void OnTakeReward(bool showAd)
        {
            // 혹시 마지막으로 확인 한번 더 하자
            if ( MLand.SavePoint.OfflineRewardManager.HaveRewardToReceive() == false )
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            if (showAd)
            {
                var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
                if (removeAdProduct != null)
                {
                    if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                    {
                        TakeReward(ad: true);
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

                    StringParam param = new StringParam("value", MLand.GameData.OfflineRewardCommonData.adBonusValue.ToString());
                    string desc = StringTableUtil.Get("Confirm_WatchAdAndTakeDoubleOfflineReward", param);

                    MonsterLandUtil.ShowAdConfirmPopup(title, desc, () => TakeReward(ad: true));
                }
                
            }
            else
            {
                TakeReward(ad:false);
            }
        }

        void TakeReward(bool ad)
        {
            List<ItemInfo> itemInfoList = new List<ItemInfo>();

            foreach(var item in mItems.Values)
            {
                if (item.Amount > 0)
                {
                    double amount = ad ? item.Amount * MLand.GameData.OfflineRewardCommonData.adBonusValue : item.Amount;

                    MLand.GameManager.AddSlimeCore(item.Type, amount);

                    ItemInfo itemInfo = new ItemInfo(ItemType.SlimeCore, amount)
                        .SetElementalType(item.Type);

                    itemInfoList.Add(itemInfo);
                }
            }

            MonsterLandUtil.ShowRewardPopup(itemInfoList.ToArray());

            MLand.SavePoint.OfflineRewardManager.TakeReward();

            MLand.SavePoint.Save();

            this.Close();
        }
    }

    class OfflineRewardItemUI : MonoBehaviour
    {
        double mAmount;
        ElementalType mType;
        public double Amount => mAmount;
        public ElementalType Type => mType;
        public void Init(ElementalType type, double amount)
        {
            mType = type;
            mAmount = amount;

            Image imgSlimeCore = gameObject.FindComponent<Image>("Image_SlimeCore");
            imgSlimeCore.sprite = MonsterLandUtil.GetSlimeCoreImg(type);

            TextMeshProUGUI textSlimeCoreAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCoreAmount");
            textSlimeCoreAmount.text = amount.ToAlphaString();
        }
    }
}