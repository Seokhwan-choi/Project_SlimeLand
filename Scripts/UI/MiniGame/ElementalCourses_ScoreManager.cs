using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class ElementalCourses_ScoreManager
    {
        int mCombo;
        int mScore;
        Text mComboText;    // 비트맵 폰트는 TMPro에서 바로 사용이 안되네..
        Image mImgNew;
        TextMeshProUGUI mScoreText;
        public int Score => mScore;
        public void Init(GameObject parent)
        {
            mImgNew = parent.FindComponent<Image>("Image_New");
            mImgNew.gameObject.SetActive(false);

            mScoreText = parent.FindComponent<TextMeshProUGUI>("Text_ScoreNum");
            mComboText = parent.FindComponent<Text>("Text_ComboNum");

            Reset();
        }

        public void Reset()
        {
            mCombo = 0;
            mScore = 0;

            mImgNew.gameObject.SetActive(false);

            RefreshUI();
        }

        void RefreshUI()
        {
            SetComboText();
            SetScoreText();
        }

        void SetComboText()
        {
            // ex) 001 과 같이 표기
            mComboText.text = $"{mCombo:D3}";
        }

        void SetScoreText()
        {
            // ex) 001 과 같이 표기 
            mScoreText.text = $"{mScore:D3}";
        }

        public void SuccessClassfiy()
        {
            AddScore();

            StackCombo(success: true);

            RefreshUI();
        }

        public void FailClassfiy()
        {
            StackCombo(success: false);

            RefreshUI();
        }

        public void StackCombo(bool success)
        {
            if (success)
            {
                mCombo += 1;

                mComboText.transform.DORewind();
                mComboText.transform
                    .DOPunchScale(Vector3.one * 0.25f, 0.3f, elasticity: 0.2f)
                    .SetAutoKill(false);
            }
            else
            {
                mCombo = 0;
            }
        }

        public void AddScore()
        {
            mScore += 1;

            CheckNewRecord();

            mScoreText.transform.DORewind();
            mScoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, elasticity: 0.2f);
        }

        void CheckNewRecord()
        {
            if (mScore >= MLand.SavePoint.SavedMTYTHighScore)
            {
                if (mImgNew.gameObject.activeSelf == false)
                {
                    mImgNew.gameObject.SetActive(true);

                    mImgNew.transform.localScale = Vector3.one * 3f;
                    mImgNew.transform.DOScale(Vector3.one, 0.5f);
                }
            }
        }
    }
}