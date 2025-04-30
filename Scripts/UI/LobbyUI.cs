using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;
using DG.Tweening;

namespace MLand
{
    class LobbyUI : MonoBehaviour
    {
        CanvasGroup mUIParent;
        CanvasScaler mCanvasScaler;

        CurrencyInfoManager mCurrencyInfoManager;
        PopupStatusManager mPopupStatusManager;
        BuffItemUIManager mBuffItemUIManager;
        IntroCloudManager mIntroCloudManager;
        NewDotManager mNewDotManager;
        ToastMessageManager mToastMessageManager;
        TutorialManager mTutorialManager;

        LobbyActionManager mLobbyActionManager;

        LobbyNavBarUI mNavBarUI;
        DetailListUI mDetailListUI;
        QuickGuide mQuickGuide;

        public void SetActiveIntroCloud()
        {
            var cloudManagerObj = Find("IntroCloud");
            cloudManagerObj.SetActive(true);
        }

        ContentsButtonUI mContentsButtonUI;

        RectTransform mTm;
        public Vector2 CanvasSize => mTm.sizeDelta;
        public CanvasScaler CanvasScaler => mCanvasScaler;
        public PopupStatusManager PopupStatusManager => mPopupStatusManager;
        public DetailListUI DetailList => mDetailListUI;
        public void Init()
        {
            mTm = GetComponent<RectTransform>();

            mCanvasScaler = GetComponent<CanvasScaler>();
            mUIParent = gameObject.FindComponent<CanvasGroup>("Parent");

            mNewDotManager = new NewDotManager();
            mNewDotManager.Init(this);

            mToastMessageManager = new ToastMessageManager();
            mToastMessageManager.Init(this);

            mCurrencyInfoManager = new CurrencyInfoManager();
            mCurrencyInfoManager.Init(this);

            mDetailListUI = new DetailListUI();
            mDetailListUI.Init(this);

            mPopupStatusManager = new PopupStatusManager();
            mPopupStatusManager.Init(this);

            mBuffItemUIManager = new BuffItemUIManager();
            mBuffItemUIManager.Init(this);

            mNavBarUI = new LobbyNavBarUI();
            mNavBarUI.Init(this);

            mContentsButtonUI = new ContentsButtonUI();
            mContentsButtonUI.Init(this);

            mTutorialManager = new TutorialManager();
            mTutorialManager.Init(this);

            mLobbyActionManager = new LobbyActionManager();
            mLobbyActionManager.Init();

            mQuickGuide = new QuickGuide();
            mQuickGuide.Init(this);

            MLand.CameraManager.SetCameraSize(MLand.GameData.CameraCommonData.cameraMaxSize);

            var cloudManagerObj = Find("IntroCloud");
            mIntroCloudManager = cloudManagerObj.GetOrAddComponent<IntroCloudManager>();

            Localize();
        }

        public void PlayIntroMotion()
        {
            // 오프라인 보상 지급할 수 있는지 계산하고 지급 결정
            if (MLand.SavePoint.OfflineRewardManager.HaveRewardToReceive())
            {
                ShowOfflineRewardPopup();
            }

            mIntroCloudManager.PlayIntroMotion();

            void ShowOfflineRewardPopup()
            {
                AddLobbyAction((lobbyAction) =>
                {
                    var popup = MLand.PopupManager.CreatePopup<Popup_OfflineRewardUI>();
                    popup.Init();

                    popup.SetOnCloseAction(() => lobbyAction.Done());
                });
            }
        }

        public void RefreshQuickGuide()
        {
            mQuickGuide.Refresh();
        }

        public void HideUI()
        {
            mUIParent.blocksRaycasts = false;

            DOTween.To((f) => mUIParent.alpha = f, 1f, 0f, 0.5f);
        }

        public void ShowUI()
        {
            mUIParent.blocksRaycasts = true;

            DOTween.To((f) => mUIParent.alpha = f, 0f, 1f, 0.5f);
        }

        public GameObject Find(string name)
        {
            return gameObject.FindGameObject(name);
        }

        public void ShowPopup(PopupStatus popup, int subIdx = 1)
        {
            mPopupStatusManager.ChangeActivePopup(popup, subIdx);

            HideDetail();

            if (popup == PopupStatus.SlimeKing)
                MLand.GameManager.SetFollowSlimeKing();
            else if (popup == PopupStatus.CheapShop)
                MLand.GameManager.SetFollowCheapShop();
        }

        public void HidePopupStatus()
        {
            mPopupStatusManager.HideCurPopup();
        }

        public void RefreshDetail()
        {
            mDetailListUI.Refresh();
        }

        public void ShowDetail(string id, DetailType type)
        {
            mDetailListUI.ChangeDetail(id, type);
        }

        public void HideDetail()
        {
            mDetailListUI.HideCurDetail();
        }

        public void Localize()
        {
            gameObject.Localize();

            // 모든 StringTableUIReader 찾아서 리프레시
            foreach (var uiReader in gameObject.GetComponentsInChildren<StringTableUIReader>(includeInactive:true))
            {
                uiReader.Localize();
            }

            mDetailListUI?.Refresh();
            mPopupStatusManager?.Localize();
            mQuickGuide?.Localize();
        }

        public void RefreshBuffDuration()
        {
            mBuffItemUIManager?.RefreshDurationTime();
        }

        public void RefreshAllCurrencyText()
        {
            mCurrencyInfoManager.RefreshAllText();
        }

        public void RefreshGoldText()
        {
            mCurrencyInfoManager.RefreshGoldText();
        }

        public void RefreshGemText()
        {
            mCurrencyInfoManager.RefreshGemText();
        }

        public void RefreshSlimeCoreText(ElementalType type)
        {
            mCurrencyInfoManager.RefreshSlimeCoreText(type);
        }

        public void RefreshSlimeCoresText()
        {
            mCurrencyInfoManager.RefreshSlimeCoresText();
        }

        public void SetActiveGoldSlimeButton(bool active)
        {
            mContentsButtonUI.SetActiveGoldSlimeButton(active);
        }

        public void SetActiveRemoveADSButton(bool active)
        {
            mContentsButtonUI.SetActiveRemoveADSButton(active);
        }

        public void RefreshGoldSlimeLifeTime(float lifeTime)
        {
            mContentsButtonUI.RefreshGoldSlimeLifeTime(lifeTime);
        }

        private void Update()
        {
            if (MLand.GameManager.IsPlay == false)
                return;

            float dt = Time.deltaTime;

            mPopupStatusManager.OnUpdate();
            mBuffItemUIManager.OnUpdate(dt);
            mToastMessageManager.OnUpdate(dt);
            mTutorialManager.OnUpdate(dt);
            mLobbyActionManager.OnUpdate();
            mQuickGuide.OnUpdate(dt);
        }

        public void RefreshNewDot()
        {
            mPopupStatusManager.Refresh();
            mDetailListUI.RefreshNewDot();
            mNewDotManager.Refresh();
        }

        public void AddToastMessage(string questId)
        {
            DailyQuestData data = MLand.GameData.DailyQuestData.TryGet(questId);
            if ( data != null )
            {
                StringParam param = new StringParam("count", data.requireCount.ToString());

                string desc = StringTableUtil.Get($"Quest_Name_{data.questType}", param);

                mToastMessageManager.AddToastMessage(desc);
            }
        }

        public void AddToastMessage(string id, int step)
        {
            StepQuestData data = DataUtil.GetStepQuestData(id, step);
            if ( data != null )
            {
                StringParam param = new StringParam("count", data.requireStackCount.ToString());

                string desc = StringTableUtil.Get($"Quest_Name_{data.type}", param);

                mToastMessageManager.AddToastMessage(desc);
            }
        }

        public void EnqueueLobbyAction(Action<LobbyAction> action)
        {
            mLobbyActionManager?.EnqueueLobbyAction(action);
        }

        public void AddLobbyAction(Action<LobbyAction> action)
        {
            mLobbyActionManager?.AddLobbyAction(action);
        }

        public void ShowAchievementsPopup(string id)
        {
            AddLobbyAction((lobbyAction) =>
            {
                Popup_AchievementsResultUI popup = MLand.PopupManager.CreatePopup<Popup_AchievementsResultUI>();

                popup.Init(id);

                popup.SetOnCloseAction(() => lobbyAction.Done());
            });
        }

        // 튜토리얼
        public void StartTutorial(string id, Action onEndTutorial = null)
        {
            AddLobbyAction((lobbyAction) =>
            {
                mTutorialManager?.StartTutorial(id, () =>
                {
                    onEndTutorial?.Invoke();

                    lobbyAction?.Done();
                });
            });
        }

        public void SetLockLobbyAction(bool isLock)
        {
            mLobbyActionManager.SetLock(isLock);
        }

        public bool InTutorial()
        {
            return mTutorialManager?.InTutorial ?? false;
        }
    }
}
