using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class SpeechBalloonUI : MonoBehaviour
    {
        TextMeshProUGUI mTextSpeechBubble;
        public void Init(string name)
        {
            // 이름 설정
            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            textName.text = name;

            // 말풀선 캐슁
            mTextSpeechBubble = gameObject.FindComponent<TextMeshProUGUI>("Text_Speech");
        }

        public void PlayTextMotion(string text)
        {
            mTextSpeechBubble.DORewind();
            mTextSpeechBubble.text = string.Empty;
            mTextSpeechBubble.DOText(text, 1f);
        }

        public void Localize(string name, string text)
        {
            // 이름 설정
            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            textName.text = name;

            mTextSpeechBubble.text = text;
        }
    }
}


