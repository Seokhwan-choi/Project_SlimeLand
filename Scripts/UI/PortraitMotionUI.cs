using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class PortraitMotionUI : MonoBehaviour
    {
        public void Init()
        {
            var rectTransform = GetComponent<RectTransform>();

            float orgPosX = rectTransform.anchoredPosition.x;

            DOTween.Sequence()
                .Append(rectTransform.DOAnchorPosX(0, 0.5f))
                .Append(rectTransform.DOAnchorPosX(orgPosX, 0.75f)
                .SetDelay(0.5f))
                .OnComplete(() => gameObject.Destroy());

            var slimes = gameObject.FindGameObject("Slimes");
            slimes.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f)
                .SetEase(Ease.OutBack);
        }
    }
}