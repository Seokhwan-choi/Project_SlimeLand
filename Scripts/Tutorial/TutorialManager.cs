using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class TutorialManager
    {
        Action mOnEndTutorial;

        GameObject mTutorialObj;
        GameObject mSpeechObj;
        GameObject mHighlightObj;
        GameObject mTouchFingerObj;

        Image mImgCharacter;
        Button mButtonSpeechModalTouch;
        Button mButtonHighlightTouch;
        TextMeshProUGUI mTextSpeech;
        TextMeshProUGUI mTextTouchAnyScreen;

        string mCurrentTutoralId;
        TutorialAction mCurrentTutorial;
        List<TutorialAction> mCurrentTutorialStepList;
        public List<TutorialAction> CurrentTutorialStepList => mCurrentTutorialStepList;
        public string CurrentTutorialId => mCurrentTutoralId;
        public bool InTutorial => mCurrentTutorial != null || mCurrentTutorialStepList.Count > 0;
        public void Init(LobbyUI lobby)
        {
            mCurrentTutorialStepList = new List<TutorialAction>();

            mTutorialObj = lobby.Find("Tutorial");
            mSpeechObj = mTutorialObj.FindGameObject("Speech");
            mHighlightObj = mTutorialObj.FindGameObject("Highlight");
            mTouchFingerObj = mHighlightObj.FindGameObject("TouchFinger");
            mTextSpeech = mSpeechObj.FindComponent<TextMeshProUGUI>("Text_SpeechMessage");
            mTextTouchAnyScreen = mSpeechObj.FindComponent<TextMeshProUGUI>("Text_TouchAnyScreen");
            mButtonSpeechModalTouch = mSpeechObj.FindComponent<Button>("Button_Modal");
            mImgCharacter = mSpeechObj.FindComponent<Image>("Image_Character");
            mButtonHighlightTouch = mHighlightObj.FindComponent<Button>("Button_HighLightTouch");
        }

        public void StartTutorial(string id, Action onEndTutorial)
        {
            if (Enum.TryParse(id, out SavePointBitFlags flag))
            {
                // 이미 진행했던 튜토리얼이면 무시하자
                if (flag.IsOn())
                {
                    onEndTutorial?.Invoke();
                    return;
                }
                else if (flag == SavePointBitFlags.Tutorial_1_SpawnSlime)
                {
                    // 첫번째 슬라임이 이미 소환되어 있다면 무시
                    if (MLand.SavePoint.SlimeManager.IsUnlockedSlime("Slime_Water_1"))
                    {
                        onEndTutorial?.Invoke();
                        return;
                    }
                }
                else if (flag == SavePointBitFlags.Tutorial_2_BuildBuilding)
                {
                    // 첫번째 건물이 이미 지어져 있다면 무시
                    if (MLand.SavePoint.BuildingManager.IsUnlockedBuilding("Building_Water_1"))
                    {
                        onEndTutorial?.Invoke();
                        return;
                    }
                }
                else if (flag == SavePointBitFlags.Tutorial_6_GoldSlime)
                {
                    if (MLand.GameManager.IsSpwanedGoldSlime() == false)
                    {
                        onEndTutorial?.Invoke();
                        return;
                    }
                }

                mOnEndTutorial = onEndTutorial;

                mTutorialObj.SetActive(true);

                mCurrentTutoralId = id;

                IEnumerable<TutorialData> datas = MLand.GameData.TutorialData.Where(x => x.id == id);

                // 순서대로 동작해야 되기때문에 혹시 모르니 정렬해주자
                datas = datas.OrderBy(x => x.step);

                // 정상적으로 튜토리얼을 진행했다면 남은게 없겠지만
                // 혹시 모르니 밀어주자
                mCurrentTutorialStepList.Clear();

                foreach (TutorialData data in datas)
                {
                    mCurrentTutorialStepList.Add(TutorialActionCreater.Create(this, data.actionType, data.step, data.waitTime));
                }
            }
            else
            {
                Debug.LogError($"Id : {id}, 잘 못된 튜토리얼 시작을 시도함");

                SoundPlayer.PlayErrorSound();
            }
        }

        public void OnUpdate(float dt)
        {
            if (mCurrentTutorial != null )
            {
                mCurrentTutorial.OnUpdate(dt);

                if (mCurrentTutorial.IsFinish)
                {
                    mCurrentTutorial.OnFinish();
                    mCurrentTutorial = null;

                    if (mCurrentTutorialStepList.Count <= 0)
                    {
                        EndTutorial();
                    }
                }
            }
            else
            {
                mCurrentTutorial = PopupTutorial();
                if (mCurrentTutorial != null)
                {
                    MLand.GameManager.StartTouchBlock(100f);

                    mCurrentTutorial.OnStart();
                }
            }
        }

        TutorialAction PopupTutorial()
        {
            if (mCurrentTutorialStepList.Count > 0)
            {
                var first = mCurrentTutorialStepList.FirstOrDefault();

                mCurrentTutorialStepList.RemoveAt(0);

                 return first;
            }

            return null;
        }

        void EndTutorial()
        {
            if (mCurrentTutoralId.IsValid())
            {
                if (Enum.TryParse(mCurrentTutoralId, out SavePointBitFlags flag))
                {
                    flag.Set(true);

                    mCurrentTutoralId = string.Empty;

                    mTutorialObj.SetActive(false);

                    mOnEndTutorial?.Invoke();

                    MLand.GameManager.EndTouchBlock();
                }
            }
        }

        public void ShowHighlight(Vector3 pos, Action onHighlightTouchAction)
        {
            ShowHighlight(onHighlightTouchAction);

            SetHighlightObjPos(pos);
        }

        public void ShowHighlight(RectTransform rectTm, Action onHighlightTouchAction)
        {
            ShowHighlight(onHighlightTouchAction);

            SetHighlightObjPos(rectTm);
        }

        void ShowHighlight(Action onHighlightTouchAction)
        {
            mSpeechObj.SetActive(false);
            if (mHighlightObj.activeSelf == false)
            {
                mHighlightObj.SetActive(true);

                foreach (var tween in mHighlightObj.GetComponentsInChildren<DOTweenAnimation>())
                {
                    tween.DORewind();
                    tween.DOPlay();
                }
            }

            mTouchFingerObj.SetActive(onHighlightTouchAction != null);
            mButtonHighlightTouch.SetButtonAction(() => onHighlightTouchAction?.Invoke());
        }

        public void SetHighlightObjPos(Vector3 pos)
        {
            mHighlightObj.transform.position = pos;
        }

        void SetHighlightObjPos(RectTransform rectTm)
        {
            RectTransform tm = mHighlightObj.GetComponent<RectTransform>();

            tm.position = rectTm.position;
        }
        
        public TutorialManager ShowSpeech(Action onModalTouch)
        {
            if (mSpeechObj.activeSelf == false)
            {
                mSpeechObj.SetActive(true);
            }

            foreach (var tween in mSpeechObj.GetComponentsInChildren<DOTweenAnimation>())
            {
                tween.DORewind();
                tween.DOPlay();
            }

            mHighlightObj.SetActive(false);
            mTextTouchAnyScreen.gameObject.SetActive(false);
            mButtonSpeechModalTouch.SetButtonAction(() => onModalTouch?.Invoke());

            return this;
        }

        public void SetActiveSpeechObj(bool active)
        {
            mSpeechObj.SetActive(active);
        }

        public void SetActivHighlightObj(bool active)
        {
            mHighlightObj.SetActive(active);
        }

        public TutorialManager SetCharacterImg(string name)
        {
            Sprite img = MLand.Atlas.GetCharacterSprite(name);

            mImgCharacter.sprite = img;

            return this;
        }

        public TutorialManager SetTextSpeech(string speechMessage, float duration, Action onComplete)
        {
            if (duration <= 0f)
            {
                mTextSpeech.DOKill();

                mTextSpeech.text = speechMessage;

                OnComplete();
            }
            else
            {
                mTextSpeech.text = string.Empty;
                mTextSpeech.DORewind();
                mTextSpeech.DOText(speechMessage, duration)
                    .SetAutoKill(false)
                    .OnComplete(OnComplete);
            }

            void OnComplete()
            {
                mTextTouchAnyScreen.gameObject.SetActive(true);

                onComplete?.Invoke();
            }

            return this;
        }
    }

    static class TutorialActionCreater
    {
        public static TutorialAction Create(TutorialManager manager, TutorialActionType actionType, int step, float waitTime)
        {
            TutorialAction action = null;
            switch (actionType)
            {
                case TutorialActionType.ShowMessage:
                    action = new ShowMessage();
                    break;
                case TutorialActionType.JustWait:
                    action = new JustWaitTime();
                    break;
                case TutorialActionType.Close_CurrentDetail:
                    action = new Close_CurrentDetail();
                    break;
                case TutorialActionType.Close_CurrentPopupStatus:
                    action = new Close_CurrentPopupStatus();
                    break;
                case TutorialActionType.Close_CurrentPopup:
                    action = new Close_CurrentPopup();
                    break;
                case TutorialActionType.Close_AllPopup:
                    action = new Close_AllPopup();
                    break;
                case TutorialActionType.Focus_SlimeKing:
                    action = new Focus_SlimeKing();
                    break;
                case TutorialActionType.Focus_GoldSlime:
                    action = new Focus_GoldSlime();
                    break;
                case TutorialActionType.Focus_CheapShop:
                    action = new Focus_CheapShop();
                    break;
                
                case TutorialActionType.Focus_ExpensiveShop:
                    action = new Focus_ExpensiveShop();
                    break;
                case TutorialActionType.Focus_MiniGame:
                    action = new Focus_MiniGame();
                    break;
                case TutorialActionType.Highlight_SlimeButton:
                    action = new Highlight_SlimeButton();
                    break;
                case TutorialActionType.Highlight_SlimeSpawnButton:
                    action = new Highlight_SlimeSpawnButton();
                    break;
                case TutorialActionType.Highlight_CurrentSlime:
                    action = new Highlight_CurrentSlime();
                    break;
                case TutorialActionType.Highlight_CurrentSlime_GiftButton:
                    action = new Highlight_CurrentSlime_GiftButton();
                    break;
                case TutorialActionType.Highlight_CurrentSlime_LevelupRewardButton:
                    action = new Highlight_CurrentSlime_LevelupRewardButton();
                    break;
                case TutorialActionType.Highlight_BuildButton:
                    action = new Highlight_BuildButton();
                    break;
                case TutorialActionType.Highlight_BuildingBuildButton:
                    action = new Highlight_BuildingBuildButton();
                    break;
                case TutorialActionType.Highlight_CheapShop:
                    action = new Highlight_CheapShop();
                    break;
                case TutorialActionType.Highlight_CheapShop_Gold:
                    action = new Highlight_CheapShop_Gold();
                    break;
                case TutorialActionType.Highlight_CheapShop_Gem:
                    action = new Highlight_CheapShop_Gem();
                    break;
                case TutorialActionType.Highlight_CheapShop_RandomBox:
                    action = new Highlight_CheapShop_RandomBox();
                    break;
                case TutorialActionType.Highlight_ExpensiveShop:
                    action = new Highlight_ExpensiveShop();
                    break;
                case TutorialActionType.Highlight_MiniGame:
                    action = new Highlight_MiniGame();
                    break;
                case TutorialActionType.Highlight_GoldSlime:
                    action = new Highlight_GoldSlime();
                    break;
                default:
                    break;
            }

            action?.Init(manager, step, waitTime);

            return action;
        }
    }
}