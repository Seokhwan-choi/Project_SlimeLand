using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MLand
{
    class IntroCloudManager : MonoBehaviour
    {
        public float CloudMovePos = 750f;
        public float CloudMotionScale = 1.5f;
        public float MotionMoveTime = 1.5f;
        public float MotionFadeTime = 0.75f;
        public float MotionScaleTime = 0.75f;
        public float ZoomSpeed = 1.5f;
        public float[] MotionDelay;
        public Image[] LeftClouds;
        public Image[] RightClouds;
        public void PlayIntroMotion()
        {
            StartCoroutine(PlayCloudMotions());
        }

        IEnumerator PlayCloudMotions()
        {
            SoundPlayer.PlayWind(0.5f);

            MLand.CameraManager.PlayResetZoomRoutine(ZoomSpeed);

            for (int i = 0; i < LeftClouds.Length; ++i)
            {
                Image imgLeftCloud = LeftClouds[i];
                Image imgRightCloud = RightClouds[i];

                imgLeftCloud.enabled = true;
                imgRightCloud.enabled = true;

                // 왼쪽 구름은 왼쪽으로
                DOTween.Sequence()
                    .Join(imgLeftCloud.rectTransform.DOAnchorPosX(-CloudMovePos, MotionMoveTime))
                    .Join(imgLeftCloud.DOFade(0f, MotionFadeTime))
                    .Join(imgLeftCloud.rectTransform.DOScale(CloudMotionScale, MotionScaleTime))
                    .OnComplete(() => imgLeftCloud.enabled = false);

                // 오른쪽 구름은 오른쪽으로
                DOTween.Sequence()
                    .Join(imgRightCloud.rectTransform.DOAnchorPosX(CloudMovePos, MotionMoveTime))
                    .Join(imgRightCloud.DOFade(0f, MotionFadeTime))
                    .Join(imgRightCloud.rectTransform.DOScale(CloudMotionScale, MotionScaleTime))
                    .OnComplete(() => imgRightCloud.enabled = false);

                yield return new WaitForSeconds(MotionDelay[i]);
            }

            yield return new WaitForSeconds(1.25f);

            MLand.GameManager.SetPlay(true);

            MLand.Lobby.StartTutorial($"{SavePointBitFlags.Tutorial_1_SpawnSlime}",
                () => MLand.Lobby.StartTutorial($"{SavePointBitFlags.Tutorial_2_BuildBuilding}",
                () => MLand.Lobby.StartTutorial($"{SavePointBitFlags.Tutorial_3_CheapShop}",
                () => MLand.Lobby.StartTutorial($"{SavePointBitFlags.Tutorial_4_MiniGame}"))));
        }
    }
}