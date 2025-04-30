using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MLand
{
    class Popup_ExpensiveShop_ElementUI : MonoBehaviour
    {
        ExpensiveItem mItem;
        Image mImgSelected;
        Image mImgPurchased;
        bool mIsSelected;
        public string Id => mItem.Id;
        public double Price => mItem.Price;
        public bool IsSelected => mIsSelected;
        public bool IsAlreadyPurchased => mItem.IsAlreadyPurchased;
        public void Init(ExpensiveItem item, Action onSelectItemAction)
        {
            mItem = item;

            mIsSelected = false;

            mImgSelected = gameObject.FindComponent<Image>("Image_Selected");
            mImgSelected.gameObject.SetActive(false);

            mImgPurchased = gameObject.FindComponent<Image>("Image_Purchased");
            mImgPurchased.gameObject.SetActive(mItem.IsAlreadyPurchased);

            ItemInfo itemInfo = ItemInfo.CreateFriendShip(item.Id);

            // 상품 이미지
            Image imgItem = gameObject.FindComponent<Image>("Image_Item");
            imgItem.sprite = itemInfo.GetIconImg();

            // 상품 등급
            Image imgGrade = gameObject.FindComponent<Image>("Image_Grade");
            imgGrade.sprite = itemInfo.GetGradeImg();

            Image imgGradeInCircle = gameObject.FindComponent<Image>("Image_GradeInCircle");
            imgGradeInCircle.sprite = itemInfo.GetCircleImg();

            // 상품 가격
            TextMeshProUGUI textPrice = gameObject.FindComponent<TextMeshProUGUI>("Text_Price");
            textPrice.text = $"{((double)item.Price).ToAlphaString()}";

            // 상품 선택 버튼
            Button buttonOnSelect = gameObject.FindComponent<Button>("Button_OnSelect");
            buttonOnSelect.SetButtonAction(() =>
            {
                if (item.IsAlreadyPurchased)
                {
                    var message = StringTableUtil.GetSystemMessage("AlreadyPurchasedItemTrySelect");

                    MonsterLandUtil.ShowSystemMessage(message);

                    SoundPlayer.PlayErrorSound();

                    return;
                }

                SetSelected(!mIsSelected);

                onSelectItemAction?.Invoke();
            });

            // 상품 설명 버튼
            Button buttonShowItemInfo = gameObject.FindComponent<Button>("Button_ShowItemInfo");
            buttonShowItemInfo.SetButtonAction(() => MonsterLandUtil.ShowDescPopup(itemInfo));
        }

        public void OnPurchased()
        {
            mIsSelected = false;

            mImgSelected.gameObject.SetActive(false);

            mImgPurchased.gameObject.SetActive(true);

            MLand.SavePoint.ExpensiveShop.PurchaseItem(mItem.Key);

            SoundPlayer.PlayShopBuyOrSell();
        }

        void SetSelected(bool isSelected)
        {
            mIsSelected = isSelected;

            mImgSelected.gameObject.SetActive(isSelected);
        }
    }
}