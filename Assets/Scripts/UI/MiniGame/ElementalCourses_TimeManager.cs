using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class ElementalCourses_TimeMananger
    {
        bool mInFever;
        float mTime;
        TimeGaugeBarUI mTimeGaugeBarUI;
        Popup_MiniGame_ElementalCoursesUI mParent;
        float MaxTime => MLand.GameData.MiniGameElementalCoursesData.playTime;
        float Level2CheckTime => MLand.GameData.MiniGameElementalCoursesData.level2CheckTime;
        float Level3CheckTime => MLand.GameData.MiniGameElementalCoursesData.level3CheckTime;
        public void Init(Popup_MiniGame_ElementalCoursesUI parent)
        {
            mParent = parent;

            var timeGaugeBarObj = parent.gameObject.FindGameObject("TimeGaugeBar");

            mTimeGaugeBarUI = timeGaugeBarObj.GetOrAddComponent<TimeGaugeBarUI>();
            mTimeGaugeBarUI.Init();
            
            Reset();
        }

        public void Reset()
        {
            mInFever = false;

            mTime = MaxTime;

            mTimeGaugeBarUI.OnReset(1f);
        }

        public void OnUpdate(float dt)
        {
            if (mInFever)
                return;

            CheckLevelUp(dt);

            float prevTime = mTime;
            mTime -= dt;
            if (mTime <= 0f)
            {
                mTime = 0f;

                mTimeGaugeBarUI.OnEndGame();

                mParent.EndGame();
            }
            else if (prevTime > MLand.GameData.MiniGameElementalCoursesData.timeLimit
                && mTime <= MLand.GameData.MiniGameElementalCoursesData.timeLimit)
            {
                mTimeGaugeBarUI.StartTimeLimit();
            }

            mTimeGaugeBarUI.SetFillValue(mTime / MaxTime);
        }

        void CheckLevelUp(float dt)
        {
            if ( mTime > Level2CheckTime && mTime - dt < Level2CheckTime )
            {
                mParent.OnLevelUp();
            }
            else if ( mTime > Level3CheckTime && mTime - dt < Level3CheckTime )
            {
                mParent.OnLevelUp();
            }
        }

        public void BroadCastFeverStart()
        {
            mInFever = true;
        }

        public void BroadCastFeverEnd()
        {
            mInFever = false;
        }
    }
}