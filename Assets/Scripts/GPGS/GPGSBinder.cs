using System.Collections.Generic;
using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace MLand
{
    class GPGSBinder
    {
        ISavedGameClient mSavedGame => PlayGamesPlatform.Instance.SavedGame;
        void Init()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }
        public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
        {
            Init();

            PlayGamesPlatform.Instance.Authenticate((success) =>
            {
                onLoginSuccess?.Invoke(success == SignInStatus.Success, Social.localUser);
            });
        }

        // 로그인 여부 확인
        public static bool IsAuthenticated()
        {
            return PlayGamesPlatform.Instance.IsAuthenticated();
        }

        public void SaveCloud(string fileName, string saveData, Action<bool> onCloudSaved = null)
        {
            if (mSavedGame == null)
            {
                onCloudSaved?.Invoke(false);
            }
            else
            {
                mSavedGame.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        var update = new SavedGameMetadataUpdate.Builder().Build();
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(saveData);

                        mSavedGame.CommitUpdate(game, update, bytes, (status2, game2) =>
                        {
                            onCloudSaved?.Invoke(status2 == SavedGameRequestStatus.Success);
                        });
                    }
                });
            }
        }

        public void LoadCloud(string fileName, Action<bool, string> onCloudLoaded = null)
        {
            if (mSavedGame == null)
            {
                onCloudLoaded?.Invoke(false, null);
            }
            else
            {
                mSavedGame.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLastKnownGood, (status, game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        mSavedGame.ReadBinaryData(game, (status2, loadedData) =>
                        {
                            if (status2 == SavedGameRequestStatus.Success)
                            {
                                string data = System.Text.Encoding.UTF8.GetString(loadedData);
                                onCloudLoaded?.Invoke(true, data);
                            }
                            else
                                onCloudLoaded?.Invoke(false, null);
                        });
                    }
                });
            }
        }

        public void DeleteCloud(string fileName, Action<bool> onCloudDeleted = null)
        {
            mSavedGame.OpenWithAutomaticConflictResolution(fileName,
                DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, (status, game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        mSavedGame.Delete(game);

                        onCloudDeleted?.Invoke(true);
                    }
                    else
                        onCloudDeleted?.Invoke(false);
                });
        }

        // ==========================================================================
        // 업적
        // ==========================================================================
        public void ShowAchievementUI()
        {
            if (IsAuthenticated())
            {
                Social.ShowAchievementsUI();
            }
        }

        public void UnlockAchievement(string gpgsId, Action<bool> onUnlocked = null)
        {
            if (IsAuthenticated())
            {
                Social.ReportProgress(gpgsId, 100, success => onUnlocked?.Invoke(success));
            }
        }

        public void IncrementAchievement(string gpgsId, int steps, Action<bool> onUnlocked = null) =>
            PlayGamesPlatform.Instance.IncrementAchievement(gpgsId, steps, success => onUnlocked?.Invoke(success));

        // ==========================================================================
        // 리더 보드
        // ==========================================================================
        public void ShowAllLeaderboardUI() =>
            Social.ShowLeaderboardUI();

        public void ShowTargetLeaderboardUI(string gpgsId) =>
            ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(gpgsId);

        public void ReportLeaderboard(string gpgsId, long score, Action<bool> onReported = null) =>
            Social.ReportScore(score, gpgsId, success => onReported?.Invoke(success));

        public void LoadAllLeaderboardArray(string gpgsId, Action<UnityEngine.SocialPlatforms.IScore[]> onloaded = null) =>
            Social.LoadScores(gpgsId, onloaded);

        public void LoadCustomLeaderboardArray(string gpgsId, int rowCount, LeaderboardStart leaderboardStart,
            LeaderboardTimeSpan leaderboardTimeSpan, Action<bool, LeaderboardScoreData> onloaded = null)
        {
            PlayGamesPlatform.Instance.LoadScores(gpgsId, leaderboardStart, rowCount, LeaderboardCollection.Public, leaderboardTimeSpan, data =>
            {
                onloaded?.Invoke(data.Status == ResponseStatus.Success, data);
            });
        }


        // ==========================================================================
        // 이벤트
        // ==========================================================================
        //public void IncrementEvent(string gpgsId, uint steps)
        //{
        //    mEvents.IncrementEvent(gpgsId, steps);
        //}

        //public void LoadEvent(string gpgsId, Action<bool, IEvent> onEventLoaded = null)
        //{
        //    mEvents.FetchEvent(DataSource.ReadCacheOrNetwork, gpgsId, (status, iEvent) =>
        //    {
        //        onEventLoaded?.Invoke(status == ResponseStatus.Success, iEvent);
        //    });
        //}

        //public void LoadAllEvent(Action<bool, List<IEvent>> onEventsLoaded = null)
        //{
        //    mEvents.FetchAllEvents(DataSource.ReadCacheOrNetwork, (status, events) =>
        //    {
        //        onEventsLoaded?.Invoke(status == ResponseStatus.Success, events);
        //    });
        //}
    }
}

