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

            Debug.Assert(itemInfos != null, $"���޵� ������ ���µ� ���� �˾��� �����ַ��� ��");

            GameObject rewardOne = gameObject.FindGameObject("Reward_One");
            GameObject rewardTen = gameObject.FindGameObject("Reward_Ten");

            // 1�� or 10���� ���� �����ϱ� ������ ������ ���� �ڵ�
            // �ٸ� ���� �����ϴ� ��찡 ����ٸ�, "�ּ�ȯ"�� �ڵ� �ٽ� ���ٲ���
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