using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace MLand
{
    class SlimeKing_SlimeTab_ElementUI : SlimeKing_Tab_ElementUI
    {
        GameObject mFriendShipObj;
        GameObject mNewDotObj;            // 레벨업 보상 레드닷
        TextMeshProUGUI mTextFriendShip; // 호감도 레벨 텍스트
        TextMeshProUGUI mTextSpawnPrice; // 소환 비용 텍스트
        TextMeshProUGUI mTextName;  
        CharacterData mData;
        Button mButtonSlimeSpawn;
        public Button ButtonSlimeSpawn => mButtonSlimeSpawn;
        public override void Init(string id)
        {
            base.Init(id);

            mData = MLand.GameData.SlimeData.TryGet(id);

            InternalInit();

            Refresh();
        }

        void InternalInit()
        {
            mFriendShipObj = gameObject.FindGameObject("FriendShip");
            mNewDotObj = gameObject.FindGameObject("NewDot");
            mTextFriendShip = mFriendShipObj.FindComponent<TextMeshProUGUI>("Text_FriendShip_Level");
            mTextSpawnPrice = mInactiveObj.FindComponent<TextMeshProUGUI>("Text_SpawnPrice");
            mTextName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            SetSlimeImage();
            SetButtonAction();
            SetSlimeName();
            SetSpawnPrice();
            SetFriendShipLevel();
        }

        public override void Refresh()
        {
            SetSpawnPrice();
            SetFriendShipLevel();

            bool isSatisfiedCondition = MLand.GameManager.IsSatisfiedConditionSlime(mData.id);
            bool isSpawned = MLand.GameManager.IsSpawnedSlime(mData.id);
            bool isActive = isSatisfiedCondition && isSpawned;

            SetLock(isSatisfiedCondition == false);
            SetActive(isActive);
            mFriendShipObj.SetActive(isActive);
            mNewDotObj.SetActive(MLand.SavePoint.AnyCanReceiveLevelReward(mData.id));
        }

        public override void Localize()
        {
            SetSlimeName();
            SetSpawnPrice();
            SetFriendShipLevel();
        }

        void SetSlimeName()
        {
            mTextName.text = StringTableUtil.GetName(mData.id);
        }

        void SetSpawnPrice()
        {
            bool isEnoughGold = MLand.SavePoint.IsEnoughGold(mData.unlockPrice);

            double price = mData.unlockPrice;

            mTextSpawnPrice.text = isEnoughGold ? price.ToAlphaString() : $"<color=red>{price.ToAlphaString()}</color>";

            Image image = mButtonSlimeSpawn.GetComponent<Image>();

            string btnSpriteName = isEnoughGold ? "Btn_Square_Green" : "Btn_Square_LightGray";

            image.sprite = MLand.Atlas.GetUISprite(btnSpriteName);
        }

        void SetFriendShipLevel()
        {
            int level = MLand.SavePoint.GetSlimeLevel(mData.id);

            mTextFriendShip.text = $"Lv.{level}";
        }

        string GetLockReasonSystemMessage()
        {
            if (mData.precendingBuilding.IsValid() && mData.precendingSlime.IsValid())
            {
                string precendingBuildingName = StringTableUtil.GetBuildingName(mData.precendingBuilding, mData.precendingBuildingLevel);
                string precendingSlimeNmae = StringTableUtil.GetName(mData.precendingSlime);

                var stringParam = new StringParam("building", precendingBuildingName);
                stringParam.AddParam("slime", precendingSlimeNmae);

                return StringTableUtil.GetSystemMessage("RequiredBuildAndSlime", stringParam);
            }
            else
            {
                string precendingSlimeNmae = StringTableUtil.GetName(mData.precendingSlime);

                var stringParam = new StringParam("slime", precendingSlimeNmae);

                return StringTableUtil.GetSystemMessage("RequiredSlime", stringParam);
            }
        }

        void SetSlimeImage()
        {
            Image emblemImg = gameObject.FindComponent<Image>("Image_Emblem");
            emblemImg.sprite = MonsterLandUtil.GetEmblemImg(mData.elementalType);

            Sprite slimeSprite = MonsterLandUtil.GetSlimeUIImg(mData.id);

            Image img = mActiveObj.FindComponent<Image>("Image_Slime");
            img.sprite = slimeSprite;

            Image silhouette = mLockObj.FindComponent<Image>("Image_Silhouette");
            silhouette.sprite = slimeSprite;
        }

        void SetButtonAction()
        {
            Button showDetailButton = mActiveObj.FindComponent<Button>("Button_ShowDetail");
            mButtonSlimeSpawn = mInactiveObj.FindComponent<Button>("Button_Spawn");
            Button lockReasonButton = mLockObj.FindComponent<Button>("Btn_LockReason");

            showDetailButton.SetButtonAction(ShowDetail);
            mButtonSlimeSpawn.SetButtonAction(SpawnSlime);
            lockReasonButton.SetButtonAction(ShowLockReason);
        }

        void ShowLockReason()
        {
            var anchor = mLockObj.FindGameObject("Anchor_SpeechBubble");

            var anchorTm = anchor.GetComponent<RectTransform>();

            Vector2 pos = anchorTm.position;
            string desc = GetLockReasonSystemMessage();

            MonsterLandUtil.ShowMiniSizeSpeechBubble(pos, desc);
        }

        void SpawnSlime()
        {
            if (mData.id.IsValid() == false)
                return;

            if (MLand.GameManager.IsReadyForSpawnSlime(mData.id) == false)
                return;

            if (MLand.SavePoint.IsEnoughGold(mData.unlockPrice) == false)
            {
                var message = StringTableUtil.GetSystemMessage($"NotEnoughGold");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            MLand.Lobby.HidePopupStatus();

            MLand.GameManager.SpawnSlime(mData.id, mData.unlockPrice, OnSpawnFinish);
        }

        void OnSpawnFinish()
        {
            ShowDetail();

            Refresh();
        }

        void ShowDetail()
        {
            if (mData.id.IsValid() == false)
                return;

            Slime slime = MLand.GameManager.GetSlime(mData.id);
            if (slime != null)
            {
                MLand.CameraManager.SetFollowInfo(new FollowInfo()
                {
                    FollowTm = slime.transform,
                    FollowType = FollowType.Slime,
                });

                MLand.Lobby.HidePopupStatus();

                MLand.Lobby.ShowDetail(mData.id, DetailType.Slime);
            }
        }
    }
}