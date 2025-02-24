using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG;
using DG.Tweening;

namespace MLand
{
    class Popup_SystemMessageUI : PopupBase
    {
        const float MotionPower = 10f;
        public void Init(string systemMessage)
        {
            TextMeshProUGUI textSystemMessage = gameObject.FindComponent<TextMeshProUGUI>("Text_SystemMessage");

            textSystemMessage.text = systemMessage;

            StartMotion();
        }

        void StartMotion()
        {
            Tweener tweener = transform.DOPunchPosition(Vector3.up * MotionPower, 1f);

            tweener.OnComplete(() => Close(immediate:false, hideMotion:false));
        }
    }
}


