using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class ScrollToTarget
    {
        const float Padding = 10f;

        ScrollRect mScrollRect;         // 스크롤 뷰 컴포넌트
        public void Init(ScrollRect scroll)
        {
            mScrollRect = scroll;
        }

        public void Scroll(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            float endValue = mScrollRect.transform.InverseTransformPoint(mScrollRect.content.position).y
                    - mScrollRect.transform.InverseTransformPoint(target.position).y - (target.rect.height * 0.5f + 25f);

            mScrollRect.content.anchoredPosition = new Vector2(0f, endValue);
        }

        //public void Scroll(RectTransform target)
        //{
        //    // 대상 오브젝트가 스크롤 뷰 영역 안에 있는지 확인합니다.
        //    if (!IsRectTransformFullyInScrollView(target, mScrollRect))
        //    {
        //        // 대상 오브젝트의 상대 위치를 계산합니다.
        //        Vector3 targetPosition = mScrollRect.content.InverseTransformPoint(target.position);

        //        // Content Rect와 Viewport Rect의 상대 위치를 계산합니다.
        //        float contentHeight = mScrollRect.content.rect.height;
        //        float viewportHeight = mScrollRect.viewport.rect.height;
        //        float topY = -mScrollRect.content.anchoredPosition.y;
        //        float bottomY = topY - viewportHeight;
        //        float targetY = -targetPosition.y;

        //        // 대상 오브젝트가 상단에 더 가깝게 위치하는 경우
        //        if (targetY > topY)
        //        {
        //            mScrollRect.verticalNormalizedPosition = (targetY - viewportHeight) / (contentHeight - viewportHeight);
        //        }
        //        // 대상 오브젝트가 하단에 더 가깝게 위치하는 경우
        //        else if (targetY < bottomY)
        //        {
        //            mScrollRect.verticalNormalizedPosition = targetY / (contentHeight - viewportHeight);
        //        }
        //    }
        //}

        //// RectTransform이 스크롤 뷰 영역 안에 있는지 확인하는 함수입니다.
        //bool IsRectTransformFullyInScrollView(RectTransform rectTransform, ScrollRect scrollRect)
        //{
        //    Vector3[] corners = new Vector3[4];
        //    rectTransform.GetWorldCorners(corners);

        //    float scrollHeight = scrollRect.viewport.rect.height;
        //    float scrollY = scrollRect.viewport.localPosition.y;

        //    float minY = corners[0].y;
        //    float maxY = corners[2].y;

        //    return (maxY < scrollY + scrollHeight && minY > scrollY);
        //}
    }

}