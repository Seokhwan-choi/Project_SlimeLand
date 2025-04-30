using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Close_CurrentPopupStatus : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.Lobby.HidePopupStatus();

            mIsFinish = true;
        }
    }
}


