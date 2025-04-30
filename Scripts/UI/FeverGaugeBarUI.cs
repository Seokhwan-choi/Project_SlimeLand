using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MLand
{
    class FeverGaugeBarUI : GaugeBarUI
    {
        public override void Init()
        {
            base.Init();

            mImgIcon.gameObject.SetActive(false);

            SetFillImg("UI_Gaugebar_Fill_White");

            SetFillColor(Color.red);

            SetFillValue(0f);
        }
    }
}


