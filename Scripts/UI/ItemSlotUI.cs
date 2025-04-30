using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class ItemSlotUI : MonoBehaviour
    {
        ItemInfo mItemInfo;
        public ItemInfo ItemInfo => mItemInfo;
        public void Init(ItemInfo itemInfo, bool showAmount = true, bool showDesc = true)
        {
            mItemInfo = itemInfo;

            if (showAmount)
                SetText();

            SetImages(); 
            SetButtonAction(showDesc);
        }

        void SetButtonAction(bool showDesc)
        {
            if (showDesc)
            {
                Button button = GetComponentInChildren<Button>();
                button.SetButtonAction(() => MonsterLandUtil.ShowDescPopup(mItemInfo));
            }
        }

        void SetImages()
        {
            Image imgFrame = gameObject.FindComponent<Image>("Image_Frame");
            Sprite gradeImg = mItemInfo.GetGradeImg();
            imgFrame.sprite = gradeImg;

            Image imgInCircle = gameObject.FindComponent<Image>("Image_InCircle");
            Sprite circleImg = mItemInfo.GetCircleImg();
            imgInCircle.sprite = circleImg;

            Image imgIcon = gameObject.FindComponent<Image>("Image_Icon");
            Sprite iconImg = mItemInfo.GetIconImg();
            imgIcon.sprite = iconImg;
        }

        void SetText()
        {
            TextMeshProUGUI textAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");
            if (mItemInfo.Amount <= 0)
            {
                textAmount.gameObject.SetActive(false);
            }
            else
            {
                textAmount.gameObject.SetActive(true);

                textAmount.text = mItemInfo.GetAmountString();
            }
        }
    }
}