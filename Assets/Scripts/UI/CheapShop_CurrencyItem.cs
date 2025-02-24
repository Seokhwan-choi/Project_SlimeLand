using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MLand
{
    class CheapShop_CurrencyItem : MonoBehaviour
    {
        protected TextMeshProUGUI mText_Amount;
        protected TextMeshProUGUI mText_Price;
        protected Button mButton_Buy;
        protected Image mImg_Item;
        public virtual void Init(string id) 
        {
            mText_Amount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");
            mText_Price = gameObject.FindComponent<TextMeshProUGUI>("Text_Price");

            mButton_Buy = gameObject.FindComponent<Button>("Button_Buy");
            mButton_Buy.SetButtonAction(OnBuyButton);

            mImg_Item = gameObject.FindComponent<Image>("Image_Item");
        }
        public virtual void OnBuyButton() { }
        public virtual void OnUpdate(float dt) { }
        public virtual void Localize() { }
        public void SetTextAmount(double amount, bool toAlphaStr = false)
        {
            mText_Amount.text = toAlphaStr ? amount.ToAlphaString() : amount.ToString();
        }
        public void SetTextPrice(string price)
        {
            mText_Price.text = price;
        }

        public void SetImgItem(Sprite itemImg)
        {
            mImg_Item.sprite = itemImg;
        }
    }
}