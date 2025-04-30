using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MLand
{
    class TimeGaugeBarUI : GaugeBarUI
    {
        const string DefaultTimeIcon = "Img_Clack";
        const string TimeLimitIcon = "Img_Clack2";

        bool mIsDefault;
        bool mIsEndGame;
        public override void Init()
        {
            base.Init();

            OnReset(1f);
        }

        public void OnReset(float fillValue)
        {
            SetIcon(DefaultTimeIcon);

            SetFillValue(fillValue);

            ResetMotion();

            mIsDefault = true;
            mIsEndGame = false;
        }

        void ResetMotion()
        {
            mImgIcon.transform.DORewind();
            mImgIcon.transform.localScale = Vector3.one;
            mImgIcon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        public void StartTimeLimit()
        {
            StartCoroutine(PlayLoopTimeLimitSound(10));

            SetIcon(TimeLimitIcon);

            ShakeIcon();

            BounceIcon();
        }

        public void OnEndGame()
        {
            mIsEndGame = true;

            ResetMotion();
        }

        IEnumerator PlayLoopTimeLimitSound(int loop)
        {
            AudioSource audiosource = SoundPlayer.PlayTimeLimit();

            while (true)
            {
                if (audiosource != null && audiosource.isPlaying)
                {
                    if(mIsEndGame)
                    {
                        audiosource.Stop();

                        break;
                    }

                    yield return null;
                }
                else
                {
                    loop--;
                    if (loop < 0)
                    {
                        break;
                    }

                    audiosource = SoundPlayer.PlayTimeLimit();
                }
            }
        }

        void BounceIcon()
        {
            mImgIcon.transform.DOScale(0.9f, 0.25f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        void ShakeIcon()
        {
            PlayIconLocalRotate(isLeft:true, duration:0.25f,
                () => PlayIconLocalRotate(isLeft: false, duration:0.25f, ShakeIcon));
        }

        void PlayIconLocalRotate(bool isLeft, float duration, Action onComplete)
        {
            mImgIcon.transform.DOLocalRotate((isLeft ? Vector3.back : Vector3.forward) * 20f, duration * 0.5f)
                .SetLoops(1, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    mIsDefault = !mIsDefault;

                    SetIcon(mIsDefault ? DefaultTimeIcon : TimeLimitIcon);

                    onComplete?.Invoke();
                });
        }
    }
}