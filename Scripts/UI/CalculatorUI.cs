using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using DG.Tweening;

namespace MLand
{
    enum CalculatorButton
    {
        Number_Zero, Number_One, Number_Two,
        Number_Three, Number_Four, Number_Five,
        Number_Six, Number_Seaven, Number_Eight,
        Number_Nine, Clear, Enter,

        Count
    }

    class CalculatorUI : MonoBehaviour
    {
        UnityAction<double> mEnterButtonAction;
        Func<double> mGetNumberMaxValueFunc;

        CanvasGroup mCanvasGroup;
        TextMeshProUGUI mText_Numbers;
        public void Init()
        {
            mEnterButtonAction = null;
            mGetNumberMaxValueFunc = null;
            mCanvasGroup = GetComponentInChildren<CanvasGroup>();

            mText_Numbers = gameObject.FindComponent<TextMeshProUGUI>("Text_Number");

            InitButtonActions();

            SetActive(false);
        }

        void InitButtonActions()
        {
            for(int i = 0; i < (int)CalculatorButton.Count; ++i)
            {
                CalculatorButton calcButton = (CalculatorButton)i;

                string buttonName = $"Button_{calcButton}";

                Button button = gameObject.FindComponent<Button>(buttonName);

                button.SetButtonAction(GetButtonAction(calcButton));
            }

            var buttonModal = gameObject.FindComponent<Button>("Button_Modal");
            buttonModal.SetButtonAction(Hide);
        }

        UnityAction GetButtonAction(CalculatorButton calcButton)
        {
            switch (calcButton)
            {
                case CalculatorButton.Number_Zero:
                case CalculatorButton.Number_One:
                case CalculatorButton.Number_Two:
                case CalculatorButton.Number_Three:
                case CalculatorButton.Number_Four:
                case CalculatorButton.Number_Five:
                case CalculatorButton.Number_Six:
                case CalculatorButton.Number_Seaven:
                case CalculatorButton.Number_Eight:
                case CalculatorButton.Number_Nine:
                    return () => AddNumberStr((int)calcButton);
                case CalculatorButton.Clear:
                    return RemoveLastNumberStr;
                case CalculatorButton.Enter:
                default:
                    return EnterNumber;
            }
        }

        void AddNumberStr(int inputNumber)
        {
            string numberText = mText_Numbers.text;

            double numberMaxValue = mGetNumberMaxValueFunc.Invoke();

            if ( double.TryParse(numberText, out double result) )
            {
                if ( result >= numberMaxValue )
                {
                    mText_Numbers.text = numberMaxValue.ToString();
                }
                else
                {
                    numberText = $"{numberText}{inputNumber}";

                    if ( double.TryParse(numberText, out double result2) )
                    {
                        if ( result2 >= numberMaxValue )
                        {
                            mText_Numbers.text = numberMaxValue.ToString();
                        }
                        else
                        {
                            mText_Numbers.text = result2.ToString();
                        }
                    }
                    else
                    {
                        Debug.LogError("계산기에서 숫자가 아닌것을 스트링으로 변경하려고 시도함");
                    }
                }
            }
            else
            {
                Debug.LogError("계산기에서 숫자가 아닌것을 스트링으로 변경하려고 시도함");
            }
        }

        void RemoveLastNumberStr()
        {
            // 마지막 숫자 삭제
            string numberText = mText_Numbers.text;
            if (numberText.IsValid() && numberText.Length > 0)
            {
                numberText = numberText.Substring(0, numberText.Length - 1);

                if (numberText.IsValid() == false)
                    numberText = "0";

                mText_Numbers.text = numberText;
            }
        }

        void EnterNumber()
        {
            if ( double.TryParse(mText_Numbers.text, out double result) )
            {
                mEnterButtonAction.Invoke(result);

                Hide();
            }
            else
            {
                Debug.LogError("계산기에서 숫자가 아닌것을 스트링으로 변경하려고 시도함");
            }
        }

        Sequence mShowSequence;
        public void Show(UnityAction<double> onEnterButtonAction, Func<double> getNumberMaxValueFunc)
        {
            mEnterButtonAction = onEnterButtonAction;
            mGetNumberMaxValueFunc = getNumberMaxValueFunc;

            mText_Numbers.text = "0";

            SetActive(true);

            float endValue = 0f;

            RectTransform tm = GetComponent<RectTransform>();

            mShowSequence?.Rewind();
            mShowSequence = DOTween.Sequence()
                .Join(tm.DOAnchorPosX(endValue, 0.25f))
                .Join(DOTween.To((f) => mCanvasGroup.alpha = f, 0f, 1f, 0.25f))
                .SetAutoKill(false);
        }

        Sequence mHideSequence;
        void Hide()
        {
            float endValue = 500;

            RectTransform tm = GetComponent<RectTransform>();

            mHideSequence?.Rewind();
            mHideSequence = DOTween.Sequence()
                .Join(tm.DOAnchorPosX(endValue, 0.25f))
                .Join(DOTween.To((f) => mCanvasGroup.alpha = f, 1f, 0f, 0.25f))
                .SetAutoKill(false)
                .OnComplete(() => SetActive(false));
        }

        void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}