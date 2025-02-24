using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MLand
{
    class Popup_DescriptionUI : PopupBase
    {
        ItemInfo mItemInfo;
        public void Init(ItemInfo itemInfo)
        {
            mItemInfo = itemInfo;

            SetUpCloseAction();
            SetTitleText(itemInfo.GetNameStr());

            GameObject itemSlotObj = gameObject.FindGameObject("ItemSlot");

            ItemSlotUI itemSlot = itemSlotObj.GetOrAddComponent<ItemSlotUI>();
            itemSlot.Init(mItemInfo, showAmount: false, showDesc:false);

            InitTextInfos();
            InitButtonInfo();
        }

        void InitTextInfos()
        {
            // 설명
            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = mItemInfo.GetDesc();

            GameObject gradeAndExp = gameObject.FindGameObject("GradeAndExp");
            gradeAndExp.SetActive(mItemInfo.IsFriendShipItem);

            // 등급 : ##
            TextMeshProUGUI textGrade = gameObject.FindComponent<TextMeshProUGUI>("Text_Grade");
            textGrade.text = mItemInfo.GetGradeStr();

            // 호감도 경험치 : ##
            TextMeshProUGUI textExp = gameObject.FindComponent<TextMeshProUGUI>("Text_Exp");
            textExp.text = mItemInfo.GetFriendShipExpStr();
        }

        void InitButtonInfo()
        {
            var buttonInfo = gameObject.FindComponent<Button>("Button_Info");

            buttonInfo.gameObject.SetActive(mItemInfo.Type == ItemType.RandomBox);

            if (mItemInfo.Type == ItemType.RandomBox)
            {
                var boxData = MLand.GameData.BoxData.TryGet(mItemInfo.Id);

                buttonInfo.SetButtonAction(() => MonsterLandUtil.ShowBoxProbabilityTable(boxData));
            }
        }
    }
}