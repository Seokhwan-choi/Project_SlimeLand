using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Loading : MonoBehaviour
    {
        public Slider ProgressBar;
        public Button ButtonTouchAnyScreen;
        public TextMeshProUGUI TextPercent;
        public TextMeshProUGUI TextTouchAnyScreen;

        CanvasGroup mCanvasGroup;
        GameObject mKingSlime;

        float mWaitTime;

        private void Start()
        {
            mWaitTime = 1.5f;
            mCanvasGroup = GetComponent<CanvasGroup>();
            mKingSlime = gameObject.FindGameObject("Title_SlimeKing");

            InitLangCode();

            gameObject.Localize();

            TextTouchAnyScreen.text = StringTableUtil.Get("UIString_AnyTouchScreen");

            MLand.Lobby.SetActiveIntroCloud();

            ButtonTouchAnyScreen.SetButtonAction(() =>
            {
                if (mWaitTime >= 0f)
                    return;

                mKingSlime.transform.DOPunchScale(Vector3.one * 0.25f, 0.5f, elasticity: 0.2f);

                SoundPlayer.PlayMiniGame_TicTacToe_Finish();

                TextTouchAnyScreen.gameObject.SetActive(false);
                ButtonTouchAnyScreen.gameObject.SetActive(false);
                ProgressBar.gameObject.SetActive(true);

                StartCoroutine(LoadScene());
            });

#if !UNITY_EDITOR
            var inappUpdateManager = new InAppUpdateManager();
            inappUpdateManager.Init();

            StartCoroutine(inappUpdateManager.CheckForUpdate());
#endif
        }

        void InitLangCode()
        {
#if UNITY_EDITOR
            if (MLand.SavePoint.LangCodeStr.IsValid() == false)
            {
                //MLand.SavePoint.LangCodeStr = $"{LangCode.KR}";
                //MLand.SavePoint.LangCode = LangCode.JP;
                //MLand.SavePoint.Save();
            }

#else
            if (MLand.SavePoint.LangCodeStr.IsValid() == false)
            {
                MLand.SavePoint.LangCodeStr = $"{Application.systemLanguage}";

                if (Application.systemLanguage == SystemLanguage.Japanese)
                {
                    MLand.SavePoint.LangCode = LangCode.JP;
                }
                else if (Application.systemLanguage == SystemLanguage.Chinese ||
                         Application.systemLanguage == SystemLanguage.ChineseSimplified)
                {
                    MLand.SavePoint.LangCode = LangCode.CN;
                }
                else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
                {
                    MLand.SavePoint.LangCode = LangCode.TW;
                }
                else if (Application.systemLanguage == SystemLanguage.Korean)
                {
                    MLand.SavePoint.LangCode = LangCode.KR;
                }
                else // 나머지 국가는 모두 영어
                {
                    MLand.SavePoint.LangCode = LangCode.US;
                }

                MLand.SavePoint.Save();
            }

            if (MLand.SavePoint.LangCode == LangCode.CN || MLand.SavePoint.LangCode == LangCode.TW)
            {
                MLand.SavePoint.LangCode = LangCode.US;
                MLand.SavePoint.Save();
            }
#endif
        }

        private void Update()
        {
            mWaitTime -= Time.deltaTime;
        }

        public void PlaySlimeAppearSound()
        {
            SoundPlayer.PlayTitleSlimeAppear();
        }

        IEnumerator LoadScene()
        {
            yield return null;

            MLand.Firebase.Setup();
            MLand.Atlas.Init();
            MLand.PopupManager.Init();

            yield return MLand.GameManager.Init();
            
            yield return PercentCharge(0.5f);

            MLand.Lobby.Init();

            yield return PercentCharge(0.8f);

            bool waitForLogin = true;

            // 구글 로그인을 잠깐 기다리자
            MLand.GPGSBinder = new GPGSBinder();
            MLand.GPGSBinder.Login(
                (b, success) =>
                {
                    MLand.IAPManager.Init();

                    waitForLogin = false;
                });

            yield return new WaitUntil(() => waitForLogin == false);

            MLand.SavePoint.LastLoginDateNum = TimeUtil.NowDateNum();

            yield return PercentCharge(1f);

            SoundPlayer.PlayLoadingComplete();

            ProgressBar.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, elasticity:0.2f)
                .OnComplete(OnLoadingComplete);

            void OnLoadingComplete()
            {
                DOTween.To(() => mCanvasGroup.alpha, (f) => mCanvasGroup.alpha = f, 0f, 1f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);

                    MLand.Lobby.PlayIntroMotion();
                });
            }
        }

        IEnumerator PercentCharge(float f)
        {
            while (ProgressBar.value < f)
            {
                ProgressBar.value = Mathf.MoveTowards(ProgressBar.value, f, Time.deltaTime);

                TextPercent.text = $"{Mathf.RoundToInt(ProgressBar.value * 100f)}%";

                yield return null;
            }
        }
    }
}


