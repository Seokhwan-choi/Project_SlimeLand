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
            // 슬라임의 간단한 인사말
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_1"), true, 0.5f);
            
            // 슬라임의 주제
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_2"), true, 1f);

            // 시스템 메세지 : ##의 기분을 풀어줘 볼까~?
            mInternalMessage.Play(StringTableUtil.Get("ChattingMessage_Start", new StringParam
                ("value", SlimeName)));

            yield return new WaitForSeconds(0.75f);

            // 유저가 선택할 때 대화 선택할 때 까지 대기
            while(true)
            {
                if (mSelectManager.AnySelected)
                    break;

                yield return null;
            }

            (string, int) selectResult = mSelectManager.GetSelectResult();

            // 유저가 선택한 채팅 보여줌
            yield return ShowChattingMotion(selectResult.Item1, false, 1f);

            // 슬라임의 대답
            yield return ShowChattingMotion(StringTableUtil.Get("ChattingMessage_SlimeSpeech_3"), true, 1f);

            // 시스템 메시시 : 호감도가 {value}만큼 상승하였습니다.
            mInternalMessage.Play(StringTableUtil.Get("ChattingMessage_Finish", new StringParam
                ("value", selectResult.Item2.ToString())));

            yield return new WaitForSeconds(1.5f);

            MLand.SavePoint.StackFriendShipExp(mSlimeId, selectResult.Item2);

            onFinishChatting?.Invoke();

            // 팝업 종료
            this.Close();
        }

        IEnumerator ShowChattingMotion(string message, bool isOther, float showTime)
        {
            ShowChatting(message, isOther, showTime);

            yield return new WaitForSeconds(showTime + 0.5f);
        }
    }
}