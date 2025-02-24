using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG;
using DG.Tweening;

namespace MLand
{
    class Chatting_InternalMessageUI
    {
        CanvasGroup mMessage;
        TextMeshProUGUI mText_Message;

        Tweener mPunchTweener;
        Tweener mVisibleTweener;
        public void Init(GameObject parent)
        {
            mMessage = parent.FindComponent<CanvasGroup>("InternalMessage");
            mText_Message = parent.FindComponent<TextMeshProUGUI>("Text_InternalMessage");

            mMessage.gameObject.SetActive(false);
        }

        public void Play(string message)
        {
            mMessage.alpha = 1f;
            mMessage.gameObject.SetActive(true);

            mText_Message.text = message;

            mPunchTweener?.Rewind();
            mPunchTweener = mMessage.transform.DOPunchScale(Vector3.one * 0.5f, 1f);
            mPunchTweener.OnComplete(OnCompleteMessageTweener);
        }

        void OnCompleteMessageTweener()
        {
            mVisibleTweener?.Rewind();
            mVisibleTweener = DOTween.To((f) => mMessage.alpha = f, 1f, 0f, 0.75f);
            mVisibleTweener.OnComplete(() => mMessage.gameObject.SetActive(false));
        }
    }
}