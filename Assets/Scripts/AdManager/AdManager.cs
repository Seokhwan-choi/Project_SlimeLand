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
            IronSource.Agent.shouldTrackNetworkState(true); // 네트워크 연결 상태 관찰
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

        // 광고가 준비되었는지 확인
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
        // 번역기 - 보상형 동영상 광고 보기가 열렸습니다. 귀하의 활동은 초점을 잃게 됩니다.
        void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            //mIsOpenedAd = true;
        }
        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        // 번역기 - 보상형 동영상 광고 보기가 곧 닫히려 합니다. 귀하의 활동은 다시 초점을 맞출 것입니다.
        void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            //mIsOpenedAd = false;
        }

        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            // 광고 보상 처리
            mOnRewardedEvent?.Invoke();
            mOnRewardedEvent = null;

            MLand.SavePoint.CheckQuests(QuestType.WatchAd);
        }
    }
}