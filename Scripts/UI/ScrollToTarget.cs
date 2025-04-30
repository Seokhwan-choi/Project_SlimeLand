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

        ScrollRect mScrollRect;         // ��ũ�� �� ������Ʈ
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
        //    // ��� ������Ʈ�� ��ũ�� �� ���� �ȿ� �ִ��� Ȯ���մϴ�.
        //    if (!IsRectTransformFullyInScrollView(target, mScrollRect))
        //    {
        //        // ��� ������Ʈ�� ��� ��ġ�� ����մϴ�.
        //        Vector3 targetPosition = mScrollRect.content.InverseTransformPoint(target.position);

        //        // Content Rect�� Viewport Rect�� ��� ��ġ�� ����մϴ�.
        //        float contentHeight = mScrollRect.content.rect.height;
        //        float viewportHeight = mScrollRect.viewport.rect.height;
        //        float topY = -mScrollRect.content.anchoredPosition.y;
        //        float bottomY = topY - viewportHeight;
        //        float targetY = -targetPosition.y;

        //        // ��� ������Ʈ�� ��ܿ� �� ������ ��ġ�ϴ� ���
        //        if (targetY > topY)
        //        {
        //            mScrollRect.verticalNormalizedPosition = (targetY - viewportHeight) / (contentHeight - viewportHeight);
        //        }
        //        // ��� ������Ʈ�� �ϴܿ� �� ������ ��ġ�ϴ� ���
        //        else if (targetY < bottomY)
        //        {
        //            mScrollRect.verticalNormalizedPosition = targetY / (contentHeight - viewportHeight);
        //        }
        //    }
        //}

        //// RectTransform�� ��ũ�� �� ���� �ȿ� �ִ��� Ȯ���ϴ� �Լ��Դϴ�.
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