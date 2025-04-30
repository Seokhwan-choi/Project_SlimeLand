using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;

namespace MLand
{
    class Popup_GiftUI : PopupBase
    {
        TextMeshProUGUI mGiftTotalAmount;
        Image mImgGiftButton;
        CalculatorUI mCalculator;
        Dictionary<string, GiftItemUI> mGiftItemDics;
        public void Init(string giftSlimeId, Action onFinishGift)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_Gift"));

            mGiftTotalAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Gift");
            mGiftItemDics = new Dictionary<string, GiftItemUI>();

            int index = 0;

            foreach(FriendShipItemData data in MLand.GameData.FriendShipItemData.Values)
            {
                GameObject giftItemObj = gameObject.FindGameObject($"GiftItem_{index}");

                GiftItemUI giftItem = giftItemObj.GetOrAddComponent<GiftItemUI>();

                giftItem.Init(this, data.id, GiftAmountChangeAction);

                index++;

                mGiftItemDics.Add(data.id, giftItem);
            }

            InitButtonAction(giftSlimeId, onFinishGift);

            GiftAmountChangeAction();

            var calcObj = gameObject.FindGameObject("Calculator");
            mCalculator = calcObj.GetOrAddComponent<CalculatorUI>();
            mCalculator.Init();

            Image imgSlime = gameObject.FindComponent<Image>("Image_Slime");
            imgSlime.sprite = MLand.Atlas.GetCharacterUISprite($"{giftSlimeId}_Happy");

            MonsterLandUtil.PlayUpAppearMotion(imgSlime.rectTransform, -500f);

            PlayTextMotion();
        }

        public void ShowCalculator(UnityAction<double> onEnterButtonAction, Func<double> getNumberMaxValueFunc)
        {
            mCalculator.Show(onEnterButtonAction, getNumberMaxValueFunc);
        }

        void SetGiftTotalAmountText()
        {
            double totalExp = GetTotalGiftExp();

            string totalStr = StringTableUtil.Get("UIString_Total");
            string friendShipStr = StringTableUtil.Get("UIString_FriendShip");
            string giftStr = StringTableUtil.Get("Title_Gift");

            mGiftTotalAmount.text = $"{totalStr} {totalExp} {friendShipStr}, {giftStr}";
        }

        void GiftAmountChangeAction()
        {
            SetGiftTotalAmountText();

            RefreshGiftButtonImg();
        }

        double GetTotalGiftExp()
        {
            double totalExp = 0;

            foreach (GiftItemUI giftItem in mGiftItemDics.Values)
            {
                totalExp += giftItem.GiftExp;
            }

            return totalExp;
        }

        void InitButtonAction(string giftSlimeId, Action onFinishGift)
        {
            var buttonGift = gameObject.FindComponent<Button>("Button_Gift");

            buttonGift.SetButtonAction(() =>
            {
                OnGiftButtonAction(giftSlimeId);

                onFinishGift.Invoke();
            });

            mImgGiftButton = buttonGift.GetComponent<Image>();
        }

        void RefreshGiftButtonImg()
        {
            string btnName = CanGift() ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

            mImgGiftButton.sprite = MLand.Atlas.GetUISprite(btnName);
        }

        bool CanGift()
        {
            return mGiftItemDics.Values.Sum(x => x.Amount) > 0;
        }

        void OnGiftButtonAction(string giftSlimeId)
        {
            if (CanGift() == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotSelectedGiftItems");

                return;
            }

            var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(giftSlimeId);
            if (slimeInfo == null)
            {
                MonsterLandUtil.ShowSystemErrorMessage("DefaultErrorMessage");

                return;
            }

            double stackedExp = slimeInfo.StackedExp;
            double totalExp = GetTotalGiftExp();
            double maxExp = DataUtil.GetMaxSlimeLevelExp();

            if (stackedExp + totalExp > maxExp)
            {
                // 경험치가 초과되어도 선물을 줄까?
                string title = StringTableUtil.Get("Title_Confirm");

                StringParam param = new StringParam("exp", ((stackedExp + totalExp) - maxExp).ToString());
                string desc = StringTableUtil.Get("Confirm_GiftOverExp", param);

                MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);
            }
            else
            {
                OnConfirm();
            }

            void OnConfirm()
            {
                int totalAmount = 0;

                foreach (var giftItem in mGiftItemDics)
                {
                    totalAmount += giftItem.Value.Amount;

                    double exp = MLand.SavePoint.UseFriendShipItem(giftItem.Key, giftItem.Value.Amount);

                    MLand.SavePoint.SlimeManager.StackFriendShipExp(giftSlimeId, exp);
                }

                MLand.SavePoint.CheckQuests(QuestType.GiftSlime, totalAmount);

                MLand.SavePoint.Save();

                SoundPlayer.PlaySlimeGift();

                Slime slime = MLand.GameManager.GetSlime(giftSlimeId);

                slime.Action.ChangeEmotion((int)EmotionType.Happy);
                slime.Action.RefreshCommunicationTime();

                this.Close();
            }
        }

        void PlayTextMotion()
        {
            var textSpeech = gameObject.FindComponent<TextMeshProUGUI>("Text_Speech");

            var orgTextSpeech = StringTableUtil.Get("UIString_GiftSlimeSpeech");

            textSpeech.text = string.Empty;
            textSpeech.DOText(orgTextSpeech, 0.5f);
        }
    }
}