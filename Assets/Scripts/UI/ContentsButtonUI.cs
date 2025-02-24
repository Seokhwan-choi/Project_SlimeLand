using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace MLand
{
    class ContentsButtonUI
    {
        public void Init(LobbyUI lobby)
        {
            GameObject contents_Buttons = lobby.Find("Contents_Buttons");

            InitButtonSetting(contents_Buttons);        // 설정   
            InitButtonAttendance(contents_Buttons);     // 출석부
            InitButtonQuest(contents_Buttons);          // 퀘스트
            InitAchievements(contents_Buttons);         // 업적
            InitRemoveADS(contents_Buttons);            // 광고 제거

            InitGoldSlime(lobby);                       // 황금 슬라임 on/off 유동적
            InitOfflineLevelUp(lobby);                  // 오프라인 보상
            InitPhoto(lobby);                           // 사진찍기
           
        }

        void InitButtonSetting(GameObject contents_Buttons)
        {
            Button buttonSetting = contents_Buttons.FindComponent<Button>("Button_Setting");
            buttonSetting.SetButtonAction(() =>
            {
                Popup_SettingUI popup = MLand.PopupManager.CreatePopup<Popup_SettingUI>();

                popup.Init();
            });
        }

        void InitButtonAttendance(GameObject contents_Buttons)
        {
            Button buttonAttendance = contents_Buttons.FindComponent<Button>("Button_Attendance");
            buttonAttendance.SetButtonAction(() =>
            {
                Popup_AttendanceUI popup = MLand.PopupManager.CreatePopup<Popup_AttendanceUI>();

                popup.Init();
            });
        }

        void InitButtonQuest(GameObject contents_Buttons)
        {
            Button buttonQuest = contents_Buttons.FindComponent<Button>("Button_Quest");
            buttonQuest.SetButtonAction(() =>
            {
                Popup_QuestUI popup = MLand.PopupManager.CreatePopup<Popup_QuestUI>();

                popup.Init();
            });
        }

        void InitAchievements(GameObject contents_Buttons)
        {
            Button buttonAchievements = contents_Buttons.FindComponent<Button>("Button_Achievements");
            buttonAchievements.SetButtonAction(() =>
            {
                Popup_AchievementsUI popup = MLand.PopupManager.CreatePopup<Popup_AchievementsUI>();

                popup.Init();
            });
        }

        Button mRemoveADSButton;
        void InitRemoveADS(GameObject contents_Buttons)
        {
            mRemoveADSButton = contents_Buttons.FindComponent<Button>("Button_RemoveADS");
            mRemoveADSButton.SetButtonAction(() =>
            {
                Popup_RemoveADS popup = MLand.PopupManager.CreatePopup<Popup_RemoveADS>();

                popup.Init();
            });

            bool active = true;
            var product = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
            if ( product != null )
            {
                active = !(product.hasReceipt && product.definition.type == ProductType.NonConsumable);
            }
            else
            {
                active = false;
            }

            SetActiveRemoveADSButton(active);
        }

        public void SetActiveRemoveADSButton(bool active)
        {
            mRemoveADSButton.gameObject.SetActive(active);
        }

        void InitOfflineLevelUp(LobbyUI lobby)
        {
            Button buttonOfflineLevelUp = lobby.gameObject.FindComponent<Button>("Button_OfflineRewardLevelUp");
            buttonOfflineLevelUp.SetButtonAction(() =>
            {
                Popup_OfflineRewardLevelUpUI popup = MLand.PopupManager.CreatePopup<Popup_OfflineRewardLevelUpUI>();

                popup.Init();
            });
        }

        void InitPhoto(LobbyUI lobby)
        {
            var photo = lobby.gameObject.FindComponent<Button>("Button_Photo");
            photo.SetButtonAction(() =>
            {
                Popup_PhotoUI popup = MLand.PopupManager.CreatePopup<Popup_PhotoUI>();

                popup.Init();
                popup.SetOnCloseAction(() => MLand.Lobby.ShowUI());
            });
        }

        GameObject mGoldSlime;
        TextMeshProUGUI mTextGoldSlimeLife;
        void InitGoldSlime(LobbyUI lobby)
        {
            mGoldSlime = lobby.gameObject.FindGameObject("GoldSlime");
            mTextGoldSlimeLife = mGoldSlime.FindComponent<TextMeshProUGUI>("Text_GoldSlimeLifeTime");

            Button buttonGoldSlime = mGoldSlime.FindComponent<Button>("Button_GoldSlime");
            buttonGoldSlime.SetButtonAction(() =>
            {
                string goldSlimeId = MLand.GameData.GoldSlimeCommonData.id;

                Slime goldSlime = MLand.GameManager.GetSlime(goldSlimeId);
                if (goldSlime != null)
                {
                    MLand.CameraManager.SetFollowInfo(new FollowInfo()
                    {
                        FollowTm = goldSlime.transform,
                        FollowType = FollowType.Slime,
                    });
                }
            });

            SetActiveGoldSlimeButton(MLand.GameManager.IsSpwanedGoldSlime());
        }

        public void SetActiveGoldSlimeButton(bool active)
        {
            mGoldSlime.SetActive(active);
        }

        public void RefreshGoldSlimeLifeTime(float lifeTime)
        {
            string timeStr = TimeUtil.GetTimeStr((int)lifeTime, ignoreHour: true);

            if (lifeTime <= 30f)
            {
                timeStr = $"<color=red>{timeStr}</color>";

                mTextGoldSlimeLife.transform.DORewind();
                mTextGoldSlimeLife.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, elasticity: 0.2f);
            }
            else
            {
                timeStr = $"<color=white>{timeStr}</color>";
            }

            mTextGoldSlimeLife.text = timeStr;
        }

        
    }
}