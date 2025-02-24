using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Focus_SlimeKing : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.GameManager.SetFollowSlimeKing();

            mIsFinish = true;
        }
    }
}


