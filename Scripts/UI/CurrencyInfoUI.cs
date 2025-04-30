using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class CurrencyInfoManager
    {
        TextMeshProUGUI mText_Gold;
        Button mButtonMoveToGoldShop;
        TextMeshProUGUI mText_Gem;
        Button mButtonMoveToGemShop;
        SlimeCoreItemUI[] mTextSlimeCores;

        public void Init(LobbyUI lobby)
        {
            GameObject parent = lobby.Find("CurrencyInfo");

            mText_Gold = parent.FindComponent<TextMeshProUGUI>("Text_Gold");
            mButtonMoveToGoldShop = parent.FindComponent<Button>("Button_MoveToGoldShop");
            mButtonMoveToGoldShop.SetButtonAction(() => MoveToGoldShop(lobby));

            mText_Gem = parent.FindComponent<TextMeshProUGUI>("Text_Gem");
            mButtonMoveToGemShop = parent.FindComponent<Button>("Button_MoveToGemShop");
            mButtonMoveToGemShop.SetButtonAction(() => MoveToGemShop(lobby));
            mTextSlimeCores = new SlimeCoreItemUI[(int)ElementalType.Count];

            GameObject slimeCoresParent = parent.FindGameObject("SlimeCores");

            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                GameObject slimeCoreItemObj = slimeCoresParent.FindGameObject($"{type}");

                SlimeCoreItemUI slimeCoreItem = slimeCoreItemObj.GetOrAddComponent<SlimeCoreItemUI>();

                slimeCoreItem.Init(type);

                mTextSlimeCores[i] = slimeCoreItem;
            }

            RefreshAllText();
        }

        void MoveToGoldShop(LobbyUI lobby)
        {
            lobby.ShowPopup(PopupStatus.CheapShop, (int)CheapShopTab.Gold);
        }

        void MoveToGemShop(LobbyUI lobby)
        {
            lobby.ShowPopup(PopupStatus.CheapShop, (int)CheapShopTab.Gem);
        }

        public void RefreshAllText()
        {
            RefreshGoldText();
            RefreshGemText();
            RefreshSlimeCoresText();
        }

        public void RefreshGoldText()
        {
            SetGoldText(MLand.SavePoint.GetTotalGold());
        }

        public void RefreshGemText()
        {
            SetGemText(MLand.SavePoint.GetTotalGem());
        }

        public void RefreshSlimeCoresText()
        {
            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                SetSlimeCoreText((ElementalType)i, MLand.SavePoint.GetSlimeCoreAmount(type));
            }
        }

        public void RefreshSlimeCoreText(ElementalType type)
        {
            SetSlimeCoreText(type, MLand.SavePoint.GetSlimeCoreAmount(type));
        }

        public void SetGoldText(double amount)
        {
            mText_Gold.text = amount.ToAlphaString();
        }

        public void SetGemText(double amount)
        {
            mText_Gem.text = amount.ToString();
        }

        public void SetSlimeCoreText(ElementalType type, double amount)
        {
            int index = (int)type;

            if (mTextSlimeCores.Length <= index)
                return;

            mTextSlimeCores[(int)type].SetAmount(amount); 
        }
    }
}