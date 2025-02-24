using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

namespace MLand
{
    class Detail_SlimeUI : DetailUI
    {
        float mSpaceOutTime;
        Slider mFriendShipExpBar;
        GameObject mNewDotObj;
        TextMeshProUGUI mTextFriendShipExp;
        TextMeshProUGUI mTextFriendShipLevel;
        EquipCostumeItemUI[] mEquippedCostumes;

        GameObject mInfoObj;
        Button mButtonCostume;
        Button mButtonGift;
        Button mButtonReward;
        public override void Init(DetailListUI parent)
        {
            base.Init(parent);

            mNewDotObj = gameObject.FindGameObject("NewDot");
            mInfoObj = gameObject.FindGameObject("Info");
            mTextFriendShipExp = gameObject.FindComponent<TextMeshProUGUI>("Text_FriendShipExp");
            mTextFriendShipLevel = gameObject.FindComponent<TextMeshProUGUI>("Text_FriendShipLevel");
            mFriendShipExpBar = gameObject.FindComponent<Slider>("FriendShipExpBar");

            InitButtonActions();

            mEquippedCostumes = new EquipCostumeItemUI[(int)CostumeType.Count];

            GameObject costumeList = gameObject.FindGameObject("EquippedCostumes");

            for (int i = 0; i < (int)CostumeType.Count; ++i)
            {
                CostumeType cosType = (CostumeType)i;

                var itemObj = costumeList.FindGameObject($"Costume_{cosType}");

                EquipCostumeItemUI itemUI = itemObj.GetOrAddComponent<EquipCostumeItemUI>();

                itemUI.Init(cosType);

                mEquippedCostumes[i] = itemUI;
            }
        }

        void InitButtonActions()
        {
            mButtonCostume = gameObject.FindComponent<Button>("Btn_Costume");
            mButtonCostume.SetButtonAction(OnCostumeButtonAction);

            mButtonReward = gameObject.FindComponent<Button>("Btn_Reward");
            mButtonReward.SetButtonAction(OnRewardButtonAction);

            mButtonGift = gameObject.FindComponent<Button>("Btn_Gift");
            mButtonGift.SetButtonAction(OnGiftButtonAction);
        }

        void OnCostumeButtonAction()
        {
            var costume = MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_CostumeUI>();

            costume.OnChangeSlime(mCurId);

            MLand.Lobby.ShowPopup(PopupStatus.Costume);
        }

        void OnRewardButtonAction()
        {
            Popup_SlimeLevelUpRewardUI rewardPopup = MLand.PopupManager.CreatePopup<Popup_SlimeLevelUpRewardUI>();

            rewardPopup.Init(mCurId);
        }

        void OnGiftButtonAction()
        {
            Popup_GiftUI giftPopup = MLand.PopupManager.CreatePopup<Popup_GiftUI>();

            giftPopup.Init(mCurId, OnFinishGift);

            void OnFinishGift()
            {
                Refresh();
            }
        }

        void SetSlimeFriendShipLevel()
        {
            int level = MLand.SavePoint.GetSlimeLevel(mCurId);

            StringParam param = new StringParam("level", level.ToString());

            mTextFriendShipLevel.text = StringTableUtil.Get("UIString_FriendShipLevel", param);

            RefreshSlimeFriendShipExp();
            RefreshNewDot();

            mInfoObj.SetActive(true);
            mButtonCostume.gameObject.SetActive(true);
            mButtonGift.gameObject.SetActive(true);
            mButtonReward.gameObject.SetActive(true);
        }

        void RefreshSlimeFriendShipExp()
        {
            SavedSlimeInfo slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mCurId);

            int level = slimeInfo.Level;
            int nextLevel = level + 1;

            var currLevelData = MLand.GameData.SlimeFriendShipLevelUpData.Where(x => x.level == level).FirstOrDefault();
            var nextLevelData = MLand.GameData.SlimeFriendShipLevelUpData.Where(x => x.level == nextLevel).FirstOrDefault(); 

            if (nextLevelData == null)
            {
                mFriendShipExpBar.value = 1f;
                mTextFriendShipExp.text = StringTableUtil.Get("UIString_MaxLevel");
            }
            else
            {
                double curExp = slimeInfo.StackedExp - currLevelData.requireStackedExp;
                double requireExp = nextLevelData.requireStackedExp - currLevelData.requireStackedExp;

                mFriendShipExpBar.value = (float)curExp / (float)requireExp;
                mTextFriendShipExp.text = $"{curExp} / {requireExp}";
            }
        }

        void SetLuckySymbolFriendShipLevel()
        {
            StringParam param = new StringParam("level", MLand.SavePoint.LuckySymbol.Level.ToString());

            mTextFriendShipLevel.text = StringTableUtil.Get("UIString_FriendShipLevel", param);

            RefreshLuckySymbolFriendShipExp();
            RefreshNewDot();

            mInfoObj.SetActive(false);
            mButtonCostume.gameObject.SetActive(false);
            mButtonGift.gameObject.SetActive(false);
            mButtonReward.gameObject.SetActive(false);
        }

        void RefreshSlimeCostumes()
        {
            var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mCurId);
            if (slimeInfo != null)
            {
                if (mEquippedCostumes.Length == slimeInfo.Costumes.Length)
                {
                    for (int i = 0; i < slimeInfo.Costumes.Length; ++i)
                    {
                        string costumeId = slimeInfo.Costumes[i];

                        mEquippedCostumes[i].RefreshCostume(costumeId);
                    }
                }
            }
        }

        void RefreshLuckySymbolFriendShipExp()
        {
            SavedLuckySymbol info = MLand.SavePoint.LuckySymbol;

            int level = info.Level;
            int nextLevel = level + 1;

            var currLevelData = MLand.GameData.CharacterFriendShipLevelUpData.Where(x => x.level == level).FirstOrDefault();
            var nextLevelData = MLand.GameData.CharacterFriendShipLevelUpData.Where(x => x.level == nextLevel).FirstOrDefault();

            if (nextLevelData == null)
            {
                mFriendShipExpBar.value = 1f;
                mTextFriendShipExp.text = StringTableUtil.Get("UIString_FriendShipGaugeMax");
            }
            else
            {
                double curExp = info.StackedExp - currLevelData.requireStackedExp;
                double requireExp = nextLevelData.requireStackedExp - currLevelData.requireStackedExp;

                mFriendShipExpBar.value = (float)curExp / (float)requireExp;
                mTextFriendShipExp.text = $"{curExp} / {requireExp}";
            }
        }

        public override void RefreshNewDot()
        {
            if (mCurId.IsValid())
            {
                bool anyCanReceiveLevelReward = MLand.SavePoint.AnyCanReceiveLevelReward(mCurId);

                mNewDotObj.SetActive(anyCanReceiveLevelReward);
            }
        }

        void SetSlimeCoreDropAmount(CharacterData data)
        {
            double levelUpValue = MLand.SavePoint.SlimeManager.GetLevelUpRewardAmount(data.id);
            double costumeValue = MLand.SavePoint.GetSlimeCoreDropAmountByCostume(data.id);
            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.IncreaseDropAmount);

            double orgValue = data.slimeCoreDropAmount;
            string bonusStr = (buffValue > 1 || levelUpValue > 0 || costumeValue > 0) ? ((orgValue + levelUpValue + costumeValue) * buffValue - orgValue).ToAlphaString() : string.Empty;

            SetSlimeCoreAmountText(orgValue.ToAlphaString(), bonusStr);
        }

        void SetSlimeCoreDropCoolTime(CharacterData data)
        {
            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.DecreaseDropCoolTime);
            float levelUpValue = MLand.SavePoint.SlimeManager.GetLevelUpRewardCoolTime(data.id);
            float costumeValue = MLand.SavePoint.GetSlimeCoreDropCoolTimeByCostume(data.id);

            float orgValue = data.slimeCoreDropCoolTime;
            float bonusValue = levelUpValue + costumeValue + (buffValue > 1 ? (orgValue - levelUpValue - costumeValue) / buffValue : 0 );
            string bonusStr = bonusValue > 0 ? $"{bonusValue:F}" : string.Empty;

            SetSlimeCoreCoolTimeText($"{orgValue.ToString()}s", bonusStr);
        }

        float mIntervalTime;
        private void Update()
        {
            if (mIsShow == false && mCurId.IsValid())
                return;

            float dt = Time.deltaTime;
            mSpaceOutTime += dt;
            mIntervalTime -= dt;
            if (mIntervalTime <= 0f && mCurId != MLand.GameData.CommonData.luckySymbol)
            {
                mIntervalTime = 60f;

                int spaceOutTimeForMinute = Mathf.RoundToInt(mSpaceOutTime / TimeUtil.SecondsInMinute);

                MLand.SavePoint.CheckAchievements(AchievementsType.SpaceOut, spaceOutTimeForMinute);
            }
        }

        public override void OnShow(string showId)
        {
            if (mCurId.IsValid())
            {
                if (mCurId != showId)
                {
                    mSpaceOutTime = 0f;
                }
            }

            base.OnShow(showId);
        }

        public override void OnHide()
        {
            base.OnHide();

            mSpaceOutTime = 0f;
        }

        public override void OnListButtonAction()
        {
            base.OnListButtonAction();

            MLand.Lobby.ShowPopup(PopupStatus.SlimeKing, subIdx:(int)SlimeKingTab.Slime);

            MLand.GameManager.SetFollowSlimeKing();

            //  스크롤 뷰 자연스럽게 이동 시켜 놓자
            MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_SlimeKingUI>().ScrollToTarget(mCurId);
        }

        public override void Refresh()
        {
            if (mCurId.IsValid() == false)
                return;

            CharacterData data = DataUtil.GetCharacterData(mCurId);

            PublicRefresh(data);

            if (data.id != MLand.GameData.CommonData.luckySymbol)
            {
                // 이름
                SetNameText(StringTableUtil.GetName(mCurId));

                // 호감도 레벨
                SetSlimeFriendShipLevel();

                SetActiveListButton(true);
            }
            else
            {
                // 이름
                SetNameText(MLand.SavePoint.LuckySymbol.IsMaxLevel ? StringTableUtil.GetName(mCurId) : StringTableUtil.GetName($"None{mCurId}"));

                // 호감도 레벨
                SetLuckySymbolFriendShipLevel();

                SetActiveListButton(false);
            }

            RefreshSlimeCostumes();
        }

        void PublicRefresh(CharacterData data)
        {
            // 초상화 이미지
            SetPotraitImg(MLand.Atlas.GetCharacterUISprite(data.spriteImg));

            // 속성
            SetSlimeCoreElementalTypeText(data.elementalType);
            SetEmblemImg(data.elementalType);

            // 슬라임 똥 생산량 / 생산속도
            SetSlimeCore1Img(data.elementalType);
            SetSlimeCore2Img(data.elementalType);

            SetSlimeCoreDropAmount(data);
            SetSlimeCoreDropCoolTime(data);
        }
    }
}