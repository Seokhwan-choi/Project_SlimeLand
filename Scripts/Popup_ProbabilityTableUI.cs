using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace MLand
{
    class Popup_ProbabilityTableUI : PopupBase
    {
        public void Init(BoxData boxData)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_ProbabilityTable"));

            GameObject list = gameObject.FindGameObject("List");

            if (boxData.boxType == BoxType.FriendShip)
            {
                var datas = MLand.GameData.FriendShipItemData.Values.OrderBy(x => x.grade);

                foreach (FriendShipItemData data in datas)
                {
                    ItemInfo itemInfo = ItemInfo.CreateFriendShip(data);

                    float gradeProb = boxData.gradeProb[(int)data.grade];
                    float itemTypeProb = boxData.itemTypeProb[(int)data.itemType];

                    GameObject itemObj = Util.InstantiateUI("ProbabilityItem", list.transform);

                    itemObj.Localize();

                    ProbabilityItem item = itemObj.GetOrAddComponent<ProbabilityItem>();

                    item.Init(itemInfo, gradeProb * itemTypeProb);
                }
            }
            else
            {
                foreach(var data in MLand.GameData.CostumeData.Values)
                {
                    ItemInfo itemInfo = ItemInfo.CreateCostume(data, 1);

                    float costumeTypeCount = MLand.GameData.CostumeData.Values.Count(x => x.costumeType == data.costumeType);
                    float costumeTypeProb = boxData.costumeTypeProb[(int)data.costumeType];

                    GameObject itemObj = Util.InstantiateUI("ProbabilityItem", list.transform);

                    itemObj.Localize();

                    ProbabilityItem item = itemObj.GetOrAddComponent<ProbabilityItem>();

                    item.Init(itemInfo, costumeTypeProb * (1 / costumeTypeCount));
                }
            }
        }
    }

    class ProbabilityItem : MonoBehaviour
    {
        public void Init(ItemInfo itemInfo, float prob)
        {
            var itemSlotObj = gameObject.FindGameObject("ItemSlot");

            var itemSlot = itemSlotObj.GetOrAddComponent<ItemSlotUI>();
            itemSlot.Init(itemInfo, showAmount:false);

            var textProb = gameObject.FindComponent<TextMeshProUGUI>("Text_Probability");

            textProb.text = $"{prob*100f:F}%";
        }
    }
}


