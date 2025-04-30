using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_BuildButton : TutorialAction
    {
        Button mButtonBuild;
        public override void Init(TutorialManager parent, int step, float waitTime = 0f)
        {
            base.Init(parent, step, waitTime);

            GameObject navbar = MLand.Lobby.Find("NavBar");

            mButtonBuild = navbar.FindComponent<Button>("Building");
        }

        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            RectTransform rectTm = mButtonBuild.GetComponent<RectTransform>();

            if (mWaitTime <= 0f)
                mParent.ShowHighlight(rectTm, OnHighlightTouch);
            else
                mParent.ShowHighlight(rectTm, null);
        }

        void OnHighlightTouch()
        {
            mButtonBuild.onClick?.Invoke();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


