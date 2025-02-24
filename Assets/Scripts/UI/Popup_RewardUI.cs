using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Popup_RewardUI : PopupBase
    {
        public void Init(RewardData rewardData)
        {
            List<ItemInfo> rewardList = new List<ItemInfo>();

            if (rewardData.goldReward > 0)
            {
                var itemInfo = new ItemInfo(ItemType.Gold, rewardData.goldReward);

                rewardList.Add(itemInfo);
            }

            if (rewardData.gemReward > 0)
            {
                var itemInfo = new ItemInfo(ItemType.Gem, rewardData.gemReward);

                rewardList.Add(itemInfo);
            }

            if (rewardData.friendShipReward.IsValid())
            {
                var itemData = MLand.GameData.FriendShipItemData.TryGet(rewardData.friendShipReward);
                if (itemData != null)
                {
                    var itemInfo = ItemInfo.CreateFriendShip(itemData, rewardData.friendShipRewardCount);

                    rewardList.Add(itemInfo);
                }
            }

            if (rewardData.slimeCores != null && rewardData.slimeCores.Length == (int)ElementalType.Count)
            {
                for(int i = 0; i < (int)ElementalType.Count; ++i)
                {
                    if (rewardData.slimeCores[i] > 0)
                    {
                        ElementalType elementalType = (ElementalType)i;

                        var itemInfo = new ItemInfo(ItemType.SlimeCore, rewardData.slimeCores[i])
                            .SetElementalType(elementalType);

                        rewardList.Add(itemInfo);
                    }
                }
                
            }


            Init(rewardList.ToArray());
        }


        public void Init(ItemInfo[] itemInfos)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_Reward"));

            Debug.Assert(itemInfos != null, $"지급된 보상이 없는데 보상 팝업을 보여주려고 함");

            GameObject rewardOne = gameObject.FindGameObject("Reward_One");
            GameObject rewardTen = gameObject.FindGameObject("Reward_Ten");

            // 1개 or 10개의 보상만 지급하기 때문에 다음과 같이 코딩
            // 다른 갯수 지급하는 경우가 생긴다면, "최석환"이 코딩 다시 해줄꺼임
            bool isOneReward = itemInfos.Length == 1;

            rewardOne.SetActive(isOneReward);
            rewardTen.SetActive(!isOneReward);

            var slots = ItemSlotUtil.CreateItemSlotList(isOneReward ? rewardOne : rewardTen, itemInfos, showTextAmount:true);
            foreach (var slot in slots)
                slot.gameObject.SetActive(true);

            SoundPlayer.PlayShowRewardPopup();
        }
    }
}