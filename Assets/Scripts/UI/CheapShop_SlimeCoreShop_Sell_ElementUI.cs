using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MLand
{
    class CheapShop_SlimeCoreShop_Sell_ElementUI : MonoBehaviour
    {
        ElementalType mType;
        double mSlimeCorePrice;
        double mTotalSlimeCoreAmount;

        Action mOnDecideAmount;

        TextMeshProUGUI mText_SlimeCorePrice;   // 슬라임 똥의 현재 가격
        TextMeshProUGUI mText_TotalSlimeCore;   // 판매하려는 슬라임 똥의 양
        TextMeshProUGUI mText_TotalGold;        // 판매하려는 슬라임 똥의 총 가격

        Shop Shop => MLand.SavePoint.Shop;
        public void Init(CheapShop_SlimeCoreShopUI parent, ElementalType type, Action onDecideAmount)
        {
            mType = type;
            mSlimeCorePrice = Shop.GetSlimeCorePrice(type);
            mTotalSlimeCoreAmount = 0;
            mOnDecideAmount = onDecideAmount;

            InitButtonActions(parent);
            InitElementTypeImg();

            mText_SlimeCorePrice = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCorePrice");
            mText_TotalSlimeCore = gameObject.FindComponent<TextMeshProUGUI>("Text_TotalSlimeCore");
            mText_TotalGold = gameObject.FindComponent<TextMeshProUGUI>("Text_TotalGold");
        }

        public void OnTabLeave()
        {
            OnReset();
        }

        public void OnReset()
        {
            mSlimeCorePrice = Shop.GetSlimeCorePrice(mType);

            SetTotalSlimeCoreAmount(0);
        }

        public void OnUpdate()
        {
            double newSlimeCorePrice = Shop.GetSlimeCorePrice(mType);
            if (mSlimeCorePrice != newSlimeCorePrice)
            {
                mSlimeCorePrice = newSlimeCorePrice;

                Refresh();
            }
        }

        public void Refresh()
        {
            RefreshSlimeCorePriceText();
            RefreshTotalSlimeCoreAmountText();
            RefreshTotalGoldText();
        }

        public void Localize()
        {
            Refresh();
        }

        void RefreshSlimeCorePriceText()
        {
            mText_SlimeCorePrice.text = mSlimeCorePrice.ToAlphaString();
        }

        void RefreshTotalSlimeCoreAmountText()
        {
            mText_TotalSlimeCore.text = mTotalSlimeCoreAmount.ToAlphaString();
        }

        void RefreshTotalGoldText()
        {
            mText_TotalGold.text = GetTotalPriceGold().ToAlphaString();
        }

        void InitElementTypeImg()
        {
            Image bigImg = gameObject.FindComponent<Image>("Image_SlimeCoreType_Big");
            Image smallImg = gameObject.FindComponent<Image>("Image_SlimeCoreType_Small");

            Sprite sprite = MonsterLandUtil.GetSlimeCoreImg(mType);

            bigImg.sprite = sprite;
            smallImg.sprite = sprite;
        }

        void InitButtonActions(CheapShop_SlimeCoreShopUI parent)
        {
            Button button_Max = gameObject.FindComponent<Button>("Button_Max");

            button_Max.SetButtonAction(SetMaxSlimeCore);

            Button button_Input = gameObject.FindComponent<Button>("Button_Input");

            button_Input.SetButtonAction(ShowCalculator);

            // =========================
            // 계산기 관련
            // =========================
            void ShowCalculator()
            {
                parent.ShowCalculator(CalculatorEnterAction, CalculatorMaxValueFunc);
            }

            void CalculatorEnterAction(double amount)
            {
                SetTotalSlimeCoreAmount(amount);

                mOnDecideAmount?.Invoke();
            }

            double CalculatorMaxValueFunc()
            {
                return MLand.SavePoint.GetSlimeCoreAmount(mType);
            }
        }

        void SetMinSlimeCore()
        {
            double minSlimeCore = Math.Min(0, MLand.SavePoint.GetSlimeCoreAmount(mType));

            SetTotalSlimeCoreAmount(minSlimeCore);

            mOnDecideAmount?.Invoke();
        }

        void SetMaxSlimeCore()
        {
            double maxSlimeCore = MLand.SavePoint.GetSlimeCoreAmount(mType);

            SetTotalSlimeCoreAmount(maxSlimeCore);

            mOnDecideAmount?.Invoke();
        }

        void SetTotalSlimeCoreAmount(double amount)
        {
            mTotalSlimeCoreAmount = amount;

            RefreshTotalSlimeCoreAmountText();
            RefreshTotalGoldText();
        }

        public double GetTotalSlimeCoreAmount()
        {
            return mTotalSlimeCoreAmount;
        }

        public double GetTotalPriceGold()
        {
            return mSlimeCorePrice * mTotalSlimeCoreAmount;
        }
    }
}