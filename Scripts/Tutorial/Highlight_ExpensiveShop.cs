using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_ExpensiveShop : TutorialAction
    {
        ExpensiveShopClicker mExpensiveShop;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            mExpensiveShop = MLand.GameManager.GetActiveExpensiveShop();
            if (mExpensiveShop == null)
            {
                mIsFinish = true;
                return;
            }

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

                MLand.GameManager.SetFollwExpensiveShop();
            }

            mParent.SetHighlightObjPos(UpdateHighlightPos());
        }

        Vector3 UpdateHighlightPos()
        {
            Vector3 pos = new Vector3(mExpensiveShop.transform.position.x, mExpensiveShop.transform.position.y + 0.5f, mExpensiveShop.transform.position.z);

            pos = Util.WorldToScreenPoint(pos);

            return pos;
        }

        void OnHighlightTouch()
        {
            ExpensiveShopClicker expensiveShop = MLand.GameManager.GetActiveExpensiveShop();

            expensiveShop?.OnPointerClick();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


