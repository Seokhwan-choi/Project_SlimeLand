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

            // ���� ���� �˾��� ���ش�.
            MLand.PopupManager.OnBackButton();

            mIsFinish = true;
        }
    }
}


