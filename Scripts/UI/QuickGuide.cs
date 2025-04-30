using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    enum GuideType
    {
        None,
        Slime,
        Building,
        CentralBuilding,
    }

    class QuickGuide
    {
        string mId;
        double mRequireGold;
        GuideType mGuideType;

        GameObject mQuickGuideObj;
        GameObject mMotionObj;
        Image mImageSlime;
        Image mImageBuilding;
        Image mImageCentralBuilding;
        TextMeshProUGUI mTextGold;
        TextMeshProUGUI mTextName;
        TextMeshProUGUI mTextActionName;
        public void Init(LobbyUI lobby)
        {
            mQuickGuideObj = lobby.Find("QuickGuide");
            mMotionObj = mQuickGuideObj.FindGameObject("Motion");
            mTextName = mQuickGuideObj.FindComponent<TextMeshProUGUI>("Text_Name");
            mTextActionName = mQuickGuideObj.FindComponent<TextMeshProUGUI>("Text_ActionName");
            mTextGold = mQuickGuideObj.FindComponent<TextMeshProUGUI>("Text_GoldAmount");

            mImageSlime = mQuickGuideObj.FindComponent<Image>("Image_Slime");
            mImageBuilding = mQuickGuideObj.FindComponent<Image>("Image_Building");
            mImageCentralBuilding = mQuickGuideObj.FindComponent<Image>("Image_CentralBuilding");

            var button = mQuickGuideObj.GetComponent<Button>();
            button.SetButtonAction(OnQuickGuideButton);

            Refresh();
        }

        float mInterval;
        public void OnUpdate(float dt)
        {
            mInterval -= dt;
            if (mInterval <= 0f)
            {
                mInterval = 1f;

                RefreshGold();

                if (mId.IsValid() == false)
                {
                    Refresh();
                }
            }
        }

        public void Localize()
        {
            mQuickGuideObj.Localize();

            Refresh();
        }

        public void Refresh()
        {
            if (SavePointBitFlags.Tutorial_4_MiniGame.IsOn())
            {
                // 현재 소환/건축/진화 할 수 있는 것 중에서
                // 비용이 가장 저렴한 것을 찾는다.
                var result = GetMinRequireGoldAction();

                mId = result.id;
                mRequireGold = result.requireGold;
                mGuideType = result.guideType;

                mImageSlime.gameObject.SetActive(result.guideType == GuideType.Slime);
                mImageBuilding.gameObject.SetActive(result.guideType == GuideType.Building);
                mImageCentralBuilding.gameObject.SetActive(result.guideType == GuideType.CentralBuilding);

                if (result.guideType == GuideType.Slime)
                {
                    // 소환
                    var slimeData = MLand.GameData.SlimeData.TryGet(result.id);
                    mImageSlime.sprite = MLand.Atlas.GetCharacterUISprite(slimeData.spriteImg);
                    mTextActionName.text = StringTableUtil.Get("UIString_Spawn");
                    mTextName.text = StringTableUtil.GetName(result.id);

                    Show();
                }
                else if (result.guideType == GuideType.Building)
                {
                    // 건축
                    var buildingData = DataUtil.GetBuildingStatData(result.id);

                    mImageBuilding.sprite = MLand.Atlas.GetBuildingUISprite(buildingData.spriteImg);
                    mTextActionName.text = StringTableUtil.Get("UIString_Building");
                    mTextName.text = StringTableUtil.GetName(result.id);

                    Show();
                }
                else if (result.guideType == GuideType.CentralBuilding)
                {
                    // 건축
                    var buildingData = DataUtil.GetBuildingStatData(result.id);
                    mImageCentralBuilding.sprite = MLand.Atlas.GetBuildingUISprite(buildingData.spriteImg);
                    mTextActionName.text = StringTableUtil.Get("UIString_Evolution");

                    int level = MLand.SavePoint.GetBuildingLevel(result.id);

                    StringParam param = new StringParam("level", level.ToString());

                    mTextName.text = StringTableUtil.GetName(result.id, param);

                    Show();
                }
                else
                {
                    // 아무것도 할 게 없으니까 가리기
                    Hide();
                }
            }
            else
            {
                Hide();
            }

            RefreshGold();
        }

        (string id, double requireGold, GuideType guideType) GetMinRequireGoldAction()
        {
            string id = string.Empty;
            double requireGold = double.MaxValue;
            GuideType type = GuideType.None;

            foreach(var data in MLand.GameData.SlimeData.Values)
            {
                if (data.id == MLand.GameData.GoldSlimeCommonData.id)
                    continue;

                if (MLand.GameManager.IsReadyForSpawnSlime(data.id))
                {
                    if (requireGold > data.unlockPrice)
                    {
                        id = data.id;
                        requireGold = data.unlockPrice;
                        type = GuideType.Slime;
                    }
                }
            }

            foreach (var data in MLand.GameData.BuildingData.Values)
            {
                if (data.isCentralBuilding == false)
                {
                    if (MLand.GameManager.IsReadyForUnlockBuilding(data.id))
                    {
                        var unlockData = MLand.GameData.BuildingUnlockData.TryGet(data.id);
                        if (unlockData == null)
                            continue;

                        if (requireGold > unlockData.unlockPrice)
                        {
                            id = data.id;
                            requireGold = unlockData.unlockPrice;
                            type = GuideType.Building;
                        }
                    }
                }
                else
                {
                    if (MLand.GameManager.IsReadyForUpgradeBuilding(data.id))
                    {
                        int buildingLevel = MLand.SavePoint.GetBuildingLevel(data.id);
                        int nextLevel = buildingLevel + 1;

                        var upgradeData = DataUtil.GetBuildingUpgradeData(data.id, nextLevel);
                        if (upgradeData == null)
                            continue;

                        if (requireGold > upgradeData.upgradePrice)
                        {
                            id = data.id;
                            requireGold = upgradeData.upgradePrice;
                            type = GuideType.CentralBuilding;
                        }
                    }
                }
            }

            return (id, requireGold, type);
        }

        void RefreshGold()
        {
            mTextGold.text = mRequireGold.ToAlphaString();
            mTextGold.color = MLand.SavePoint.IsEnoughGold(mRequireGold) ? Color.white : Color.red;
        }

        void OnQuickGuideButton()
        {
            if (mId.IsValid() == false || mGuideType == GuideType.None)
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            if (MLand.SavePoint.IsEnoughGold(mRequireGold) == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGold");

                return;
            }

            if (mGuideType == GuideType.Slime)
            {
                if (MLand.GameManager.IsReadyForSpawnSlime(mId) == false)
                {
                    MonsterLandUtil.ShowSystemDefaultErrorMessage();

                    return;
                }

                MLand.GameManager.SpawnSlime(mId, mRequireGold, OnSpawnFinish);
            }
            else if (mGuideType == GuideType.Building)
            {
                if (MLand.GameManager.IsReadyForUnlockBuilding(mId) == false)
                {
                    MonsterLandUtil.ShowSystemDefaultErrorMessage();

                    return;
                }

                MLand.GameManager.UnlockBuilding(mId, mRequireGold, OnBuildFinish);
            }
            else
            {
                if (MLand.GameManager.IsReadyForUpgradeBuilding(mId) == false)
                {
                    MonsterLandUtil.ShowSystemDefaultErrorMessage();

                    return;
                }

                MLand.GameManager.UpgradeBuilding(mId, mRequireGold, OnBuildFinish);
            }

            void OnSpawnFinish()
            {
                Slime slime = MLand.GameManager.GetSlime(mId);
                if (slime != null)
                {
                    MLand.CameraManager.SetFollowInfo(new FollowInfo()
                    {
                        FollowTm = slime.transform,
                        FollowType = FollowType.Slime,
                    });

                    MLand.Lobby.HidePopupStatus();

                    MLand.Lobby.ShowDetail(mId, DetailType.Slime);
                }

                Refresh();
            }

            void OnBuildFinish()
            {
                Building build = MLand.GameManager.GetBuilding(mId);
                if (build != null)
                {
                    MLand.CameraManager.SetFollowInfo(new FollowInfo()
                    {
                        FollowTm = build.transform,
                        FollowType = FollowType.Building,
                    });

                    MLand.Lobby.HidePopupStatus();

                    MLand.Lobby.ShowDetail(mId, DetailType.Building);
                }

                Refresh();
            }
        }

        void Show()
        {
            if (mQuickGuideObj.activeSelf == false)
            {
                mQuickGuideObj.SetActive(true);
                mMotionObj.transform.DORewind();
                mMotionObj.transform.localScale = Vector3.zero;
                mMotionObj.transform.DOScale(1f, 0.5f)
                    .SetAutoKill(false)
                    .SetEase(Ease.InBack);
            }
        }

        void Hide()
        {
            if (mQuickGuideObj.activeSelf)
            {
                mMotionObj.transform.DORewind();
                mMotionObj.transform.localScale = Vector3.one;
                mMotionObj.transform.DOScale(0f, 0.5f)
                    .SetAutoKill(false)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => mQuickGuideObj.SetActive(false));
            }
        }
    }
}