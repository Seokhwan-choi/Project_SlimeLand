using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace MLand
{
    class Popup_ChattingUI : PopupBase
    {
        string mSlimeId;
        ChattingSelectManager mSelectManager;
        Chatting_InternalMessageUI mInternalMessage;
        string SlimeName => StringTableUtil.GetName(mSlimeId);
        public void Init(string slimeId, Action onFinishChatting)
        {
            mSlimeId = slimeId;

            SetUpCloseAction();

            mSelectManager = new ChattingSelectManager();
            mSelectManager.Init(gameObject);

            mInternalMessage = new Chatting_InternalMessageUI();
            mInternalMessage.Init(gameObject);

            InitTitleText();

            StartCoroutine(PlaySlimeChattingMotion(onFinishChatting));
        }

        void InitTitleText()
        {
            TextMeshProUGUI textTitle = gameObject.FindComponent<TextMeshProUGUI>("Text_Title");

            textTitle.text = StringTableUtil.Get("Title_Chatting", new StringParam("value", SlimeName));
        }

        void ShowChatting(string message, bool isOther, float showTime)
        {
            GameObject parent = gameObject.FindGameObject("ChattingList");

            GameObject chattingItemObj = Util.InstantiateUI("ChattingItem", parent.transform);

            ChattingItemUI chattingItemUI = chattingItemObj.GetOrAddComponent<ChattingItemUI>();
            chattingItemUI.Init(mSlimeId, message, isOther, showTime);
        }

        IEnumerator PlaySlimeChattingMotion(Action onFinishChatting)
        {
            // �������� ������ �λ縻
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_1"), true, 0.5f);
            
            // �������� ����
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_2"), true, 1f);

            // �ý��� �޼��� : ##�� ����� Ǯ���� ����~?
            mInternalMessage.Play(StringTableUtil.Get("ChattingMessage_Start", new StringParam
                ("value", SlimeName)));

            yield return new WaitForSeconds(0.75f);

            // ������ ������ �� ��ȭ ������ �� ���� ���
            while(true)
            {
                if (mSelectManager.AnySelected)
                    break;

                yield return null;
            }

            (string, int) selectResult = mSelectManager.GetSelectResult();

            // ������ ������ ä�� ������
            yield return ShowChattingMotion(selectResult.Item1, false, 1f);

            // �������� ���
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_3"), true, 1f);

            // �ý��� �޽ý� : ȣ������ {value}��ŭ ����Ͽ����ϴ�.
            mInternalMessage.Play(StringTableUtil.Get("ChattingMessage_Finish", new StringParam
                ("value", selectResult.Item2.ToString())));

            yield return new WaitForSeconds(1.5f);

            MLand.SavePoint.StackFriendShipExp(mSlimeId, selectResult.Item2);

            onFinishChatting?.Invoke();

            // �˾� ����
            this.Close();
        }

        IEnumerator ShowChattingMotion(string message, bool isOther, float showTime)
        {
            ShowChatting(message, isOther, showTime);

            yield return new WaitForSeconds(showTime + 0.5f);
        }
    }
}