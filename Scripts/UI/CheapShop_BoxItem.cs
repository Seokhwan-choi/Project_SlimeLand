using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;
using System.Linq;


namespace MLand
{
    class CheapShop_BoxItem : MonoBehaviour
    {
        BoxShopData mShopData;

        Button mButtonWatchAd;
        Button mButtonOneOpen;
        Button mButtonTenOpen;
        TextMeshProUGUI mTextWatchAdRemainTime;
        TextMeshProUGUI mTextRemainWathAdCount;
        TextMeshProUGUI mTextOneOpenPrice;
        TextMeshProUGUI mTextTenOpenPrice;

        TextMeshProUGUI mTextBoxName;
        TextMeshProUGUI mTextOneOpen;
        TextMeshProUGUI mTextOneOpen2;
        TextMeshProUGUI mTextTenOpen;
        public void Init(BoxShopData boxShopData)
        {
            mShopData = boxShopData;

            // 박스 이미지
            Image imageBox = gameObject.FindComponent<Image>("Image_Box");
            imageBox.sprite = MLand.Atlas.GetUISprite(mShopData.spriteImg);

            // 박스 이름
            mTextBoxName = gameObject.FindComponent<TextMeshProUGUI>("Text_BoxName");
            mTextBoxName.text = StringTableUtil.GetName($"{mShopData.boxType}Box");

            // 박스 확률
            string boxId = mShopData.boxId[(int)BoxOpenType.One];
            Button buttonProbability = gameObject.FindComponent<Button>("Button_ProbabilityTable");
            buttonProbability.SetButtonAction(() => MonsterLandUtil.ShowBoxProbabilityTable(boxId));

            InitWatchAd();
            InitOneOpen();
            InitTenOpen();

            RefreshText();
            RefreshWatchAd();
            RefreshBuyPriceText();
        }

        void InitWatchAd()
        {
            GameObject watchAdObj = gameObject.FindGameObject("Open_WatchAd");
            mTextWatchAdRemainTime = watchAdObj.FindComponent<TextMeshProUGUI>("Text_WatchAdRemainTime");

            mTextOneOpen2 = watchAdObj.FindComponent<TextMeshProUGUI>("Text_Open");

            mTextRemainWathAdCount = watchAdObj.FindComponent<TextMeshProUGUI>("Text_WatchAdRemainCount");

            mButtonWatchAd = watchAdObj.FindComponent<Button>("Button_WatchAd");
            mButtonWatchAd.SetButtonAction(() => OnBuyButtonAction(BoxOpenType.Ad));
        }

        void InitOneOpen()
        {
            var oneOpenObj = gameObject.FindGameObject("One_Open");

            mTextOneOpen = oneOpenObj.FindComponent<TextMeshProUGUI>("Text_Open");
            

            mTextOneOpenPrice = oneOpenObj.FindComponent<TextMeshProUGUI>("Text_Price");

            mButtonOneOpen = oneOpenObj.FindComponent<Button>("Button_One_Open");
            mButtonOneOpen.SetButtonAction(() => OnBuyButtonAction(BoxOpenType.One));
        }

        void InitTenOpen()
        {
            var tenOpenObj = gameObject.FindGameObject("Ten_Open");

            mTextTenOpen = tenOpenObj.FindComponent<TextMeshProUGUI>("Text_Open");
            
            mTextTenOpenPrice = tenOpenObj.FindComponent<TextMeshProUGUI>("Text_Price");

            mButtonTenOpen = tenOpenObj.FindComponent<Button>("Button_Ten_Open");
            mButtonTenOpen.SetButtonAction(() => OnBuyButtonAction(BoxOpenType.Ten));
        }

        float mIntervalTime;
        public void OnUpdate(float dt)
        {
            mIntervalTime -= dt;
            if (mIntervalTime <= 0f)
            {
                mIntervalTime = 1f;

                RefreshWatchAd();
                RefreshBuyPriceText();
            }
        }

        public void Localize()
        {
            mTextBoxName.text = StringTableUtil.GetName($"{mShopData.boxType}Box");
            RefreshText();
            RefreshWatchAd();
            RefreshBuyPriceText();
        }

        public void RefreshText()
        {
            StringParam param = new StringParam("count", 1.ToString());

            string oneOpen = StringTableUtil.Get("UIString_BoxOpen", param);
            mTextOneOpen.text = oneOpen;
            mTextOneOpen2.text = oneOpen;

            StringParam param2 = new StringParam("count", 10.ToString());
            mTextTenOpen.text = StringTableUtil.Get("UIString_BoxOpen", param2);
        }

        void RefreshWatchAd()
        {
            if (MLand.SavePoint.CanWatchAdBoxItem(mShopData.id))
            {
                if (mTextRemainWathAdCount.gameObject.activeSelf)
                    mTextWatchAdRemainTime.gameObject.SetActive(false);

                var imgButton = mButtonWatchAd.GetComponent<Image>();
                imgButton.sprite = MLand.Atlas.GetUISprite("Btn_Square_Red");
            }
            else
            {
                mTextWatchAdRemainTime.gameObject.SetActive(true);

                int remainToNextDay = TimeUtil.RemainSecondsToNextDay();
                string timeStr = TimeUtil.GetTimeStr(remainToNextDay);
                StringParam param = new StringParam("time", timeStr);

                var text = StringTableUtil.Get("Shop_FreeOpenRemainTime", param);

                mTextWatchAdRemainTime.text = text;

                var imgButton = mButtonWatchAd.GetComponent<Image>();
                imgButton.sprite = MLand.Atlas.GetUISprite("Btn_Square_LightGray");
            }

            int maxCount = mShopData.watchAdDailyCount;
            int remainCount = MLand.SavePoint.Shop.GetRemainWatchAdCount(mShopData.id);

            mTextRemainWathAdCount.text = $"({remainCount}/{maxCount})";
        }

        void RefreshBuyPriceText()
        {
            double oneOpenGemPrice = mShopData.gemPrice[(int)BoxOpenType.One];
            bool isEnoughGem = MLand.SavePoint.IsEnoughGem(oneOpenGemPrice);

            mTextOneOpenPrice.text = isEnoughGem ? $"{oneOpenGemPrice}" : $"<color=red>{oneOpenGemPrice}</color>";

            double tenOpenGemPrice = mShopData.gemPrice[(int)BoxOpenType.Ten];
            isEnoughGem = MLand.SavePoint.IsEnoughGem(tenOpenGemPrice);

            mTextTenOpenPrice.text = isEnoughGem ? $"{tenOpenGemPrice}" : $"<color=red>{tenOpenGemPrice}</color>";
        }

        void OnBuyButtonAction(BoxOpenType openType)
        {
            if (openType == BoxOpenType.Ad)
            {
                if (MLand.SavePoint.CanWatchAdBoxItem(mShopData.id) == false)
                {
                    int remainToNextDay = TimeUtil.RemainSecondsToNextDay();
                    string timeStr = TimeUtil.GetTimeStr(remainToNextDay);
                    StringParam param = new StringParam("time", timeStr);

                    MonsterLandUtil.ShowSystemMessage(StringTableUtil.Get("Shop_FreeOpenRemainTime", param));

                    SoundPlayer.PlayErrorSound();

                    return;
                }

                var popup = MLand.PopupManager.CreatePopup<Popup_PurchaseConfirmUI>();

                popup.InitBox(mShopData.id, openType, () =>
                {
                    var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
                    if (removeAdProduct != null)
                    {
                        if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                        {
                            BuyBox();
                        }
                        else
                        {
                            MonsterLandUtil.ShowRewardedAd(BuyBox);
                        }
                    }
                    else
                    {
                        MonsterLandUtil.ShowRewardedAd(BuyBox);
                    }
                });
            }
            else
            {
                double gemPrice = mShopData.gemPrice[(int)openType];

                if (MLand.SavePoint.IsEnoughGem(gemPrice) == false)
                {
                    MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGem");

                    return;
                }

                var popup = MLand.PopupManager.CreatePopup<Popup_PurchaseConfirmUI>();

                popup.InitBox(mShopData.id, openType, BuyBox);
            }

            void BuyBox()
            {
                BuyBoxUtil.BuyBox(mShopData, openType);
            }
        }
    }
}

