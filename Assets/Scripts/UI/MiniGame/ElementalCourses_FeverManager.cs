using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class ElementalCourses_FeverManager
    {
        GameObject mFeverText;
        GameObject mFeverBackground;
        FeverGaugeBarUI mFeverGaugeUI;
        Popup_MiniGame_ElementalCoursesUI mParent;

        float mFeverTime;
        float mFeverGauge;
        public bool InFever => mFeverTime > 0;
        public void Init(Popup_MiniGame_ElementalCoursesUI parent)
        {
            mParent = parent;

            GameObject parentObj = parent.gameObject;

            mFeverText = parentObj.FindGameObject("FeverText");
            mFeverBackground = parentObj.FindGameObject("FeverBackground");

            var feverGaugeBarObj = parentObj.FindGameObject("FeverGaugeBar");
            mFeverGaugeUI = feverGaugeBarObj.GetOrAddComponent<FeverGaugeBarUI>();
            mFeverGaugeUI.Init();

            Reset();
        }

        public void Reset()
        {
            mFeverTime = 0f;
            mFeverGauge = 0f;

            mFeverGaugeUI.SetFillValue(0f);
        }

        public void OnUpdate(float dt)
        {
            if (InFever)
            {
                mFeverTime -= dt;
                if (mFeverTime <= 0f)
                {
                    EndFever();
                }

                mFeverGaugeUI.SetFillValue(mFeverTime / MLand.GameData.MiniGameElementalCoursesData.feverTime);
            }
        }

        public void StackFeverGauge()
        {
            if (InFever)
                return;

            mFeverGauge += 1;
            if (mFeverGauge >= MLand.GameData.MiniGameElementalCoursesData.feverCount)
            {
                StartFever();
            }

            mFeverGaugeUI.SetFillValue(mFeverGauge / MLand.GameData.MiniGameElementalCoursesData.feverCount);
        }

        void StartFever()
        {
            SoundPlayer.PlayStartFever();

            mParent.BroadCastFeverStart();

            mFeverBackground.SetActive(true);
            mFeverText.SetActive(true);

            mFeverGauge = 0;

            mFeverTime = MLand.GameData.MiniGameElementalCoursesData.feverTime;
        }

        void EndFever()
        {
            mParent.BroadCastFeverEnd();

            mFeverBackground.SetActive(false);
            mFeverText.SetActive(false);
        }
    }
}