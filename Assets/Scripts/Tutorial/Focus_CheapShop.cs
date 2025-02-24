using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Focus_CheapShop : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.GameManager.SetFollowCheapShop();

            mIsFinish = true;
        }
    }
}


