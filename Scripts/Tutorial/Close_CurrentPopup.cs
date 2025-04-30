using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Close_CurrentPopup : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            // 제일 위에 팝업을 꺼준다.
            MLand.PopupManager.OnBackButton();

            mIsFinish = true;
        }
    }
}


