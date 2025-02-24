using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Highlight_GoldSlime : TutorialAction
    {
        GoldSlime mGoldSlime;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            mGoldSlime = MLand.GameManager.GetSlime(MLand.GameData.GoldSlimeCommonData.id) as GoldSlime;
            if (mGoldSlime == null)
            {
                mIsFinish = true;
                return;
            }

            Vector3 pos = new Vector3(mGoldSlime.transform.position.x, mGoldSlime.transform.position.y + 0.5f, mGoldSlime.transform.position.z);

            pos = Util.WorldToScreenPoint(pos);

            mParent.ShowHighlight(pos, OnHighlightTouch);
        }

        float mIntervalTime;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mIntervalTime -= dt;
            if (mIntervalTime <= dt)
            {
                mIntervalTime = 0.5f;

                MLand.GameManager.SetFollowGoldSlime();
            }

            UpdateHighlightPos();
        }

        void UpdateHighlightPos()
        {
            if (mGoldSlime != null)
            {
                Vector3 pos = new Vector3(mGoldSlime.transform.position.x, mGoldSlime.transform.position.y + 0.5f, mGoldSlime.transform.position.z);

                pos = Util.WorldToScreenPoint(pos);

                mParent.SetHighlightObjPos(pos);
            }
        }

        void OnHighlightTouch()
        {
            mGoldSlime?.OnPointerClick();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


