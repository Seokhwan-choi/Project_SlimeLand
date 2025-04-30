using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_SlimeButton : TutorialAction
    {
        Button mButtonSlime;
        public override void Init(TutorialManager parent, int step, float waitTime = 0f)
        {
            base.Init(parent, step, waitTime);

            GameObject navbar = MLand.Lobby.Find("NavBar");

            mButtonSlime = navbar.FindComponent<Button>("Slime");
        }

        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            RectTransform rectTm = mButtonSlime.GetComponent<RectTransform>();

            if (mWaitTime <= 0f)
                mParent.ShowHighlight(rectTm, OnHighlightTouch);
            else
                mParent.ShowHighlight(rectTm, null);
        }

        void OnHighlightTouch()
        {
            mButtonSlime.onClick?.Invoke();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


