using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLand
{
    static class BuyBoxUtil
    {
        public static void BuyBox(BoxShopData boxShopData, BoxOpenType openType)
        {
            BoxOpenResult[] results = MLand.SavePoint.Shop.BuyBox(boxShopData.id, openType);
            if (results != null)
            {
                string boxId = boxShopData.boxId[(int)openType];

                var boxData = MLand.GameData.BoxData.TryGet(boxId);

                ShowBoxResult(boxShopData.id, openType, boxData.id, results);

                if (boxShopData.boxType == BoxType.FriendShip)
                    MLand.SavePoint.CheckQuests(QuestType.OpenRandomBox, results.Length);
                else
                    MLand.SavePoint.CheckQuests(QuestType.OpenCostumeBox, results.Length);
            }
        }

        public static List<BoxOpenResult> OpenBox(BoxData boxData, bool isFriendShip, int bonusValue = 1)
        {
            int openCount = boxData.openCount * bonusValue;
            List<BoxOpenResult> randomItemList = new List<BoxOpenResult>();

            for (int i = 0; i < openCount; ++i)
            {
                if (isFriendShip)
                {
                    ItemType type = (ItemType)Util.RandomChoose(boxData.itemTypeProb);
                    ItemGrade grade = (ItemGrade)Util.RandomChoose(boxData.gradeProb);

                    FriendShipItemData itemData = DataUtil.GetRandomFriendShipItemData(type, grade);

                    randomItemList.Add(new BoxOpenResult()
                    {
                        Id = itemData.id,
                        Type = type,
                        Amount = 1,
                    });
                }
                else
                {
                    CostumeType type = (CostumeType)Util.RandomChoose(boxData.costumeTypeProb);

                    MinMaxRange range = MinMaxRange.Parse(boxData.costumeAmountRange);
                    int amount = range.SelectRandom();
                    CostumeData itemData = DataUtil.GetRandomCostumeData(type);

                    randomItemList.Add(new BoxOpenResult()
                    {
                        Id = itemData.id,
                        Type = ItemType.Costume,
                        Amount = amount,
                    });
                }
            }

            return randomItemList;
        }

        public static void ShowBoxResult(string boxShopId, BoxOpenType openType, string boxId, BoxOpenResult[] results)
        {
            Popup_BoxResultUI resultPopup = MLand.PopupManager.CreatePopup<Popup_BoxResultUI>();

            resultPopup.Init(boxShopId, openType, boxId, results);
        }
    }
}
