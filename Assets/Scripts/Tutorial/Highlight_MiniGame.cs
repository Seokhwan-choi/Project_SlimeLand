using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_MiniGame : TutorialAction
    {
        MiniGameClicker mMiniGame;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            mMiniGame = MLand.GameManager.MiniGame;

            Vector3 pos = UpdateHighlightPos();

            if (mWaitTime <= 0f)
                mParent.ShowHighlight(pos, OnHighlightTouch);
            else
                mParent.ShowHighlight(pos, null);
        }

        float mIntervalTime;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mIntervalTime -= dt;
            if (mIntervalTime <= dt)
            {
                mIntervalTime = 0.5f;

                MLand.GameManager.SetFollowMiniGame();
            }

            mParent.SetHighlightObjPos(UpdateHighlightPos());
        }

        Vector3 UpdateHighlightPos()
        {
            Vector3 pos = new Vector3(mMiniGame.transform.position.x, mMiniGame.transform.position.y, mMiniGame.transform.position.z);

            pos = Util.WorldToScreenPoint(pos);

            return pos;
        }

        void OnHighlightTouch()
        {
            MLand.GameManager.MiniGame.OnPointerClick();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


