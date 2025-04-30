using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_BuildingBuildButton : TutorialAction
    {
        Button mButtonBuildingBuild;
        public override void Init(TutorialManager parent, int step, float waitTime = 0f)
        {
            base.Init(parent, step, waitTime);

            PopupStatus_SlimeKingUI popupStatus = MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_SlimeKingUI>();

            SlimeKing_BuildingTabUI buildingTab = popupStatus.GetTab(SlimeKingTab.Building) as SlimeKing_BuildingTabUI;

            foreach (var data in MLand.GameData.BuildingData.Values)
            {
                bool isReady = data.isCentralBuilding ? 
                    MLand.GameManager.IsReadyForUpgradeBuilding(data.id) :
                    MLand.GameManager.IsReadyForUnlockBuilding(data.id);
                if (isReady)
                {
                    var elementList = buildingTab.GetElementList(data.elementalType);
                    var element = elementList.GetElement(data.id);

                    mButtonBuildingBuild = element?.ButtonBuild;

                    break;
                }
            }
        }

        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            if (mButtonBuildingBuild != null)
            {
                RectTransform rectTm = mButtonBuildingBuild.GetComponent<RectTransform>();

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
            mButtonBuildingBuild.onClick?.Invoke();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


