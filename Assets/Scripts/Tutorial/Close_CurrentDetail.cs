using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Close_CurrentDetail : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.Lobby.HideDetail();

            mIsFinish = true;
        }
    }
}


