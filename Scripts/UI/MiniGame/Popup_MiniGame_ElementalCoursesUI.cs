using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace MLand
{
    class Popup_MiniGame_ElementalCoursesUI : Popup_MiniGameBase
    {
        float mPenaltyTime;
        Image mImgPenaltyBlock;
        TextMeshProUGUI mTextHighScore;
        
        GameObject mCountDownObj;
        TextMeshProUGUI mCountDownText;

        ElementalCourses_ScoreManager mScoreManager;
        ElementalCourses_FeverManager mFeverManager;
        ElementalCourses_TimeMananger mTimeManager;
        ElementalCourses_TeamManager mTeamManager;

        bool InPenalty => mPenaltyTime > 0f;
        public void Init()
        {
            Init(MiniGameType.ElementalCourses);

            mImgPenaltyBlock = gameObject.FindComponent<Image>("Image_Penalty_Block");
            mImgPenaltyBlock.gameObject.SetActive(false);

            mScoreManager = new ElementalCourses_ScoreManager();
            mScoreManager.Init(gameObject);

            mFeverManager = new ElementalCourses_FeverManager();
            mFeverManager.Init(this);

            mTimeManager = new ElementalCourses_TimeMananger();
            mTimeManager.Init(this);

            mTeamManager = new ElementalCourses_TeamManager();
            mTeamManager.Init(this);

            mCountDownObj = gameObject.FindGameObject("CountDown");
            mCountDownText = mCountDownObj.FindComponent<TextMeshProUGUI>("Text_CountDown");

            mTextHighScore = gameObject.FindComponent<TextMeshProUGUI>("Text_HighScoreNum");

            RefreshMainHighScoreNum();
        }

        public void OnLevelUp()
        {
            mTeamManager.OnLevelUp();
        }

        public override void OnPause()
        {
            base.OnPause();

            SoundPlayer.PlayMiniGameElementalCourses_ReadyBGM();
        }

        public override void OnUnPause()
        {
            StartCoroutine(PlayGameStartMotion(base.OnUnPause));
        }

        public void OnElementalButtonAction(bool isLeft)
        {
            bool isSuccess = mTeamManager.ClassifyTeam(isLeft);
            if (isSuccess)
            {
                SoundPlayer.PlayElementalCoursesCollect(mFeverManager.InFever);

                mFeverManager.StackFeverGauge();

                mScoreManager.SuccessClassfiy();
            }
            else
            {
                mScoreManager.FailClassfiy();

                StartPenalty();
            }
        }

        public void BroadCastFeverStart()
        {
            mTimeManager.BroadCastFeverStart();
            mTeamManager.BroadCastFeverStart();
        }

        public void BroadCastFeverEnd()
        {
            mTimeManager.BroadCastFeverEnd();
            mTeamManager.BroadCastFeverEnd();
        }

        public void RefreshMainHighScoreNum()
        {
            int highScore = MLand.SavePoint.SavedMTYTHighScore;

            mTextHighScore.text = $"{highScore:D3}";
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (mIsPlay == false || mIsPause)
                return;

            mFeverManager.OnUpdate(dt);
            mTimeManager.OnUpdate(dt);

            if (InPenalty)
            {
                mPenaltyTime -= dt;
                if (mPenaltyTime <= 0)
                {
                    EndPenalty();
                }
            }
        }

        public override void EndGame()
        {
            mIsPlay = false;

            StartCoroutine(PlayGameEndMotion());
        }

        public override int GetScore()
        {
            return mScoreManager.Score;
        }

        public override void OnStart()
        {
            base.OnStart();

            mFeverManager.Reset();
            mScoreManager.Reset();
            mTimeManager.Reset();
            mTeamManager.Reset();

            MLand.SavePoint.CheckQuests(QuestType.PlayAnyMiniGame);

            StartCoroutine(PlayGameStartMotion());
        }

        void StartPenalty()
        {
            if (InPenalty == false)
            {
                mPenaltyTime = MLand.GameData.MiniGameCommonData.penaltyTime;

                transform.DORewind();
                transform.DOShakePosition(1f, Vector3.one * 20f, vibrato: 25);

                mImgPenaltyBlock.gameObject.SetActive(true);

                mTeamManager.BroadCastPenaltyStart();

                SoundPlayer.PlayElementalCoursesWrong();
            }
        }

        void EndPenalty()
        {
            mPenaltyTime = 0f;

            mImgPenaltyBlock.gameObject.SetActive(false);

            mTeamManager.BroadCastPenaltyEnd();
        }

        IEnumerator PlayGameStartMotion(Action onStart = null)
        {
            // 카운트 다운을 시작한다.
            mCountDownObj.SetActive(true);

            // 3초
            SoundPlayer.PlayCountDown();
            yield return PlayCountDownMotion($"{3}");
            // 2초
            SoundPlayer.PlayCountDown();
            yield return PlayCountDownMotion($"{2}");
            // 1초
            SoundPlayer.PlayCountDown();
            yield return PlayCountDownMotion($"{1}");

            // GameStart!!
            SoundPlayer.PlayAirhorn();
            yield return PlayCountDownMotion("Game\nStart!!!");

            // 카운트 다운 종료
            mCountDownObj.SetActive(false);

            SoundPlayer.PlayMiniGameElementalCourses_PlayBGM();

            // 게임 시작!
            mIsPlay = true;
            mIsPause = false;

            onStart?.Invoke();
        }

        IEnumerator PlayCountDownMotion(string text)
        {
            mCountDownText.text = text;
            mCountDownText.transform.DORewind();
            mCountDownText.transform.localScale = Vector3.one * 5f;
            mCountDownText.transform.DOScale(Vector3.one, 0.3f)
                .SetAutoKill(false);

            yield return new WaitForSeconds(1f);
        }

        IEnumerator PlayGameEndMotion()
        {
            // 터치하지 못하게 막아주자
            mImgPenaltyBlock.gameObject.SetActive(true);

            SoundPlayer.PlaySchoolBell();

            yield return new WaitForSeconds(2f);

            base.EndGame();

            // 터치할 수 있게 다시 풀어주자
            mImgPenaltyBlock.gameObject.SetActive(false);

            SoundPlayer.PlayMiniGameElementalCourses_ReadyBGM();

            SoundPlayer.PlayElementalCoursesEnd();

            var rewardObj = mGameEnd.FindGameObject("RewardInfo");

            rewardObj.SetActive(false);

            Image imageNew = mGameEnd.FindComponent<Image>("Image_New");

            imageNew.gameObject.SetActive(false);

            int score = GetScore();

            TextMeshProUGUI textHighScore = mGameEnd.FindComponent<TextMeshProUGUI>("Text_HighScoreNum");
            textHighScore.text = $"{MLand.SavePoint.SavedMTYTHighScore}";

            TextMeshProUGUI textEndScore = mGameEnd.FindComponent<TextMeshProUGUI>("Text_EndScoreNum");
            textEndScore.DOCounter(0, score, 1f);

            yield return new WaitForSeconds(1f);

            bool isNewRecord = score > MLand.SavePoint.SavedMTYTHighScore;
            if (isNewRecord)
            {
                imageNew.gameObject.SetActive(true);
                imageNew.transform.DORewind();
                imageNew.transform.DOScale(Vector3.one, 0.3f).SetAutoKill(false);

                textHighScore.DOCounter(MLand.SavePoint.SavedMTYTHighScore, score, 0.3f);

                MLand.SavePoint.SavedMTYTHighScore = score;
                MLand.SavePoint.Save();

                RefreshMainHighScoreNum();
            }

            RewardData reward = DataUtil.GetMiniGameRewardData(score);
            if (reward != null)
            {
                rewardObj.SetActive(true);
                rewardObj.transform.DORewind();
                rewardObj.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, elasticity: 0.25f);

                ItemInfo itemInfo = ItemInfo.CreateRewardInfo(reward);

                Button buttonReward = rewardObj.FindComponent<Button>("Button_Reward");
                buttonReward.SetButtonAction(() => MonsterLandUtil.ShowDescPopup(itemInfo));

                Image imgReward = rewardObj.FindComponent<Image>("Image_Reward");
                imgReward.sprite = itemInfo.GetIconImg();

                TextMeshProUGUI textRewardAmount = rewardObj.FindComponent<TextMeshProUGUI>("Text_Amount");

                StringParam param = new StringParam("amount", itemInfo.GetAmountString());

                string amountText = StringTableUtil.Get("UIString_Amount", param);

                textRewardAmount.text = amountText;
            }

            // 우등생 업적 확인
            MLand.SavePoint.CheckAchievements(AchievementsType.HonorStudent, score);
        }
    }
}