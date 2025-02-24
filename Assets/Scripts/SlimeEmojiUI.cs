using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class SlimeEmojiUI : MonoBehaviour
    {
        const float PosOffsetY = 2f;

        bool mIsClosing;
        float mDuration;
        Image mImgEmoji;
        Transform mTarget;
        Transform mMotionBase;
        Transform mFollowBase;
        Tweener mVisibleTweener;
        public void Init(Transform target, EmotionType emotion)
        {
            mTarget = target;
            mFollowBase = gameObject.FindGameObject("Follower").transform;
            mMotionBase = gameObject.FindGameObject("Motion").transform;
            mImgEmoji = gameObject.FindComponent<Image>("Image_Emoji");

            SetPos();

            ChangeEmotion(emotion);
        }

        public void ChangeEmotion(EmotionType emotion)
        {
            RefreshDurationTime();

            SetEmojiImg(emotion);

            SoundPlayer.PlaySlimeEmotion(emotion);

            PlayPunchMotion();
        }

        void RefreshDurationTime()
        {
            mDuration = MLand.GameData.SlimeCommonData.emojiDuration;
        }

        void SetEmojiImg(EmotionType emotion)
        {
            var emoji = MLand.Atlas.GetUISprite($"Emoji_{emotion}");

            mImgEmoji.sprite = emoji;
        }

        private void Update()
        {
            SetPos();

            if (mDuration <= 0 && mIsClosing == false)
            {
                mDuration = 0;

                PlayPunchMotion();

                OnRelease();
            }
        }

        public void OnRelease(bool immediately = false)
        {
            PlayHideMotion(immediately);
        }

        void SetPos()
        {
            Vector3 pos = new Vector3(mTarget.position.x, mTarget.position.y + PosOffsetY, mTarget.position.z);

            pos = Util.WorldToScreenPoint(pos);

            mFollowBase.position = pos;
        }

        void PlayPunchMotion()
        {
            mMotionBase.DORewind();
            mMotionBase.DOPunchScale(Vector3.one * 0.25f, 0.5f, elasticity:0.15f);
        }

        void PlayHideMotion(bool immediately)
        {
            mIsClosing = true;

            CanvasGroup canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            canvasGroup.blocksRaycasts = true;

            if(immediately == false)
            {
                mVisibleTweener?.Rewind();
                mVisibleTweener = DOTween.To((f) => canvasGroup.alpha = f, 1f, 0f, 0.5f);
                mVisibleTweener.OnComplete(
                    () =>
                    {
                        Release();
                    });
            }
            else
            {
                Release();
            }

            void Release()
            {
                MLand.ObjectPool.ReleaseUI(gameObject);

                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = false;

                mIsClosing = false;
            }
        }
    }
}