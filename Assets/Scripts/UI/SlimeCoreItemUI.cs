using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MLand
{
    class SlimeCoreItemUI : MonoBehaviour
    {
        TextMeshProUGUI mTextAmount;
        public void Init(ElementalType type)
        {
            // 슬라임 똥 이미지 초기화
            Image imgSlimeCore = gameObject.FindComponent<Image>("Image_SlimeCore");
            imgSlimeCore.sprite = MLand.Atlas.GetCurrencySprite($"SlimeCore_{type}_Line");

            // 슬라임 똥 텍스트 캐슁
            mTextAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");
        }

        public void SetAmount(double amount)
        {
            mTextAmount.text = amount.ToAlphaString();
        }
    }
}