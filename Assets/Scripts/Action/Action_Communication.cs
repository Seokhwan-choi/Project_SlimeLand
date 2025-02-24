using UnityEngine;

namespace MLand
{
    class Action_Communication : CharacterAction
    {
        SlimeEmojiUI mEmojiUI;
        EmotionType mCurrentEmotion;
        float mTime;
        int mTouchCount;
        public override void OnStart(ActionManager parent)
        {
            base.OnStart(parent);

            mTime = MLand.GameData.SlimeCommonData.communicationTime;

            mTouchCount = 0;

            ChangeEmotion();
        }

        public override void OnUpdate(float dt)
        {
            if (MLand.PopupManager.GetPopup<Popup_GiftUI>() != null)
                return;

            mTime -= dt;
            if (mTime <= 0)
            {
                mIsFinish = true;
            }
        }

        public override void OnFinish()
        {
            base.OnFinish();

            mEmojiUI?.OnRelease(immediately:true);
        }

        public void Interaction()
        {
            mTouchCount++;
            if (mTouchCount > MLand.GameData.SlimeCommonData.communicationTouchCount)
            {
                mTouchCount = 0;

                ChangeEmotion();
            }

            RefreshTime();
        }

        public void ChangeEmotion(int emotionNum = -1)
        {
            mCurrentEmotion = (emotionNum == -1 || emotionNum == (int)EmotionType.Count) ? CharacterUtil.GetRandEmotion() : (EmotionType)emotionNum;

            mCharacter.Anim.PlayIdle(mCurrentEmotion);

            RefreshEmojiUI();
        }

        public void RefreshTime()
        {
            mTime = MLand.GameData.SlimeCommonData.communicationTime;
        }

        void RefreshEmojiUI()
        {
            if (mEmojiUI == null)
            {
                Transform parent = MLand.GameManager.UIMotionParent;

                var emojiObj = MLand.ObjectPool.AcquireUI("SlimeEmojiUI", parent);

                mEmojiUI = emojiObj.GetOrAddComponent<SlimeEmojiUI>();
                mEmojiUI.Init(mCharacter.transform, mCurrentEmotion);
            }
            else
            {
                mEmojiUI.ChangeEmotion(mCurrentEmotion);
            }
        }
    }
}