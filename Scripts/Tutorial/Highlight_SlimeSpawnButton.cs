using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_SlimeSpawnButton : TutorialAction
    {
        Button mButtonSlimeSpawn;
        public override void Init(TutorialManager parent, int step, float waitTime = 0f)
        {
            base.Init(parent, step, waitTime);

            PopupStatus_SlimeKingUI popupStatus = MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_SlimeKingUI>();

            SlimeKing_SlimeTabUI slimeTab = popupStatus.GetTab(SlimeKingTab.Slime) as SlimeKing_SlimeTabUI;

            foreach(var data in MLand.GameData.SlimeData.Values)
            {
                bool isReady = MLand.GameManager.IsReadyForSpawnSlime(data.id);
                if ( isReady )
                {
                    var element = slimeTab.GetElement(data.id);

                    mButtonSlimeSpawn = element?.ButtonSlimeSpawn;

                    break;
                }
            }
        }

        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            if (mButtonSlimeSpawn != null)
            {
                RectTransform rectTm = mButtonSlimeSpawn.GetComponent<RectTransform>();

                if (mWaitTime <= 0f)
                    mParent.ShowHighlight(rectTm, OnHighlightTouch);
                else
                    mParent.ShowHighlight(rectTm, null);
            }
            else
            {
                mIsFinish = true;
            }
        }

        void OnHighlightTouch()
        {
            mButtonSlimeSpawn.onClick?.Invoke();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}