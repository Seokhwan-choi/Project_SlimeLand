using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Like
{
    static class RectTransformUtil
    {
        public static RectTransform GetRt(this GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null)
            {
                rt = go.AddComponent<RectTransform>();
            }

            return rt;
        }

        public static Rect GetRect(this GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null)
            {
                rt = go.AddComponent<RectTransform>();
            }

            return rt.WorldRect();
        }

        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            return a.WorldRect().Overlaps(b.WorldRect());
        }
        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
        }

        public static bool IntersectRect(this RectTransform a, RectTransform b, out Rect result)
        {
            Rect rectA = a.rect;
            Rect rectB = b.rect;

            // x 좌표를 기준으로 누가 더 오른쪽에 있는지 확인
            float width; float posX;
            if (rectA.center.x > rectB.center.x)
            {
                width = rectB.xMax - rectA.xMin;
                posX = rectA.xMin + (width * 0.5f);
            }
            else
            {
                width = rectA.xMax - rectB.xMin;
                posX = rectB.xMin + (width * 0.5f);
            }

            // y 좌표를 기준으로 누가 더 위에 있는지 확인
            float height; float posY;
            if (rectA.center.y > rectB.center.y)
            {
                height = rectB.yMax - rectA.yMin;
                posY = rectB.yMax + (height * 0.5f);
            }
            else
            {
                height = rectA.yMax - rectB.xMin;
                posY = rectB.yMin + (width * 0.5f);
            }

            result = new Rect(posX, posY, width, height);

            return a.Overlaps(b);
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            var rect = rectTransform.rect;
            rect.center = rectTransform.TransformPoint(rect.center);
            rect.size = rectTransform.TransformVector(rect.size);
            return rect;
        }
    }
}

