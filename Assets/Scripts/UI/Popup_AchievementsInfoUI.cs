using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MLand
{
    class Popup_AchievementsInfoUI : PopupBase
    {
        public void Init()
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_Help"));

            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            textDesc.text = StringTableUtil.GetDesc("AchievementsInfo");
        }
    }
}