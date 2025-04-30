using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace MLand
{
    class Popup_RemoveADS : PopupBase
    {
        public void Init()
        {
            SetUpCloseAction();

            string productId = MLand.GameData.ShopCommonData.removeAdProductId;
            
            var go = gameObject.FindGameObject("RemoveADS_Frame");
            var iapButton = go.GetOrAddComponent<CodelessIAPButton>();

            iapButton.onPurchaseComplete = new CodelessIAPButton.OnPurchaseCompletedEvent();
            iapButton.onPurchaseComplete.AddListener(OnPurchaseSuccess);

            iapButton.onPurchaseFailed = new CodelessIAPButton.OnPurchaseFailedEvent();
            iapButton.onPurchaseFailed.AddListener(OnPurchaseFailed);

            iapButton.consumePurchase = true;

            Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);

            var titleLabels = gameObject.FindGameObject("TitleLabels");
            foreach (var img in titleLabels.GetComponentsInChildren<Image>(true))
            {
                if (img.name == "TitleLabels") continue;

                img.gameObject.SetActive(img.name == $"{MLand.SavePoint.LangCode}");
            }

            var textPrice = gameObject.FindComponent<TextMeshProUGUI>("Text_Price");
            textPrice.text = product?.metadata.localizedPriceString ?? "---";

            var buttonPurchase = gameObject.FindComponent<Button>("Button_Purchase");
            buttonPurchase.SetButtonAction(OnPurchase);

            void OnPurchase()
            {
                CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
            }
        }

        void OnPurchaseSuccess(Product product)
        {
            SoundPlayer.PlayGetGem();

            MLand.GameManager.AddGem(MLand.GameData.ShopCommonData.removeAdPurchaseRewardGem);

            MLand.Lobby.SetActiveRemoveADSButton(false);

            Close();
        }

        void OnPurchaseFailed(Product product, PurchaseFailureDescription failureReason)
        {
            if (product?.Equals(PurchaseFailureReason.UserCancelled) == false)
                MonsterLandUtil.ShowSystemErrorMessage("FailedPurchased");
        }
    }
}