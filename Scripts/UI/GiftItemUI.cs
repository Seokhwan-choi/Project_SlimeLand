using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MLand
{
    class GiftItemUI : MonoBehaviour
    {
        int mGiftAmount;
        FriendShipItemData mData;
        TextMeshProUGUI mTextAmount;
        public int Amount => mGiftAmount;
        public double GiftExp => mData.friendShipExp * mGiftAmount;
        public void Init(Popup_GiftUI parent, string id, Action giftAmountChangeAction)
        {
            mData = MLand.GameData.FriendShipItemData.TryGet(id);
            mTextAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");

            SetGiftAmount(0);

            InitImages();
            InitButtonActions(parent, giftAmountChangeAction);
        }

        void InitImages()
        {
            Image imgGrade = gameObject.FindComponent<Image>("Image_Grade");
            imgGrade.sprite = MLand.Atlas.GetUISprite($"Grade_{mData.grade}");

            Image imgGradeInCircel = gameObject.FindComponent<Image>("Image_GradeInCircle");
            imgGradeInCircel.sprite = MLand.Atlas.GetUISprite($"Grade_{mData.grade}_InCircle");

            Image imgIcon = gameObject.FindComponent<Image>("Image_Icon");
            imgIcon.sprite = MLand.Atlas.GetFriendShipSprite(mData.spriteImg);
        }

        void InitButtonActions(Popup_GiftUI parent, Action giftAmountChangeAction)
        {
            Button buttonShowDetail = gameObject.FindComponent<Button>("Button_ShowDetail");
            buttonShowDetail.SetButtonAction(() =>
            {
                MonsterLandUtil.ShowDescPopup(ItemInfo.CreateFriendShip(mData));
            });

            Button buttonDown = gameObject.FindComponent<Button>("Button_Down");
            Image imgButtonDown = buttonDown.GetComponent<Image>();
            Button buttonUp = gameObject.FindComponent<Button>("Button_Up");
            Image imgButtonUp = buttonUp.GetComponent<Image>();
            Button buttonMax = gameObject.FindComponent<Button>("Button_Max");

            buttonDown.SetButtonAction(() =>
            {
                OnButtonAction(true);

                giftAmountChangeAction?.Invoke();

                RefreshButtonImgs();
            });

            buttonUp.SetButtonAction(() =>
            {
                OnButtonAction(false);

                giftAmountChangeAction.Invoke();

                RefreshButtonImgs();
            });

            buttonMax.SetButtonAction(() =>
            {
                SetGiftAmount(MLand.SavePoint.Inventory.GetFriendShipItemAmount(mData.id));

                giftAmountChangeAction.Invoke();

                RefreshButtonImgs();
            });

            RefreshButtonImgs();

            var buttonCalc = gameObject.FindComponent<Button>("Button_Input");
            buttonCalc.SetButtonAction(ShowCalculator);

            // =========================
            // 계산기 관련
            // =========================
            void ShowCalculator()
            {
                parent.ShowCalculator(CalculatorEnterAction, CalculatorMaxValueFunc);
            }

            void CalculatorEnterAction(double amount)
            {
                SetGiftAmount((int)amount);

                giftAmountChangeAction?.Invoke();

                RefreshButtonImgs();
            }

            double CalculatorMaxValueFunc()
            {
                return MLand.SavePoint.Inventory.GetFriendShipItemAmount(mData.id);
            }

            void RefreshButtonImgs()
            {
                RefreshButtonImg(imgButtonDown, true);
                RefreshButtonImg(imgButtonUp, false);
            }
        }

        

        void OnButtonAction(bool isDown)
        {
            if (IsCantUpdateAmount(isDown))
            {
                SoundPlayer.PlayErrorSound();

                return;
            }

            SetGiftAmount(mGiftAmount + (isDown ? -1 : +1));
        }

        void SetGiftAmount(int amount)
        {
            mGiftAmount = amount;

            mTextAmount.text = $"{mGiftAmount.ToString()} / {MLand.SavePoint.Inventory.GetFriendShipItemAmount(mData.id)}";
        }

        bool IsCantUpdateAmount(bool isDown)
        {
            if (isDown)
            {
                return mGiftAmount - 1 < 0;
            }
            else
            {
                int maxAmount = MLand.SavePoint.Inventory.GetFriendShipItemAmount(mData.id);

                return mGiftAmount + 1 > maxAmount;
            }
        }

        void RefreshButtonImg(Image imgButton, bool isDown)
        {
            bool isCantUpdate = IsCantUpdateAmount(isDown);

            string btnSpriteName = isCantUpdate ? "Btn_Square_DarkGray" : "Btn_Square_Blue";

            imgButton.sprite = MLand.Atlas.GetUISprite(btnSpriteName);
        }
    }
}