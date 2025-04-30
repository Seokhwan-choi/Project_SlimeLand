using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MLand
{
    public class ButtonMotion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject ParentTarget;
        public float Speed = 100f;
        public Vector2 SunkSize = new Vector2(0.9f, 0.9f);
        public Color Color = Color.gray;
        public Text[] TargetTexts;
        public Image[] TargetImages;

        Vector2 mOrgSize;
        Color[] mTextOrgColors;
        Color[] mImageOrgColors;

        private bool mIsBtnDown;

        private void Awake()
        {
            if (ParentTarget == null)
                ParentTarget = this.gameObject;

            if (ParentTarget != null)
            {
                mOrgSize = ParentTarget.transform.localScale;
            }

            if (TargetTexts != null && TargetTexts.Length > 0)
            {
                mTextOrgColors = new Color[TargetTexts.Length];
                for (int i = 0; i < TargetTexts.Length; ++i)
                {
                    mTextOrgColors[i] = TargetTexts[i].color;
                }
            }

            if (TargetImages != null && TargetImages.Length > 0)
            {
                mImageOrgColors = new Color[TargetTexts.Length];
                for (int i = 0; i < TargetImages.Length; ++i)
                {
                    mImageOrgColors[i] = TargetImages[i].color;
                }
            }
        }

        private void Update()
        {
            float speed = Time.deltaTime * Speed;
            if (mIsBtnDown)
            {
                if ( ParentTarget != null )
                {
                    ParentTarget.transform.localScale = Vector2.Lerp(ParentTarget.transform.localScale, SunkSize, speed);
                }

                foreach(var text in TargetTexts)
                {
                    text.color = Color.Lerp(text.color, Color, speed);
                }

                foreach(var image in TargetImages)
                {
                    image.color = Color.Lerp(image.color, Color, speed);
                }
            }
            else
            {
                if ( ParentTarget != null )
                {
                    ParentTarget.transform.localScale = Vector2.Lerp(ParentTarget.transform.localScale, mOrgSize, speed);
                }

                if (mTextOrgColors != null)
                {
                    for (int i = 0; i < mTextOrgColors.Length; ++i)
                    {
                        var text = TargetTexts[i];
                        if (text.color != mTextOrgColors[i])
                            text.color = Color.Lerp(text.color, mTextOrgColors[i], speed);
                    }
                }

                if (mImageOrgColors != null)
                {
                    for (int i = 0; i < mImageOrgColors.Length; ++i)
                    {
                        var image = TargetImages[i];
                        if (image.color != mImageOrgColors[i])
                            image.color = Color.Lerp(image.color, mImageOrgColors[i], speed);
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            mIsBtnDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mIsBtnDown = false;
        }
    }
}
