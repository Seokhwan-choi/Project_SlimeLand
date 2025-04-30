using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MLand
{
    class Highlight_CheapShop_Gold : TutorialAction
    {
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            PopupStatus_CheapShopUI cheapShop = MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_CheapShopUI>();

            Button tabButton = cheapShop.GetTabButton(CheapShopTab.Gold);
            if (tabButton != null)
            {
                RectTransform rectTm = tabButton.GetComponent<RectTransform>();

                if (mWaitTime <= 0f)
                    mParent.ShowHighlight(rectTm, OnHighlightTouch);
                else
                    mParent.ShowHighlight(rectTm, null);

                void OnHighlightTouch()
                {
                    tabButton?.onClick?.Invoke();

                    mIsFinish = true;

                    MLand.GameManager.StartTouchBlock(100f);
                }
            }
            else
            {
                mIsFinish = true;

                MLand.GameManager.StartTouchBlock(100f);
            }
        }
    }
}


