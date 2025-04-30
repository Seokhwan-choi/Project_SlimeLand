using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_CurrentSlime_LevelupRewardButton : TutorialAction
    {
        Button mButtonSlimeLevelupReward;
        public override void Init(TutorialManager parent, int step, float waitTime = 0f)
        {
            base.Init(parent, step, waitTime);

            DetailUI curDetail = MLand.Lobby.DetailList.GetCurrentDetail();

            if (curDetail is Detail_SlimeUI detail_slime)
            {
                mButtonSlimeLevelupReward = detail_slime.gameObject.FindComponent<Button>("Btn_Reward");
            }
        }

        public override void OnStart()
        {
            if (mButtonSlimeLevelupReward != null)
            {
                RectTransform rectTm = mButtonSlimeLevelupReward.GetComponent<RectTransform>();

                if (mWaitTime <= 0f)
                    mParent.ShowHighlight(rectTm, OnHighlighTouch);
                else
                    mParent.ShowHighlight(rectTm, null);
            }
            else
            {
                mIsFinish = true;
            }
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mWaitTime -= dt;
            if (mWaitTime <= 0f)
            {
                mIsFinish = true;
            }
        }

        void OnHighlighTouch()
        {
            mButtonSlimeLevelupReward.onClick?.Invoke();

            mIsFinish = true;
        }
    }
}


