using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using System;

namespace MLand
{
    class ReviewManager
    {
        public void RequestReview()
        {
            if (IsSatisfiedReviewRequest())
            {
                string title = StringTableUtil.Get("Title_RequestReview");
                string desc = StringTableUtil.GetDesc("RequestReview");

                MonsterLandUtil.ShowConfirmPopup(title, desc, LaunchReview, FinishReview, ignoreModalTouch:true);
            }
        }

        void LaunchReview()
        {
            try
            {
                var review = new Google.Play.Review.ReviewManager();

                var playReviewInfoAsyncOperation = review.RequestReviewFlow();

                playReviewInfoAsyncOperation.Completed += (infoAsync) =>
                {
                    if (infoAsync.Error == ReviewErrorCode.NoError)
                    {
                        var reviewInfo = infoAsync.GetResult();
                        if (reviewInfo == null)
                        {
                            // ���� ������ �߻��ߴ�. 
                            // Url�� �ٷ� ��������
                            OpenUrl();
                        }
                        else
                        {
                            // �ξ� ���� ����!
                            review.LaunchReviewFlow(reviewInfo);
                        }
                    }
                    else
                    {
                        // ���� ������ �߻��ߴ�. 
                        // Url�� �ٷ� ��������
                        OpenUrl();
                    }
                };
            }
            catch(Exception e)
            {
                // Url�� �ٷ� ��������
                OpenUrl();

                Debug.LogError(e.Message);
            }

            FinishReview();
        }

        void FinishReview()
        {
            SavePointBitFlags.RequestReview.Set(true);
        }

        bool IsSatisfiedReviewRequest()
        {
            if (SavePointBitFlags.RequestReview.IsOn())
                return false;

            string buildingId = MLand.GameData.CommonData.requestReviewBuilding;
            int requireLevel = MLand.GameData.CommonData.requestReviewBuildingLevel;

            bool isUnlocked = MLand.SavePoint.BuildingManager.IsUnlockedBuilding(buildingId);
            if (isUnlocked == false)
                return false;

            int buildingLevel = MLand.SavePoint.BuildingManager.GetBuildingLevel(buildingId);
            if (buildingLevel < requireLevel)
                return false;

            return true;
        }

        void OpenUrl()
        {
            Application.OpenURL(MLand.GameData.CommonData.playStoreUrl);
        }
    }
}