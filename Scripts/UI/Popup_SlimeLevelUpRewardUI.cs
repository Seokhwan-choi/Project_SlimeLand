using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Popup_SlimeLevelUpRewardUI : PopupBase
    {
        public void Init(string slimeId)
        {
            SetUpCloseAction();

            SetTitleText(StringTableUtil.Get("Title_RewardList"));

            CharacterData slimeData = MLand.GameData.SlimeData.TryGet(slimeId);

            int slimeLevel = slimeData.level;

            IEnumerable<SlimeLevelUpRewardData> datas = DataUtil.GetSlimeLevelUpRewardDataBySlimeLevel(slimeLevel);

            GameObject list = gameObject.FindGameObject("Content");

            foreach(SlimeLevelUpRewardData rewardData in datas)
            {
                GameObject levelUpItemObj = Util.InstantiateUI("SlimeLevelUpRewardItem", list.transform);

                levelUpItemObj.Localize();

                LevelUpRewardItemUI levelUpItem = levelUpItemObj.GetOrAddComponent<LevelUpRewardItemUI>();

                levelUpItem.Init(slimeId, rewardData);
            }
        }
    }

    class LevelUpRewardItemUI : MonoBehaviour
    {
        GameObject mNewDotObj;
        Image mClearModal;
        Image mClearStamp;
        GameObject mClearObj;

        string mSlimeId;
        SlimeLevelUpRewardData mData;
        bool CanReceiveReward => MLand.SavePoint.CanReceiveLevelReward(mSlimeId, mData.level);
        bool IsReceivedRewardData => MLand.SavePoint.IsReceivedSlimeLevelReward(mSlimeId, mData.level);
        public void Init(string slimeId, SlimeLevelUpRewardData data)
        {
            mSlimeId = slimeId;
            mData = data;

            // 레벨 표시
            TextMeshProUGUI textLevel = gameObject.FindComponent<TextMeshProUGUI>("Text_LevelNumber");
            textLevel.text = data.level.ToString();

            // 어떤 타입의 보상인지 표시
            var type1Obj = gameObject.FindGameObject("Type1");
            var type2Obj = gameObject.FindGameObject("Type2");
            if (data.increaseSlimeCoreDropAmount > 0 || data.decreaseSlimeCoreDropCoolTime > 0)
            {
                type1Obj.SetActive(false);
                type2Obj.SetActive(true);

                // 슬라임 똥 모양 초기화
                var slimeData = MLand.GameData.SlimeData.TryGet(mSlimeId);
                var imgSlimeCore = type2Obj.FindComponent<Image>("Image_SlimeCore");
                imgSlimeCore.sprite = MonsterLandUtil.GetSlimeCoreImg(slimeData.elementalType);

                var textRewardEffect = type2Obj.FindComponent<TextMeshProUGUI>("Text_RewardEffect");

                if (mData.increaseSlimeCoreDropAmount > 0)
                {
                    StringParam param = new StringParam("value", mData.increaseSlimeCoreDropAmount.ToString());

                    textRewardEffect.text = StringTableUtil.Get("Desc_SlimeCore_IncreaseDropAmount", param);
                }
                else if (mData.decreaseSlimeCoreDropCoolTime > 0)
                {
                    StringParam param = new StringParam("value", mData.decreaseSlimeCoreDropCoolTime.ToString());

                    textRewardEffect.text = StringTableUtil.Get("Desc_SlimeCore_DecreaseDropCoolTime", param);
                }

                // 보상 받기 버튼 초기화
                var button = type2Obj.FindComponent<Button>("Button_ReceiveReward");
                button.SetButtonAction(() =>
                {
                    bool receive = OnReceiveReward();
                    if (receive)
                    {
                        RefreshButtonImg();

                        SoundPlayer.PlayShowRewardPopup();
                    }
                });

                RefreshButtonImg();

                void RefreshButtonImg()
                {
                    // 보상 받을 수 없으면 회색
                    var imgButton = button.GetComponent<Image>();

                    string btnName = CanReceiveReward ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

                    imgButton.sprite = MLand.Atlas.GetUISprite(btnName);
                }
            }
            else
            {
                type1Obj.SetActive(true);
                type2Obj.SetActive(false);

                GameObject itemSlotObj = type1Obj.FindGameObject("ItemSlot_Num");
                ItemSlotUI itemSlot = itemSlotObj.GetOrAddComponent<ItemSlotUI>();
                itemSlot.Init(new ItemInfo(ItemType.Gem, data.rewardGem));

                var button = type1Obj.FindComponent<Button>("Button_ReceiveReward");
                button.SetButtonAction(() =>
                {
                    OnReceiveReward();

                    RefreshButtonImg();
                });

                RefreshButtonImg();

                void RefreshButtonImg()
                {
                    // 보상 받을 수 없으면 회색
                    var imgButton = button.GetComponent<Image>();

                    string btnName = CanReceiveReward ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

                    imgButton.sprite = MLand.Atlas.GetUISprite(btnName);
                }
            }

            mNewDotObj = gameObject.FindGameObject("NewDot");
            mClearObj = gameObject.FindGameObject("Clear");
            mClearModal = mClearObj.FindComponent<Image>("Image_Modal");
            mClearStamp = mClearObj.FindComponent<Image>("Image_Stamp");

            RefreshNewDot();
            
            mClearObj.SetActive(IsReceivedRewardData);


            bool OnReceiveReward()
            {
                if (MLand.SavePoint.ReceiveSlimeLevelUpReward(mSlimeId, mData.level))
                {
                    MLand.Lobby.RefreshAllCurrencyText();

                    RefreshNewDot();

                    PlayReceiveRewardMotion();

                    MLand.Lobby.RefreshNewDot();
                    MLand.Lobby.RefreshDetail();

                    return true;
                }
                else
                {
                    SoundPlayer.PlayErrorSound();

                    return false;
                }
            }
        }

        void RefreshNewDot()
        {
            mNewDotObj.SetActive(CanReceiveReward && IsReceivedRewardData == false);
        }

        void PlayReceiveRewardMotion()
        {
            mClearObj.SetActive(true);

            // 0.28 까지 fade
            Color startColor = Color.black;

            mClearModal.color = startColor;
            mClearModal.DOFade(0.28f, 0.5f);

            // 도장 쾅
            Vector3 startScale = Vector3.one * 3f;

            mClearStamp.rectTransform.localScale = startScale;
            mClearStamp.rectTransform.DOScale(1f, 0.5f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => SoundPlayer.PlayDropStamp());
        }
    }
}