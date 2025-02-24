using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    static class ItemSlotUtil
    {
        public static List<ItemSlotUI> CreateItemSlotList(GameObject parent, ItemInfo[] itemInfos, bool showTextAmount)
        {
            List<ItemSlotUI> itemSlotList = new List<ItemSlotUI>();

            for (int i = 0; i < itemInfos.Length; ++i)
            {
                GameObject itemSlotObj = parent.FindGameObject($"ItemSlot_{i}");

                ItemSlotUI itemSlotUI = itemSlotObj.GetOrAddComponent<ItemSlotUI>();

                ItemInfo itemInfo = itemInfos[i];

                itemSlotUI.Init(itemInfo, showTextAmount);

                itemSlotList.Add(itemSlotUI);
            }

            return itemSlotList;
        }
    }
}
