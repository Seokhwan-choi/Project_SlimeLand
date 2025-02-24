using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace MLand
{
    class ChattingItemUI : MonoBehaviour
    {
        public void Init(string slimeId, string message, bool isOther, float showTime)
        {
            GameObject otherChatting = gameObject.FindGameObject("OtherChatting");
            GameObject myChatting = gameObject.FindGameObject("MyChatting");

            otherChatting.SetActive(isOther);
            myChatting.SetActive(!isOther);

            TextMeshProUGUI textSpeech = isOther ?
                otherChatting.FindComponent<TextMeshProUGUI>("Text_Speech") :
                myChatting.FindComponent<TextMeshProUGUI>("Text_Speech");

            textSpeech.text = string.Empty;
            textSpeech.DOText(message, showTime);
        }
    }
}


