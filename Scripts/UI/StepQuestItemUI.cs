using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    class StepQuestItemUI : MonoBehaviour
    {
        Slider mSlider;
        Image mImgReward;
        Image mImgButtonReceive;
        TextMeshProUGUI mTextName;
        TextMeshProUGUI mTextProgress;
        TextMeshProUGUI mTextRewardAmount;

        GameObject mAllFinishObj;
        Image mAllFinishModal;
        Image mAllFinishStamp;

        StepQuestInfo mInfo;
        public void Init(StepQuestInfo info)
        {
            mInfo = info;

            InitButtonAction();
            InitAllFinish();

            Refresh();
        }

        public void Refresh()
        {
            RefreshReward();
            RefreshProgress();
            RefreshName();
            RefreshNewDot();
            RefreshButtonReceiveImg();
        }

        void RefreshNewDot()
        {
            var newDot = gameObject.FindGameObject("NewDot");

            newDot.SetActive(mInfo.IsSatisfiedStack() && mInfo.IsFinishAllSteps() == false);
        }

        void RefreshReward()
        {
            RewardData rewardData = MLand.GameData.RewardData.TryGet(mInfo.CurrentData.rewardId);
            if (rewardData == null)
            {
                SoundPlayer.PlayErrorSound();

                Debug.LogError($"{mInfo.Id} 업적의 보상데이터가 존재하지 않음");

                return;
            }

            if (mImgReward == null)
                mImgReward = gameObject.FindComponent<Image>("Image_Reward");

            if (mTextRewardAmount == null)
                mTextRewardAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_RewardAmount");

            ItemInfo rewardItemInfo = ItemInfo.CreateRewardInfo(rewardData);

            mImgReward.sprite = rewardItemInfo.GetIconImg();
            mTextRewardAmount.text = rewardItemInfo.GetAmountString();
        }

        void RefreshProgress()
        {
            if (mSlider == null)
                mSlider = gameObject.FindComponent<Slider>("Progress");

            if (mTextProgress == null)
                mTextProgress = gameObject.FindComponent<TextMeshProUGUI>("Text_Progress");

            ObscuredInt stackedCount = mInfo.StackedCount;
            int currentStepCount = mInfo.CurrentData.requireStackCount;

            mSlider.value = (float)stackedCount / (float)currentStepCount;
            mTextProgress.text = $"{stackedCount} / {currentStepCount}";
        }

        void RefreshName()
        {
            if (mTextName == null)
                mTextName = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            StringParam param = new StringParam("count", mInfo.CurrentData.requireStackCount.ToString());

            mTextName.text = StringTableUtil.Get($"Quest_Name_{mInfo.Type}", param);
        }

        void RefreshButtonReceiveImg()
        {
            string btnName = (mInfo.IsSatisfiedStack() && mInfo.IsFinishAllSteps() == false) ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

            mImgButtonReceive.sprite = MLand.Atlas.GetUISprite(btnName);
        }

        void InitButtonAction()
        {
            Button buttonReceive = gameObject.FindComponent<Button>("Button_Receive");

            buttonReceive.SetButtonAction(OnButtonReceive);

            mImgButtonReceive = buttonReceive.GetComponent<Image>();
        }

        void OnButtonReceive()
        {
            if (mInfo.IsFinishAllSteps())
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsAllFinishQuestStep");

                return;
            }

            if (mInfo.IsReceivedRewardStep(mInfo.CurrentStep))
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsAlreadyReceiveQuestReward");

                return;
            }

            if (mInfo.IsSatisfiedStack() == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("CantReceiveQuestReward");

                return;
            }

            RewardData rewardData = MLand.GameData.RewardData.TryGet(mInfo.CurrentData.rewardId);
            if (rewardData == null)
            {
                SoundPlayer.PlayErrorSound();

                return;
            }

            MLand.SavePoint.ReceiveStepQuestReward(mInfo.Id);

            MLand.Lobby.RefreshAllCurrencyText();

            MLand.Lobby.RefreshNewDot();

            if (mInfo.IsFinishAllSteps())
            {
                PlayAllFinishMotion();
            }
        }

        void InitAllFinish()
        {
            mAllFinishObj = gameObject.FindGameObject("AllFinish");
            mAllFinishObj.SetActive(mInfo.IsFinishAllSteps());

            mAllFinishModal = mAllFinishObj.FindComponent<Image>("Image_Modal");
            mAllFinishStamp = mAllFinishObj.FindComponent<Image>("Image_AllFinish");
            mAllFinishStamp.maskable = true;
        }

        void PlayAllFinishMotion()
        {
            mAllFinishObj.SetActive(true);
            mAllFinishStamp.enabled = false;
            mAllFinishStamp.maskable = false;

            var startColor = Color.black;
            startColor.a = 0f;

            mAllFinishModal.color = startColor;
            mAllFinishModal.DOFade(0.65f, 0.3f)
                .OnComplete(() =>
                {
                    mAllFinishStamp.enabled = true;
                    mAllFinishStamp.rectTransform.localScale = Vector3.one * 3f;

                    DOTween.Sequence()
                    .Append(mAllFinishStamp.rectTransform.DOScale(1f, 0.25f))
                    .Append(mAllFinishStamp.rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.5f, elasticity: 0.1f))
                    .OnComplete(() =>
                    {
                        SoundPlayer.PlayDropStamp();

                        mAllFinishStamp.maskable = true;
                    });
                });
        }
    }
}