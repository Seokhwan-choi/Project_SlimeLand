using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class JustWaitTime : TutorialAction
    {
        public override void Init(TutorialManager parent, int step, float waitTime = 0)
        {
            base.Init(parent, step, waitTime);
        }

        public override void OnStart()
        {
            base.OnStart();

            mParent.SetActiveSpeechObj(false);
            mParent.SetActivHighlightObj(false);

            MLand.GameManager.StartTouchBlock(mWaitTime);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mWaitTime -= dt;
            if (mWaitTime <= 0f)
            {
                mIsFinish = true;
            }
            else
            {
                if (MLand.GameManager.IsActiveTouchBlock() == false)
                    MLand.GameManager.StartTouchBlock(mWaitTime);
            }
        }

        public override void OnFinish()
        {
            base.OnFinish();

            MLand.GameManager.EndTouchBlock();
        }
    }
}


