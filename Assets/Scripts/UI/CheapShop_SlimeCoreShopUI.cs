using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class CheapShop_SlimeCoreShopUI : CheapShopTabUI
    {
        float mUpdateTime;
        TextMeshProUGUI mTextTodayPrice;
        TextMeshProUGUI mTextTotalSlimeCorePrice;
        Dictionary<ElementalType, CheapShop_SlimeCoreShop_Sell_ElementUI> mElementDics;
        public override void Init(CheapShopTabUIManager parent)
        {
            base.Init(parent);

            mTextTodayPrice = gameObject.FindComponent<TextMeshProUGUI>("Text_TodayPrice");

            mTextTotalSlimeCorePrice = gameObject.FindComponent<TextMeshProUGUI>("Text_TotalSlimeCorePrice");
            mTextTotalSlimeCorePrice.text = "0";

            mElementDics = new Dictionary<ElementalType, CheapShop_SlimeCoreShop_Sell_ElementUI>();
            for(int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                GameObject elementObj = gameObject.FindGameObject($"{type}_SlimeCore_Sell");

                var element = elementObj.GetOrAddComponent<CheapShop_SlimeCoreShop_Sell_ElementUI>();

                element.Init(this, type, OnDecideSlimeCoreAmount);

                mElementDics.Add(type, element);
            }

            Button button = gameObject.FindComponent<Button>("Button_SlimeCore_Sell");
            button.SetButtonAction(OnSellButtonAction);
        }

        public override void OnTabEnter()
        {
            base.OnTabEnter();

            foreach (var element in mElementDics.Values)
            {
                element.Refresh();
            }

            OnDecideSlimeCoreAmount();
        }

        public override void Localize()
        {
            foreach (var element in mElementDics.Values)
            {
                element.Localize();
            }

            OnDecideSlimeCoreAmount();
        }

        public override void OnTabLeave()
        {
            base.OnTabLeave();

            foreach (var element in mElementDics.Values)
            {
                element.OnTabLeave();
            }
        }

        public void ShowCalculator(UnityAction<double> onEnterButtonAction, Func<double> getNumberMaxValueFunc)
        {
            mParent.ShowCalculator(onEnterButtonAction, getNumberMaxValueFunc);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            foreach (var element in mElementDics.Values)
            {
                element.OnUpdate();
            }

            mUpdateTime -= Time.deltaTime;
            if (mUpdateTime <= 0)
            {
                mUpdateTime = 1f;

                UpdateRemainTime();
            }
        }

        void UpdateRemainTime()
        {
            int remainTime = MLand.SavePoint.Shop.UpdateRemainTime;

            StringParam param = new StringParam("time", TimeUtil.GetTimeStr(remainTime));

            mTextTodayPrice.text = StringTableUtil.Get("CheapShop_SlimeCoreUpdateRemainTime", param);
        }

        void OnDecideSlimeCoreAmount()
        {
            mTextTotalSlimeCorePrice.text = GetTotalSlimeCorePrice().ToAlphaString();
        }

        void OnSellButtonAction()
        {
            double totalGold = GetTotalSlimeCorePrice();
            if (totalGold == 0)
            {
                string message = StringTableUtil.GetSystemMessage("RequiredSellSlimeCores");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            MLand.GameManager.AddGold(totalGold);

            
            UseCurSelectedSlimeCores();

            // 판매가 모두 완료된 시점에서 UI를 리셋해준다.
            foreach (var element in mElementDics.Values)
                element.OnReset();

            OnDecideSlimeCoreAmount();

            SoundPlayer.PlayGetGold();
            SoundPlayer.PlayShopBuyOrSell();
        }

        double GetTotalSlimeCorePrice()
        {
            double totalGold = 0;

            foreach (var element in mElementDics)
            {
                ElementalType type = element.Key;
                double slimeCoreAmount = element.Value.GetTotalSlimeCoreAmount();

                if (MLand.SavePoint.IsEnoughSlimeCore(type, slimeCoreAmount))
                {
                    totalGold += element.Value.GetTotalPriceGold();
                }
                else
                {
                    Debug.LogError("가지고 있는 슬라임 똥 보다 많은 양으로 계산하려 함");

                    return 0;
                }
            }

            return totalGold;
        }

        void UseCurSelectedSlimeCores()
        {
            foreach (var element in mElementDics)
            {
                ElementalType type = element.Key;
                double slimeCoreAmount = element.Value.GetTotalSlimeCoreAmount();

                MLand.GameManager.UseSlimeCore(type, slimeCoreAmount);
            }
        }
    }
}