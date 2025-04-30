using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class GaugeBarUI : MonoBehaviour
    {
        protected Slider mSlider;
        protected Image mImgFill;
        protected Image mImgIcon;
        public virtual void Init()
        {
            mSlider = gameObject.FindComponent<Slider>("GaugeBar");
            mImgFill = gameObject.FindComponent<Image>("Image_Fill");
            mImgIcon = gameObject.FindComponent<Image>("Image_Icon");
        }

        public void SetFillValue(float value)
        {
            mSlider.value = value;
        }

        public void SetFillImg(string gaugeImg)
        {
            var whiteGauge = MLand.Atlas.GetUISprite(gaugeImg);

            mImgFill.sprite = whiteGauge;
        }

        public void SetFillColor(Color color)
        {
            mImgFill.color = color;
        }

        public GaugeBarUI SetIcon(string iconName)
        {
            Sprite iconSprite = MLand.Atlas.GetUISprite(iconName);

            mImgIcon.sprite = iconSprite;

            return this;
        }
    }
}