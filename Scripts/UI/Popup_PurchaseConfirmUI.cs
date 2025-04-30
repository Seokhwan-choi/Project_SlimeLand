using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace MLand
{
    class Popup_PurchaseConfirmUI : PopupBase
    {
        GameObject mGoldObj;
        GameObject mGemObj;
        GameObject mBoxObj;
        public void InitGold(string goldShopId, bool watchAd, Action onConfirm)
        {
            Init(onConfirm, watchAd);

            var goldShopData = MLand.GameData.GoldShopData.TryGet(goldShopId);

            // 구매 상품 이미지
            var imgGold = mGoldObj.FindComponent<Image>("Image_Gold");
            imgGold.sprite = MLand.Atlas.GetUISprite(goldShopData.spriteImg);

            // 구매 상품 양
            var slimeCoreDropAmountForMinute = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(goldShopData.goldAmountForMinute);
            double amount = slimeCoreDropAmountForMinute * MLand.GameData.ShopCommonData.slimeCoreDefaultPrice;
            var textGoldAmount = mGoldObj.FindComponent<TextMeshProUGUI>("Text_Amount");
            textGoldAmount.text = amount.ToAlphaString();

            // 구매 확인에 대한 설명
            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            StringParam param = new StringParam("gem", goldShopData.gemPrice.ToString());
            string desc = StringTableUtil.Get("Confirm_PurchaseGold", param);

            textDesc.text = desc;

            mGoldObj.SetActive(true);
        }

        public void InitGem(string gemShopId, bool watchAd, Action onConfirm)
        {
            Init(onConfirm, watchAd);

            var gemShopData = MLand.GameData.GemShopData.TryGet(gemShopId);

            // 구매 상품 이미지
            var imgGem = mGemObj.FindComponent<Image>("Image_Gem");
            imgGem.sprite = MLand.Atlas.GetUISprite(gemShopData.spriteImg);

            var normalPurchaseObj = mGemObj.FindGameObject("NormalPurchase");
            var firstPurchaseObj = mGemObj.FindGameObject("FirstPurchase");

            // 구매 상품 양
            if (gemShopData.onlyOne == false)
            {
                normalPurchaseObj.SetActive(true);
                firstPurchaseObj.SetActive(false);

                var textGemAmount = normalPurchaseObj.FindComponent<TextMeshProUGUI>("Text_Amount");
                textGemAmount.text = gemShopData.gemAmount.ToString();
            }
            else
            {
                normalPurchaseObj.SetActive(false);
                firstPurchaseObj.SetActive(true);

                var textOnlyOne = firstPurchaseObj.FindComponent<TextMeshProUGUI>("Text_OnlyOne");
                textOnlyOne.text = StringTableUtil.Get("UIString_FirstPurchaseBonus");

                var textOrgAmount = firstPurchaseObj.FindComponent<TextMeshProUGUI>("Text_Amount");
                textOrgAmount.text = gemShopData.gemAmount.ToString();

                var textFirstPurchaseAmount = firstPurchaseObj.FindComponent<TextMeshProUGUI>("Text_FirstPurchaseAmount");
                textFirstPurchaseAmount.text = (gemShopData.gemAmount + gemShopData.bonusGemAmount).ToString();
            }

            // 구매 확인에 대한 설명
            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            string desc = watchAd ? 
                StringTableUtil.Get("Confirm_PurchaseGemByAd") :
                StringTableUtil.Get("Confirm_PurchaseGem");

            textDesc.text = desc;

            mGemObj.SetActive(true);
        }

        public void InitBox(string boxShopId, BoxOpenType openType, Action onConfirm)
        {
            Init(onConfirm, openType == BoxOpenType.Ad);

            BoxShopData boxShopData = MLand.GameData.BoxShopData.TryGet(boxShopId);

            // 구매 상품 이미지
            Image imgBox = mBoxObj.FindComponent<Image>("Image_Box");
            imgBox.sprite = MLand.Atlas.GetUISprite(boxShopData.spriteImg);

            string boxId = boxShopData.boxId[(int)openType];
            var boxData = MLand.GameData.BoxData.TryGet(boxId);

            // 구매 상품 양
            var textGoldAmount = mBoxObj.FindComponent<TextMeshProUGUI>("Text_Amount");
            StringParam param = new StringParam("amount", boxData.openCount.ToString());
            textGoldAmount.text = StringTableUtil.Get("UIString_Amount", param);

            
            // 구매 확인에 대한 설명
            TextMeshProUGUI textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            string subDesc = string.Empty;

            if (boxShopData.boxType == BoxType.Costume)
            {
                var minMaxRange = MinMaxRange.Parse(boxData.costumeAmountRange);

                string amountStr = minMaxRange.min == minMaxRange.max ?
                    minMaxRange.min.ToString() : minMaxRange.ToString();

                StringParam param2 = new StringParam("amount", amountStr);
                subDesc = StringTableUtil.Get("UIString_CostumeAmount", param2);
            }

            double gemPrice = boxShopData.gemPrice[(int)openType];
            StringParam param3 = new StringParam("gem", gemPrice.ToString());
            param3.AddParam("subDesc", subDesc);

            string descIndex = openType != BoxOpenType.Ad ? "PurchaseBox" : "PurchaseBoxByAd";

            textDesc.text = StringTableUtil.Get($"Confirm_{descIndex}", param3);

            mBoxObj.SetActive(true);
        }

        void Init(Action onConfirm, bool watchAd)
        {
            mGoldObj = gameObject.FindGameObject("Gold");
            mGemObj = gameObject.FindGameObject("Gem");
            mBoxObj = gameObject.FindGameObject("Box");

            mGoldObj.SetActive(false);
            mGemObj.SetActive(false);
            mBoxObj.SetActive(false);

            string confirm = StringTableUtil.Get("Title_Confirm");

            SetUpCloseAction();
            SetTitleText(confirm);

            var textCancel = gameObject.FindComponent<TextMeshProUGUI>("Text_Cancel");
            textCancel.text = StringTableUtil.Get("UIString_Cancel");
            var textConfirm = gameObject.FindComponent<TextMeshProUGUI>("Text_Confirm");
            textConfirm.text = confirm;
            var textAdConfirm = gameObject.FindComponent<TextMeshProUGUI>("Text_AdConfirm");
            textAdConfirm.text = confirm;

            var buttonCancel = gameObject.FindComponent<Button>("Button_Cancel");
            buttonCancel.SetButtonAction(() => Close());

            var buttonConfirm = gameObject.FindComponent<Button>("Button_Confirm");
            var buttonAdConfirm = gameObject.FindComponent<Button>("Button_AdConfirm");

            buttonConfirm.gameObject.SetActive(!watchAd);
            buttonAdConfirm.gameObject.SetActive(watchAd);

            buttonConfirm.SetButtonAction(OnConfirm);
            buttonAdConfirm.SetButtonAction(OnConfirm);

            void OnConfirm()
            {
                onConfirm?.Invoke();

                Close();
            }
        }
    }
}


