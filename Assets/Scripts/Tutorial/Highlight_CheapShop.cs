using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_CheapShop : TutorialAction
    {
        CheapShopClicker mCheapShop;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            mCheapShop = MLand.GameManager.CheapShop;

            Vector3 pos = UpdateHighlightPos();

            if (mWaitTime <= 0f)
                mParent.ShowHighlight(pos, OnHighlightTouch);
            else
                mParent.ShowHighlight(pos, null);
        }

        float mIntervalTime;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mIntervalTime -= dt;
            if (mIntervalTime <= dt)
            {
                mIntervalTime = 0.5f;

                MLand.GameManager.SetFollowCheapShop();
            }

            mParent.SetHighlightObjPos(UpdateHighlightPos());
        }

        Vector3 UpdateHighlightPos()
        {
            Vector3 pos = new Vector3(mCheapShop.transform.position.x, mCheapShop.transform.position.y + 1.5f, mCheapShop.transform.position.z);

            pos = Util.WorldToScreenPoint(pos);

            return pos;
        }

        void OnHighlightTouch()
        {
            MLand.GameManager.CheapShop.OnPointerClick();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


