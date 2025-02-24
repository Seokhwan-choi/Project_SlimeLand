using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace MLand
{
    class SlimeKing_BuildingTab_ElementUI : SlimeKing_Tab_ElementUI
    {
        TextMeshProUGUI mTextBuildingPrice; // 소환 비용 텍스트
        TextMeshProUGUI mTextName;
        Button mButton_Lock;
        Button mButtonBuild;
        BuildingData mData;

        public Button ButtonBuild => mButtonBuild;
        public override void Init(string id)
        {
            base.Init(id);

            mData = MLand.GameData.BuildingData.TryGet(id);

            mTextBuildingPrice = mUnlockObj.FindComponent<TextMeshProUGUI>("Text_BuildingPrice");
            mTextName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            SetButtonAction();

            Refresh();
        }

        public override void Refresh()
        {
            SetBuildingImage();
            SetBuildingName();
            SetBuildingPrice();

            bool isCentral = mData.isCentralBuilding;
            if ( isCentral == false )
            {
                bool isSatisfiedCondition = MLand.GameManager.IsSatisfiedUnlockBuilding(mData.id);
                bool isUnlocked = MLand.GameManager.IsUnlockedBuilding(mData.id);

                SetLock(isSatisfiedCondition == false);
                SetActive(isSatisfiedCondition && isUnlocked);
            }
            else
            {
                bool isSatisfiedCondition = MLand.GameManager.IsSatisfiedUpgradeBuilding(mData.id);
                bool isMaxLevel = MLand.GameManager.IsMaxLevelBuilding(mData.id);

                SetLock(isSatisfiedCondition == false);
                SetActive(true);

                mButton_Lock.gameObject.SetActive(!isMaxLevel);
            }
        }

        public override void Localize()
        {
            SetBuildingName();
            SetBuildingPrice();
        }

        void SetBuildingName()
        {
            int level = MLand.SavePoint.GetBuildingLevel(mData.id);

            level = level == 0 ? 1 : level;

            mTextName.text = StringTableUtil.GetBuildingName(mData.id, level);
        }

        void SetBuildingPrice()
        {
            string priceStr = "0";

            bool isEnoughGold = false;

            if (mData.isCentralBuilding)
            {
                int level = MLand.SavePoint.GetBuildingLevel(mData.id);

                BuildingUpgradeData upgradeData = DataUtil.GetBuildingUpgradeData(mData.id, level + 1);
                if (upgradeData != null)
                {
                    isEnoughGold = MLand.SavePoint.IsEnoughGold(upgradeData.upgradePrice);

                    priceStr = upgradeData.upgradePrice.ToAlphaString();
                }
            }
            else
            {
                BuildingUnlockData unlockData = MLand.GameData.BuildingUnlockData.TryGet(mData.id);
                if(unlockData != null)
                {
                    isEnoughGold = MLand.SavePoint.IsEnoughGold(unlockData.unlockPrice);

                    priceStr = unlockData.unlockPrice.ToAlphaString();
                }
            }

            var buttonBuilding = gameObject.FindComponent<Image>("Button_Building");

            string btnSpriteName = isEnoughGold ? "Btn_Square_Green" : "Btn_Square_LightGray";

            buttonBuilding.sprite = MLand.Atlas.GetUISprite(btnSpriteName);

            mTextBuildingPrice.text = isEnoughGold ? priceStr : $"<color=red>{priceStr}</color>";
        }

        string GetLockReasonSystemMessage()
        {
            int buildngLevel = MLand.SavePoint.GetBuildingLevel(mData.id);

            string precendingBuildingName = string.Empty;

            if (mData.isCentralBuilding)
            {
                int nextLevel = buildngLevel + 1;

                BuildingUpgradeData data = DataUtil.GetBuildingUpgradeData(mData.id, nextLevel);
                if (data != null)
                {
                    precendingBuildingName = StringTableUtil.GetBuildingName(data.precendingBuilding, data.precendingBuildingLevel);
                }
            }
            else
            {
                BuildingUnlockData data = MLand.GameData.BuildingUnlockData.TryGet(mData.id);
                if (data != null)
                {
                    precendingBuildingName = StringTableUtil.GetBuildingName(data.precendingBuilding, data.precendingBuildingLevel);
                }
            }

            StringParam stringParam = new StringParam("building", precendingBuildingName);

            return StringTableUtil.GetSystemMessage("RequiredBuild", stringParam);
        }

        void SetBuildingImage()
        {
            Image activeImg = gameObject.FindComponent<Image>("Image_ActiveBuilding");
            
            int level = MLand.SavePoint.GetBuildingLevel(mData.id);

            level = level == 0 ? 1 : level;

            BuildingStatData statData = DataUtil.GetBuildingStatData(mData.id, level);

            Sprite sprite = MLand.Atlas.GetBuildingUISprite(statData.spriteImg);
            activeImg.sprite = sprite;

            Image silhouette = gameObject.FindComponent<Image>("Image_Silhouette");
            silhouette.sprite = sprite;
        }

        void SetButtonAction()
        {
            Button showDetailButton = gameObject.FindComponent<Button>("Button_ShowDetail");
            mButtonBuild = gameObject.FindComponent<Button>("Button_Building");
            mButton_Lock = mLockObj.FindComponent<Button>("Btn_LockReason");

            showDetailButton.SetButtonAction(ShowDetail);
            mButtonBuild.SetButtonAction(Building);
            mButton_Lock.SetButtonAction(ShowLockReason);
        }

        void ShowLockReason()
        {
            var anchor = mLockObj.FindGameObject("Anchor_SpeechBubble");

            var anchorTm = anchor.GetComponent<RectTransform>();

            Vector2 pos = anchorTm.position;
            string desc = GetLockReasonSystemMessage();

            MonsterLandUtil.ShowMiniSizeSpeechBubble(pos, desc);
        }

        void Building()
        {
            if (mData.id.IsValid() == false)
            {
                SoundPlayer.PlayErrorSound();

                return;
            }

            if (mData.isCentralBuilding)
            {
                var level = MLand.SavePoint.GetBuildingLevel(mData.id);
                var nextLevel = level + 1;
                BuildingUpgradeData upgradeData = DataUtil.GetBuildingUpgradeData(mData.id, nextLevel);
                if (upgradeData == null)
                {
                    SoundPlayer.PlayErrorSound();

                    return;
                }

                if (MLand.GameManager.IsReadyForUpgradeBuilding(mData.id) == false)
                {
                    SoundPlayer.PlayErrorSound();

                    return;
                }

                if (MLand.SavePoint.IsEnoughGold(upgradeData.upgradePrice) == false)
                {
                    var message = StringTableUtil.GetSystemMessage("NotEnoughGold");

                    MonsterLandUtil.ShowSystemMessage(message);

                    SoundPlayer.PlayErrorSound();

                    return;
                }

                MLand.Lobby.HidePopupStatus();

                MLand.GameManager.UpgradeBuilding(mData.id, upgradeData.upgradePrice, OnBuildFinish);
            }
            else
            {
                BuildingUnlockData unlockData = MLand.GameData.BuildingUnlockData.TryGet(mData.id);
                if (unlockData == null)
                {
                    SoundPlayer.PlayErrorSound();

                    return;
                }

                if (MLand.GameManager.IsReadyForUnlockBuilding(mData.id) == false)
                {
                    SoundPlayer.PlayErrorSound();

                    return;
                }

                if (MLand.SavePoint.IsEnoughGold(unlockData.unlockPrice) == false)
                {
                    var message = StringTableUtil.GetSystemMessage("NotEnoughGold");

                    MonsterLandUtil.ShowSystemMessage(message);

                    SoundPlayer.PlayErrorSound();

                    return;
                }

                MLand.Lobby.HidePopupStatus();

                MLand.GameManager.UnlockBuilding(mData.id, unlockData.unlockPrice, OnBuildFinish);
            }
        }

        void OnBuildFinish()
        {
            ShowDetail();

            Refresh();
        }

        void ShowDetail()
        {
            if (mData.id.IsValid() == false)
                return;

            Building build = MLand.GameManager.GetBuilding(mData.id);
            if (build != null)
            {
                MLand.CameraManager.SetFollowInfo(new FollowInfo()
                {
                    FollowTm = build.transform,
                    FollowType = FollowType.Building,
                });

                MLand.Lobby.HidePopupStatus();

                MLand.Lobby.ShowDetail(mData.id, DetailType.Building);
            }
        }
    }
}