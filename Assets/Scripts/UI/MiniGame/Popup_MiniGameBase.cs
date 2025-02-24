using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Popup_MiniGameBase : PopupBase
    {
        protected bool mIsPlay;
        protected bool mIsPause;
        protected MiniGameType mType;
        protected GameObject mGameStart;
        protected GameObject mGamePlay;
        protected GameObject mGameEnd;
        protected GameObject mButtons;

        Image mImgFade;
        Button mButtonNormalReward;
        Button mButtonWatchAdReward;
        TextMeshProUGUI mTextChargingTime;

        GameObject mFreeStartObj;
        GameObject mOtherStartObj;
        StartButtonItem mFreeStart;
        StartButtonItem mGemStart;
        StartButtonItem mWatchAdStart;
        public void Init(MiniGameType type)
        {
            mType = type;

            mImgFade = gameObject.FindComponent<Image>("Image_Fade");
            mImgFade.raycastTarget = false;

            mGameStart = gameObject.FindGameObject("GameStart");
            mGamePlay = gameObject.FindGameObject("GamePlay");
            mGameEnd = gameObject.FindGameObject("GameEnd");
            mButtons = gameObject.FindGameObject("Buttons");
            mTextChargingTime = mButtons.FindComponent<TextMeshProUGUI>("Text_FreePlayCountChargingTime");

            mFreeStartObj = mButtons.FindGameObject("FreeStart");
            mOtherStartObj = mButtons.FindGameObject("OtherStart");

            var freeStartObj = mFreeStartObj.FindGameObject("Button_FreeStart");
            mFreeStart = freeStartObj.GetOrAddComponent<StartButtonItem>();
            mFreeStart
                .InitText("Text_RemainFreePlayCount")
                .InitButton("Button_FreeStart", OnFreeStartButtonAction);

            var gemStartObj = mOtherStartObj.FindGameObject("Button_GemStart");
            mGemStart = gemStartObj.GetOrAddComponent<StartButtonItem>();
            mGemStart
                .InitText("Text_GemPrice")
                .InitButton("Button_GemStart", OnGemStartButtonAction);

            var watchAdStartObj = mOtherStartObj.FindGameObject("Button_WatchAd");
            mWatchAdStart = watchAdStartObj.GetOrAddComponent<StartButtonItem>();
            mWatchAdStart
                .InitText("Text_AdRemainCount")
                .InitButton("Button_WatchAd", OnWatchAdStartButtonAction);

            var receiveRewardButton = mGameEnd.FindGameObject("Button_ReceiveRewards");

            mButtonNormalReward = receiveRewardButton.FindComponent<Button>("Button_NormalReward");
            mButtonNormalReward.SetButtonAction(() => OnReceiveRewardButtonAction(watchAd:false));

            mButtonWatchAdReward = receiveRewardButton.FindComponent<Button>("Button_WatchAdReward");
            mButtonWatchAdReward.SetButtonAction(() =>
            {
                var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
                if (removeAdProduct != null)
                {
                    if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                    {
                        OnReceiveRewardButtonAction(watchAd: true);
                    }
                    else
                    {
                        ConfirmWatchAd();
                    }
                }
                else
                {
                    ConfirmWatchAd();
                }

                void ConfirmWatchAd()
                {
                    string title = StringTableUtil.Get("Title_Confirm");

                    StringParam param = new StringParam("bonus", MLand.GameData.MiniGameElementalCoursesData.watchAdBonusValue.ToString());

                    string desc = StringTableUtil.Get("MiniGame_ConfirmWatchAdBonusReward", param);

                    MonsterLandUtil.ShowAdConfirmPopup(title, desc, () => OnReceiveRewardButtonAction(watchAd: true));
                }
            });
            
            var buttonGuide = mButtons.FindComponent<Button>("Button_Guide");
            buttonGuide.SetButtonAction(OnGuideButtonAction);

            var buttonRewardList = mButtons.FindComponent<Button>("Button_RewardList");
            buttonRewardList.SetButtonAction(OnRewardListButtonAction);

            var buttonClose = mGameStart.FindComponent<Button>("Button_Close");
            buttonClose.SetButtonAction(() => Close());

            var buttonPause = mGamePlay.FindComponent<Button>("Button_Pause");
            buttonPause.SetButtonAction(() => Close());

            InitTitleImg(type);

            FadeIn();

            ResetUI();
        }

        void InitTitleImg(MiniGameType type)
        {
            string miniGameTitleName = string.Empty;

            var imgTitle = mGameStart.FindComponent<Image>("Image_Title");

            if (type == MiniGameType.ElementalCourses)
            {
                miniGameTitleName = "Img_Minigame_elementalCourses_Title";
            }
            else
            {
                miniGameTitleName = "Img_Minigame_Tictactoe_Title";
            }

            imgTitle.sprite = MLand.Atlas.GetUISprite($"{miniGameTitleName}_{MLand.SavePoint.LangCode}");
        }

        void ResetUI()
        {
            mGameStart.SetActive(true);
            mGameEnd.SetActive(false);
            mButtons.SetActive(true);

            RefreshStartButtons();
        }

        void RefreshStartButtons()
        {
            bool isMaxPlayCount = MLand.SavePoint.MiniGame.IsMaxFreePlayCount(mType);
            if (isMaxPlayCount)
            {
                mFreeStartObj.SetActive(false);
                mOtherStartObj.SetActive(true);
                mTextChargingTime.gameObject.SetActive(true);

                double gemPrice = mType == MiniGameType.ElementalCourses ?
                    MLand.GameData.MiniGameElementalCoursesData.gemPlayPrice :
                    MLand.GameData.MiniGameTicTacToeData.gemPlayPrice;

                bool isNotEnoughGem = MLand.SavePoint.IsEnoughGem(gemPrice) == false;
                if (isNotEnoughGem)
                {
                    mGemStart.SetText($"<color=red>{gemPrice}</color>");
                }
                else
                {
                    mGemStart.SetText($"{gemPrice}");
                }

                int currentCount = MLand.SavePoint.MiniGame.GetRemainWatchAdCount(mType);
                int maxCount = MLand.SavePoint.MiniGame.GetWatchAdCount(mType);

                bool isNotEnoughWatchAd = MLand.SavePoint.MiniGame.IsMaxWatchAdCount(mType);
                if (isNotEnoughWatchAd)
                {
                    mWatchAdStart.SetText($"<color=red>( {currentCount} / {maxCount} )</color>");
                }
                else
                {
                    mWatchAdStart.SetText($"( {currentCount} / {maxCount} )");
                }
            }
            else
            {
                mFreeStartObj.SetActive(true);
                mOtherStartObj.SetActive(false);
                mTextChargingTime.gameObject.SetActive(false);

                int remainFreePlayCount = MLand.SavePoint.MiniGame.GetRemainFreePlayCount(mType);

                var param = new StringParam("freePlayCount", remainFreePlayCount.ToString());

                mFreeStart.SetText(StringTableUtil.Get("MiniGame_RemainFreePlayCount", param));
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            OnUpdate(dt);
        }

        float mTimeInterval;
        public virtual void OnUpdate(float dt) 
        {
            mTimeInterval -= dt;
            if (mTimeInterval <= 0f)
            {
                mTimeInterval = 1f;

                RefreshFreePlayChargingTimeText();
            }
        }

        void RefreshFreePlayChargingTimeText()
        {
            if (mTextChargingTime.gameObject.activeSelf == false)
                return;

            int freePlayCount = MLand.GameData.MiniGameElementalCoursesData.dailyFreePlayCount;

            int nextDayRemainTime = TimeUtil.RemainSecondsToNextDay();

            string remainTimeStr = TimeUtil.GetTimeStr(nextDayRemainTime);

            StringParam param = new StringParam("time", remainTimeStr);
            param.AddParam("freePlayCount", freePlayCount.ToString());

            var message = StringTableUtil.Get("MiniGame_RemainTimeFreePlayCountCharging", param);

            mTextChargingTime.text = message;
        }

        public virtual void OnStart()
        {
            mGameStart.SetActive(false);
            mGameEnd.SetActive(false);
            mButtons.SetActive(false);

            RefreshStartButtons();
        }

        public virtual void EndGame()
        {
            mGameEnd.SetActive(true);
            mButtons.SetActive(false);

            RefreshStartButtons();
        }

        public virtual void OnFreeStartButtonAction() 
        {
            if (MLand.SavePoint.MiniGame.IsMaxFreePlayCount(mType))
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughMiniGameFreePlayCount");

                return;
            }

            if (MLand.SavePoint.MiniGame.StackCount(mType))
            {
                MLand.SavePoint.Save();
            }

            OnStart();
        }
        public virtual void OnGemStartButtonAction() 
        {
            double priceGem = mType == MiniGameType.ElementalCourses ? 
                MLand.GameData.MiniGameElementalCoursesData.gemPlayPrice :
                MLand.GameData.MiniGameTicTacToeData.gemPlayPrice;

            if (MLand.SavePoint.IsEnoughGem(priceGem) == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGem");

                return;
            }

            string title = StringTableUtil.Get("Title_Confirm");

            StringParam param = new StringParam("gem", priceGem.ToString());
            string desc = StringTableUtil.Get("Confirm_UseGem", param);

            MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);

            void OnConfirm()
            {
                if (MLand.GameManager.UseGem(priceGem))
                {
                    OnStart();
                }
            }
        }
        public virtual void OnWatchAdStartButtonAction() 
        {
            if (MLand.SavePoint.MiniGame.StackWatchAdCount(mType))
            {
                var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
                if (removeAdProduct != null)
                {
                    if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                    {
                        OnStart();
                    }
                    else
                    {
                        ConfirmWathAd();
                    }
                }
                else
                {
                    ConfirmWathAd();
                }
                
                void ConfirmWathAd()
                {
                    string title = StringTableUtil.Get("Title_Confirm");
                    string desc = StringTableUtil.Get("Confirm_WatchAdAndPlayMiniGame");

                    MonsterLandUtil.ShowAdConfirmPopup(title, desc, OnStart);
                }
            }
            else
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughMiniGameFreePlayCount");
            }
        }
        public virtual void OnReceiveRewardButtonAction(bool watchAd) 
        {
            RewardData rewardData = DataUtil.GetMiniGameRewardData(GetScore());
            if (rewardData == null)
            {
                // 정상적인 상황에서 다음과 같은 경우는 없을 것이다..
                SoundPlayer.PlayErrorSound();

                return;
            }

            int bonusValue = watchAd ? MLand.GameData.MiniGameElementalCoursesData.watchAdBonusValue : 1;

            if (bonusValue > 1)
            {
                RewardData newRewardData = new RewardData()
                {
                    gemReward = rewardData.gemReward * bonusValue,
                    goldReward = rewardData.goldReward * bonusValue,
                    friendShipReward = rewardData.friendShipReward,
                    friendShipRewardCount = rewardData.friendShipRewardCount * bonusValue,
                    boxReward = rewardData.boxReward,
                    boxRewardCount = rewardData.boxRewardCount * bonusValue,
                };

                BoxOpenResult[] boxResults = MLand.GameManager.ReceiveReward(newRewardData);
                if (boxResults != null && newRewardData.boxReward.IsValid())
                {
                    var boxData = MLand.GameData.BoxData.TryGet(newRewardData.boxReward);
                    if ( boxData != null )
                    {
                        BuyBoxUtil.ShowBoxResult(string.Empty, BoxOpenType.Reward, boxData.id, boxResults);
                    }
                }
                else
                {
                    MonsterLandUtil.ShowRewardPopup(newRewardData);
                }
            }
            else
            {
                BoxOpenResult[] boxResults = MLand.GameManager.ReceiveReward(rewardData);
                if (boxResults != null && rewardData.boxReward.IsValid())
                {
                    var boxData = MLand.GameData.BoxData.TryGet(rewardData.boxReward);
                    if (boxData != null)
                    {
                        BuyBoxUtil.ShowBoxResult(string.Empty, BoxOpenType.Reward, boxData.id, boxResults);
                    }
                }
                else
                {
                    MonsterLandUtil.ShowRewardPopup(rewardData);
                }
            }

            ResetUI();
        }
        public virtual void OnGuideButtonAction() 
        {
            Popup_MiniGameGuideUI popupGuide = MLand.PopupManager.CreatePopup<Popup_MiniGameGuideUI>();

            popupGuide.Init(mType);
        }

        public virtual void OnRewardListButtonAction()
        {
            Popup_MiniGameRewardListUI popupRewardList = MLand.PopupManager.CreatePopup<Popup_MiniGameRewardListUI>();

            popupRewardList.Init(mType);
        }

        public virtual int GetScore() { return 0; }
        public virtual void OnPause() { mIsPause = true; }
        public virtual void OnUnPause() { mIsPause = false; }
        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            if (mIsPlay)
            {
                if (mIsPause == false)
                {
                    var title = StringTableUtil.Get("Title_Confirm");
                    var desc = StringTableUtil.Get("Confirm_EndMiniGame");

                    OnPause();

                    Popup_ConfirmUI popup = MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, OnUnPause);

                    popup.SetOnCloseAction(OnUnPause);

                    void OnConfirm()
                    {
                        FadeOut(OnClose);
                    }
                }

                return;
            }

            FadeOut(OnClose);

            void OnClose()
            {
                base.Close(true, true);

                SoundPlayer.PlayMiniGameHomeBGM();
            }
        }

        // 서서히 밝아짐
        void FadeIn()
        {
            mImgFade.raycastTarget = true;

            Fade(1f, 0f, onComplete: () => mImgFade.raycastTarget = false);
        }

        // 서서히 어두워짐
        void FadeOut(Action onComplete)
        {
            mImgFade.raycastTarget = true;

            Fade(0f, 1f, onComplete: OnComplete);

            void OnComplete()
            {
                onComplete?.Invoke();

                mImgFade.raycastTarget = false;
            }
        }

        void Fade(float startValue, float endValue, float duration = 0.5f, Action onComplete = null)
        {
            Color startColor = Color.black;
            startColor.a = startValue;

            mImgFade.color = startColor;

            mImgFade.DOFade(endValue, duration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }

    class StartButtonItem : MonoBehaviour
    {
        Button mButton;
        TextMeshProUGUI mText;

        public StartButtonItem InitText(string textName)
        {
            mText = gameObject.FindComponent<TextMeshProUGUI>(textName);

            return this;
        }

        public StartButtonItem InitButton(string buttonName, Action onButtonAction)
        {
            mButton = gameObject.FindComponent<Button>(buttonName);
            mButton.SetButtonAction(() => onButtonAction?.Invoke());

            return this;
        }

        public void SetText(string text)
        {
            mText.text = text;
        }
    }
}


