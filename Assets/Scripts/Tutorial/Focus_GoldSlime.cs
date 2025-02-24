using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Focus_GoldSlime : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.GameManager.SetFollowGoldSlime();

            mIsFinish = true;
        }
    }
}


