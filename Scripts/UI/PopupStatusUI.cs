using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class PopupStatusUI : MonoBehaviour
    {
        protected PopupStatus mType;
        protected Button mClose_Button;
        protected RectTransform mRectTm;
        protected CanvasGroup mCanvasGroup;
        public PopupStatus PopupType => mType;
        public virtual void Init(PopupStatus type)
        {
            mType = type;
            mRectTm = GetComponent<RectTransform>();
            mCanvasGroup = GetComponent<CanvasGroup>();
            mClose_Button = gameObject.Find("Btn_Close", true)?.GetComponent<Button>();

            Debug.Assert(mClose_Button != null, $"{gameObject.name}의 PopupStatus에 Btn_Close가 존재하지 않음");

            SetOnCloseButtonAction(OnCloseButtonAction);
        }
        public virtual void Refresh() { }
        public virtual void Localize() { }
        public virtual void OnEnter(int subIdx) { }
        public virtual void OnLeave() { }
        public virtual void OnUpdate() { }
        public virtual void OnCloseButtonAction() { }
        void SetOnCloseButtonAction(UnityAction onCloseButtonAction)
        {
            mClose_Button.SetButtonAction(onCloseButtonAction);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        Sequence mShowMotionSequence;
        public void PlayShowMotion()
        {
            SoundPlayer.PlayShowPopup();

            SetActive(true);

            mCanvasGroup.blocksRaycasts = true;

            if (mShowMotionSequence != null)
            {
                mHideMotionSequence?.Rewind();
                mShowMotionSequence.Rewind();
                mShowMotionSequence.Play();
            }
            else
            {
                float startAlpha = 0f;
                float endAlpha = 1f;
                float startPos = -1280f;
                float endPos = 0f;
                float duration = 0.5f;

                mShowMotionSequence = DOTween.Sequence()
                    .Append(mRectTm.DOAnchorPosY(startPos, 0f))
                    .Join(mRectTm.DOAnchorPosY(endPos, duration).SetEase(Ease.OutBack))
                    .Join(DOTween.To((f) => mCanvasGroup.alpha = f, startAlpha, endAlpha, duration))
                    .SetAutoKill(false);
            }
        }

        Sequence mHideMotionSequence;
        public void PlayHideMotion()
        {
            SoundPlayer.PlayHidePopup();

            mCanvasGroup.blocksRaycasts = false;

            if (mHideMotionSequence != null)
            {
                mShowMotionSequence?.Rewind();
                mHideMotionSequence.Rewind();
                mHideMotionSequence.Play();
            }
            else
            {
                float startAlpha = 1f;
                float endAlpha = 0f;
                float startPos = 0f;
                float endPos = -1280f;
                float duration = 0.5f;

                mHideMotionSequence = DOTween.Sequence()
                .Append(mRectTm.DOAnchorPosY(startPos, 0f))
                .Join(mRectTm.DOAnchorPosY(endPos, duration).SetEase(Ease.InBack))
                .Join(DOTween.To((f) => mCanvasGroup.alpha = f, startAlpha, endAlpha, duration))
                .SetAutoKill(false)
                .OnComplete(() => SetActive(false));
            }
        }
    }
}




