using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class Popup_SpeechBubbleUI : PopupBase
    {
        public void Init(Vector2 pos, string desc)
        {
            GetComponent<RectTransform>().position = pos;

            TextMeshProUGUI textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = desc;

            Button buttonModal = gameObject.FindComponent<Button>("Button_Modal");
            buttonModal.SetButtonAction(() => Close());
        }
    }

}