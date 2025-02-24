using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Common;
using Google.Play.AppUpdate;

namespace MLand
{
    class InAppUpdateManager
    {
        AppUpdateManager mManager;
        public void Init()
        {
            mManager = new AppUpdateManager();
        }

        public IEnumerator CheckForUpdate()
        {
            PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
              mManager.GetAppUpdateInfo();

            yield return appUpdateInfoOperation;

            if (appUpdateInfoOperation.IsSuccessful)
            {
                AppUpdateInfo appUpdateInfoResult = appUpdateInfoOperation.GetResult();
                AppUpdateOptions appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

                var startUpdateRequest = mManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);

                yield return startUpdateRequest;
            }
            else
            {
                Debug.LogError($"{appUpdateInfoOperation.Error}");
            }
        }
    }
}