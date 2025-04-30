using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class CheapShop_GemItem : CheapShop_CurrencyItem
    {
        GemShopData mData;
        GameObject mFirstPurchaseObj;
        GameObject mNormalObj; 
        GameObject mWatchAdObj;
        Image mImageButtonWatchAd;
        TextMeshProUGUI mTextFirstPurchaseAmount;
        TextMeshProUGUI mTextWatchAdRemainTime;
        CodelessIAPButton mIAPButton;
        bool IsWatchAdItem => mData.watchAdDailyCount > 0;
        public override void Init(string id)
        {
            base.Init(id);

            mFirstPurchaseObj = gameObject.FindGameObject("FirstPurchase");
            mNormalObj = gameObject.FindGameObject("Normal");
            mWatchAdObj = gameObject.FindGameObject("WatchAd");
            
            mTextFirstPurchaseAmount = mFirstPurchaseObj.FindComponent<TextMeshProUGUI>("Text_FirstPurchaseAmount");
            mTextWatchAdRemainTime = mWatchAdObj.FindComponent<TextMeshProUGUI>("Text_WatchAdRemainTime");
            var buttonWatchAd = mWatchAdObj.FindComponent<Button>("Button_WatchAd");
            buttonWatchAd.SetButtonAction(OnWatchAdButton);

            mImageButtonWatchAd = mWatchAdObj.FindComponent<Image>("Image_ButtonWatchAd");

            mData = MLand.GameData.GemShopData.TryGet(id);

            mNormalObj.SetActive(!IsWatchAdItem);
            mWatchAdObj.SetActive(IsWatchAdItem);

            RefreshProduct();
        }

        public override void Localize()
        {
            RefreshProduct();
        }

        void RefreshIAPButton()
        {
            if (mData.productId.IsValid())
            {
                mIAPButton = gameObject.GetOrAddComponent<CodelessIAPButton>();
                mIAPButton.productId = mData.productId;

                mIAPButton.onPurchaseComplete = new CodelessIAPButton.OnPurchaseCompletedEvent();
                mIAPButton.onPurchaseComplete.AddListener(OnPurchaseSuccess);

                mIAPButton.onPurchaseFailed = new CodelessIAPButton.OnPurchaseFailedEvent();
                mIAPButton.onPurchaseFailed.AddListener(OnPurchaseFailed);

                mIAPButton.consumePurchase = true;
            }
        }

        void OnPurchaseSuccess(Product product)
        {
            SoundPlayer.PlayGetGem();

            MLand.GameManager.AddGem(mData.gemAmount + mData.bonusGemAmount);

            RefreshProduct();
        }

        void OnPurchaseFailed(Product product, PurchaseFailureDescription failureReason)
        {
            if (product?.Equals(PurchaseFailureReason.UserCancelled) == false)
                MonsterLandUtil.ShowSystemErrorMessage("FailedPurchased");
        }

        Product GetProduct()
        {
            if (mData.productId.IsValid())
            {
                return CodelessIAPStoreListener.Instance.GetProduct(mData.productId);
            }

            return null;
        }

        string GetProductPrice()
        {
            var product = GetProduct();
            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }
            else
            {
                return "---";
            }
        }

        float mInterval;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mInterval -= dt;
            if (mInterval <= 0f)
            {
                mInterval = 1f;

                RefreshWatchAd();
                RefreshProduct();
            }
        }

        public override void OnBuyButton()
        {
            var popup = MLand.PopupManager.CreatePopup<Popup_PurchaseConfirmUI>();

            popup.InitGem(mData.id, false, () => CodelessIAPStoreListener.Instance.InitiatePurchase(mData.productId));
        }

        void SetTextFirstPurchaseAmount()
        {
            if (mData.onlyOne)
            {
                mTextFirstPurchaseAmount.text = $"{mData.gemAmount + mData.bonusGemAmount}";
            }
        }

        void RefreshProduct()
        {
            RefreshFirstPurchase();
            RefreshIAPButton();

            SetTextAmount(mData.gemAmount);
            SetTextPrice(GetProductPrice());
            SetTextFirstPurchaseAmount();
            SetImgItem(MLand.Atlas.GetUISprite(mData.spriteImg));

            mFirstPurchaseObj.SetActive(mData.onlyOne);
        }

        void RefreshFirstPurchase()
        {
            // 첫 구매 상품을 구매완료 후, 더 이상 구매할 수 없는 상태가 되었다면
            // 다시 일반 상품을 보여주도록하자
            if (mData.productId.IsValid())
            {
                var product = GetProduct();
                if (product != null)
                {
                    if (product.definition.type == ProductType.NonConsumable && product.hasReceipt)
                    {
                        int slot = mData.slot;

                        IEnumerable<GemShopData> datas = MLand.GameData.GemShopData.Values.Where(x => x.slot == slot);

                        string prevId = mData.id;

                        mData = datas.Where(x => x.id != prevId).FirstOrDefault();
                    }
                }
            }
        }

        void OnWatchAdButton()
        {
            if (IsWatchAdItem == false)
            {
                SoundPlayer.PlayErrorSound();

                return;
            }

            if (MLand.SavePoint.CanWatchAdGemItem(mData.id) == false)
            {
                int remainToNextDay = TimeUtil.RemainSecondsToNextDay();
                string timeStr = TimeUtil.GetTimeStr(remainToNextDay);
                StringParam param = new StringParam("time", timeStr);

                MonsterLandUtil.ShowSystemMessage(StringTableUtil.Get("Shop_FreeBuyRemainTime", param));

                SoundPlayer.PlayErrorSound();

                return;
            }

            var popup = MLand.PopupManager.CreatePopup<Popup_PurchaseConfirmUI>();

            popup.InitGem(mData.id, true, () =>
            {
                var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
                if (removeAdProduct != null)
                {
                    if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                    {
                        StackWatchAd();
                    }
                    else
                    {
                        MonsterLandUtil.ShowRewardedAd(StackWatchAd);
                    }
                }
                else
                {
                    MonsterLandUtil.ShowRewardedAd(StackWatchAd);
                }
            });

            void StackWatchAd()
            {
                if (MLand.SavePoint.StackWatchAdGemItem(mData.id))
                {
                    MLand.GameManager.AddGem(mData.gemAmount + mData.bonusGemAmount);

                    MLand.SavePoint.CheckQuests(QuestType.BuyAdGem);
                }
            }
        }

        void RefreshWatchAd()
        {
            if (IsWatchAdItem == false)
                return;

            bool canWatchAd = MLand.SavePoint.CanWatchAdGemItem(mData.id);

            string buttonName = canWatchAd ? "Btn_Square_Red" : "Btn_Square_LightGray";

            mImageButtonWatchAd.sprite = MLand.Atlas.GetUISprite(buttonName);

            mTextWatchAdRemainTime.gameObject.SetActive(!canWatchAd);
            if (canWatchAd == false)
            {
                int remainToNextDay = TimeUtil.RemainSecondsToNextDay();
                string timeStr = TimeUtil.GetTimeStr(remainToNextDay);
                StringParam param = new StringParam("time", timeStr);

                mTextWatchAdRemainTime.text = StringTableUtil.Get("Shop_FreeBuyRemainTime", param);
            }
        }

    }
}