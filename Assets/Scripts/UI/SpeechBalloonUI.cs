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
            // �̸� ����
            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            textName.text = name;

            // ��Ǯ�� ĳ��
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
            // �̸� ����
            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            textName.text = name;

            mTextSpeechBubble.text = text;
        }
    }
}


