using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace MLand
{
    class AdManager
    {
#if UNITY_ANDROID
        string mAppKey = "1de8ba0cd";
#else
        string mAppKey = "unexpected_platform";
#endif
        Action mOnRewardedEvent;

        public void Init()
        {
            IronSource.Agent.validateIntegration();         //
            IronSource.Agent.shouldTrackNetworkState(true); // ��Ʈ��ũ ���� ���� ����
            IronSource.Agent.init(mAppKey);
        }

        public void OnEnableEvent()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;

            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        }

        public void OnApplicationPauseEvent(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }

        void SDKInitialized()
        {
            //IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            //IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            //IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            //IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

            LoadRewarded();
        }

        // ���� �غ�Ǿ����� Ȯ��
        public bool IsRewardedVideoAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public void LoadRewarded()
        {
            IronSource.Agent.loadRewardedVideo();
        }

        public void ShowRewardedVideo(Action onUserRewarded, Action adFailedShow = null, Action adShown = null)
        {
            if (IsRewardedVideoAvailable() == false)
                return;

            mOnRewardedEvent = onUserRewarded;

            IronSource.Agent.showRewardedVideo();
        }

        // The Rewarded Video ad view has opened. Your activity will loose focus.
        // ������ - ������ ������ ���� ���Ⱑ ���Ƚ��ϴ�. ������ Ȱ���� ������ �Ұ� �˴ϴ�.
        void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            //mIsOpenedAd = true;
        }
        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        // ������ - ������ ������ ���� ���Ⱑ �� ������ �մϴ�. ������ Ȱ���� �ٽ� ������ ���� ���Դϴ�.
        void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            //mIsOpenedAd = false;
        }

        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            // ���� ���� ó��
            mOnRewardedEvent?.Invoke();
            mOnRewardedEvent = null;

            MLand.SavePoint.CheckQuests(QuestType.WatchAd);
        }
    }
}