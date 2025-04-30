using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


namespace MLand
{
    // 슬라임 소환, 건물 건축, 버프 관리
    class PopupStatus_SlimeKingUI : PopupStatusUI
    {
        SpeechBalloonUI mSpeechBalloonUI;
        SlimeKingTabUIManager mTabManager;
        public override void Init(PopupStatus type)
        {
            base.Init(type);

            GameObject speechBalloonObj = gameObject.FindGameObject("SpeechBalloon");

            mSpeechBalloonUI = speechBalloonObj.GetOrAddComponent<SpeechBalloonUI>();
            mSpeechBalloonUI.Init(StringTableUtil.GetName("SlimeKing"));

            mTabManager = new SlimeKingTabUIManager();
            mTabManager.Init(this);
        }

        public override void OnCloseButtonAction()
        {
            MLand.Lobby.HidePopupStatus();

            MLand.CameraManager.ResetFollowInfo();
        }

        public override void OnEnter(int subIdx) 
        {
            SlimeKingTab tab = (SlimeKingTab)subIdx;

            if ( mTabManager.ChangeTab(tab) == false )
            {
                mTabManager.RefreshCurTab();
            }
        }

        public override void Refresh()
        {
            mTabManager.RefreshCurTab();
        }

        public override void Localize()
        {
            mTabManager.Localize();

            string name = StringTableUtil.GetName("SlimeKing");
            string message = StringTableUtil.Get($"SsadaMessage_{mTabManager.CurTab}");

            mSpeechBalloonUI.Localize(name, message);
        }

        public void PlaySpeechBallonMotion(SlimeKingTab tab, bool playSound)
        {
            if (playSound)
                SoundPlayer.PlayMessageShowSound(0.5f);

            var message = StringTableUtil.Get($"SlimeKingMessage_{tab}");

            mSpeechBalloonUI.PlayTextMotion(message);
        }

        public void ScrollToTarget(string targetId)
        {
            mTabManager.ScrollToTarget(targetId);
        }

        public SlimeKingTabUI GetTab(SlimeKingTab tab)
        {
            return mTabManager.GetTab(tab);
        }

        public void MoveToTab(string id)
        {
            mTabManager.MoveToTab(id);
        }
    }
}

