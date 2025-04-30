using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MLand
{
    class Popup_MiniGameRewardListUI : PopupBase
    {
        public void Init(MiniGameType activeType)
        {
            SetUpCloseAction();

            SetTitleText(StringTableUtil.Get("Title_RewardList"));

            GameObject active = null;

            for(int i = 0; i < (int)MiniGameType.Count; ++i)
            {
                MiniGameType type = (MiniGameType)i;

                var content = gameObject.FindGameObject($"{type}");
                if (activeType == type)
                {
                    content.SetActive(true);

                    active = content;
                }
                else
                {
                    content.SetActive(false);
                }
            }

            int index = 0;

            foreach(MiniGameRewardData data in MLand.GameData.MiniGameRewardData.Values)
            {
                GameObject rewardItemObj = active.FindGameObject($"Reward_{index+1}");

                RewardItem rewardItem = rewardItemObj.GetOrAddComponent<RewardItem>();

                rewardItem.Init(data.rewardId, data.score, activeType);

                index++;
            }
        }
    }

    class RewardItem : MonoBehaviour
    {
        public void Init(string rewardId, string score, MiniGameType type)
        {
            RewardData rewardData = MLand.GameData.RewardData.TryGet(rewardId);

            ItemInfo itemInfo = ItemInfo.CreateRewardInfo(rewardData);

            GameObject itemSlotObj = gameObject.FindGameObject("ItemSlot_Num");

            ItemSlotUI itemSlot = itemSlotObj.GetOrAddComponent<ItemSlotUI>();

            itemSlot.Init(itemInfo);

            if (type == MiniGameType.ElementalCourses)
            {
                var textScore = gameObject.FindComponent<TextMeshProUGUI>("Text_Score");

                textScore.text = score;
            }
        }
    }
}


