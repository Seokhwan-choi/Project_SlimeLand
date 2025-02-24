using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class DailyQuestItemUI : MonoBehaviour
    {
        Quest_DailyQuestUI mParent;
        GameObject mQuestClearObj;
        public void Init(Quest_DailyQuestUI parent, DailyQuestInfo info)
        {
            mParent = parent;

            RefreshProgress(info);
            RefreshName(info);
            RefreshReward(info);
            RefreshNewDot(info);

            Button buttonReceive = gameObject.FindComponent<Button>("Button_Receive");
            buttonReceive.SetButtonAction(() => OnReceiveButton(info));

            Image ImgButton = buttonReceive.GetComponent<Image>();

            string btnName = (info.CanReceiveReward && info.IsReceivedReward == false) ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

            ImgButton.sprite = MLand.Atlas.GetUISprite(btnName);

            mQuestClearObj = gameObject.FindGameObject("QuestClear");
            mQuestClearObj.SetActive(info.IsReceivedReward);
        }

        void RefreshNewDot(DailyQuestInfo info)
        {
            var newDot = gameObject.FindGameObject("NewDot");

            newDot.SetActive(info.CanReceiveReward && info.IsReceivedReward == false);
        }

        void RefreshProgress(DailyQuestInfo info)
        {
            float requireCount = info.RequireCount;
            float maxRequireCount = info.MaxRequireCount;

            Slider slider = gameObject.FindComponent<Slider>("Progress");
            slider.value = requireCount / maxRequireCount;

            TextMeshProUGUI textProgress = gameObject.FindComponent<TextMeshProUGUI>("Text_Progress");
            textProgress.text = $"{requireCount} / {maxRequireCount}";
        }

        void RefreshName(DailyQuestInfo info)
        {
            TextMeshProUGUI textQuestDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            StringParam param = new StringParam("count", info.Data.requireCount.ToString());

            textQuestDesc.text = StringTableUtil.Get($"Quest_Name_{info.Type}", param);
        }

        void RefreshReward(DailyQuestInfo info)
        {
            RewardData reward = MLand.GameData.RewardData.TryGet(info.Data.rewardId);
            if (reward == null)
                return;

            ItemInfo itemInfo = ItemInfo.CreateRewardInfo(reward);

            Image imgReward = gameObject.FindComponent<Image>("Image_Reward");
            imgReward.sprite = itemInfo.GetIconImg();

            TextMeshProUGUI textRewardAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_RewardAmount");
            textRewardAmount.text = itemInfo.GetAmountString();
        }

        void OnReceiveButton(DailyQuestInfo info)
        {
            if (info.IsReceivedReward)
            {
                var message = StringTableUtil.GetSystemMessage("IsAlreadyReceiveQuestReward");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            if (info.CanReceiveReward == false)
            {
                var message = StringTableUtil.GetSystemMessage("CantReceiveQuestReward");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            PlayQuestClearMotion();

            MLand.SavePoint.ReceiveDailyQuestReward(info.Id);

            MLand.Lobby.RefreshAllCurrencyText();

            MLand.Lobby.RefreshNewDot();
        }

        void PlayQuestClearMotion()
        {
            mQuestClearObj.SetActive(true);

            Image imgClear = mQuestClearObj.FindComponent<Image>("Image_Clear");

            imgClear.transform.localScale = Vector3.one * 3f;

            DOTween.Sequence()
                .Append(imgClear.transform.DOScale(Vector3.one, 0.25f))
                .Append(imgClear.transform.DOPunchScale(Vector3.one, 0.5f, elasticity: 0.15f))
                .OnComplete(() => SoundPlayer.PlayDropStamp());
        }
    }
}