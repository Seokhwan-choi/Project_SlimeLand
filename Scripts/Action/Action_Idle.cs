using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Action_Idle : CharacterAction
    {
        float mIdleTime;
        public override void OnStart(ActionManager parent)
        {
            base.OnStart(parent);

            EmotionType randEmotion = CharacterUtil.GetRandEmotion();

            mIdleTime = GetIdleTime(randEmotion);

            mParent.Character.Anim.PlayIdle(randEmotion);
        }

        float GetIdleTime(EmotionType emotionType)
        {
            switch (emotionType)
            {
                case EmotionType.Excited:
                    return MLand.GameData.SlimeCommonData.exitedTime;
                case EmotionType.Happy:
                    return MLand.GameData.SlimeCommonData.happyTime;
                case EmotionType.Shock:
                    return MLand.GameData.SlimeCommonData.shockTime;
                case EmotionType.Sleepy:
                    return MLand.GameData.SlimeCommonData.sleepyTime;
                case EmotionType.Idle:
                default:
                    return MLand.GameData.SlimeCommonData.idleTime;
            }
        }

        public override void OnUpdate(float dt)
        {
            mIdleTime -= dt;
            if (mIdleTime <= 0f)
            {
                mIsFinish = true;
            }
        }
    }
}

