using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class Popup_MiniGameHomeUI : PopupBase
    {
        Image mImgFade;
        public void Init()
        {
            SetUpCloseAction();

            mImgFade = gameObject.FindComponent<Image>("Image_Fade");
            mImgFade.raycastTarget = false;

            InitButtonActions();

            SoundPlayer.PlayMiniGameHomeBGM();

            MLand.Lobby.SetLockLobbyAction(true);
        }

        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            base.Close(immediate, hideMotion);

            SoundPlayer.PlayLobbyBGM();

            MLand.CameraManager.ResetFollowInfo();

            MLand.Lobby.SetLockLobbyAction(false);
        }

        void InitButtonActions()
        {
            Button buttonMTYT = gameObject.FindComponent<Button>("Button_ElementalCourses");
            Button buttonTicTacToe = gameObject.FindComponent<Button>("Button_TicTacToe");

            buttonMTYT.SetButtonAction(() =>
            {
                FadeOut(OnMTYTButtonAction);

                SoundPlayer.PlayMiniGameElementalCourses_ReadyBGM();
            });
            buttonTicTacToe.SetButtonAction(() =>
            {
                FadeOut(OnTicTacToeButtonAction);

                SoundPlayer.PlayMiniGameTicTacToeBGM();
            });

            Image imgButtonMTYT = buttonMTYT.GetComponent<Image>();
            imgButtonMTYT.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_ElementalCourses_Start_{MLand.SavePoint.LangCode}");
            Image imgButtonTicTacToe = buttonTicTacToe.GetComponent<Image>();
            imgButtonTicTacToe.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_TicTacToe_Start_{MLand.SavePoint.LangCode}");
        }

        void OnMTYTButtonAction()
        {
            var miniGamePopup = MLand.PopupManager.CreatePopup<Popup_MiniGame_ElementalCoursesUI>(showMotion:false);

            miniGamePopup.Init();

            miniGamePopup.SetOnCloseAction(() => FadeIn(duration: 1f));
        }

        void OnTicTacToeButtonAction()
        {
            var miniGamePopup = MLand.PopupManager.CreatePopup<Popup_MiniGame_TicTacToeUI>(showMotion:false);

            miniGamePopup.Init();

            miniGamePopup.SetOnCloseAction(() => FadeIn(duration:1f));
        }

        // ¼­¼­È÷ ¹à¾ÆÁü
        public void FadeIn(float duration)
        {
            mImgFade.raycastTarget = true;

            Fade(0f, duration, onComplete: () => mImgFade.raycastTarget = false);
        }

        // ¼­¼­È÷ ¾îµÎ¿öÁü
        void FadeOut(Action onComplete)
        {
            mImgFade.raycastTarget = true;

            Fade(1f, onComplete:OnComplete);

            void OnComplete()
            {
                onComplete?.Invoke();

                mImgFade.raycastTarget = false;
            }
        }

        void Fade(float endValue, float duration = 0.5f, Action onComplete = null)
        {
            mImgFade.DOFade(endValue, duration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}


