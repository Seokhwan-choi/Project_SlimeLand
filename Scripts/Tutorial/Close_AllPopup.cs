using System;
using System.Collections;


namespace MLand
{
    class Close_AllPopup : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            // 켜져있는 모든 팝업을 꺼준다.
            MLand.GameManager.StartCoroutine(AllPopupCloseMotion());
        }

        IEnumerator AllPopupCloseMotion()
        {
            // 켜져있는 모든 팝업을 꺼준다.
            while (MLand.PopupManager.Count > 0)
            {
                yield return null;

                MLand.PopupManager.OnBackButton(immediate: true);
            }

            mIsFinish = true;
        }
    }
}