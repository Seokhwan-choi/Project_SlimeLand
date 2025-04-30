using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Highlight_CurrentSlime : TutorialAction
    {
        Slime mCurrentSlime;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            DetailUI curDetail = MLand.Lobby.DetailList.GetCurrentDetail();

            if (curDetail is Detail_SlimeUI detail_slime)
            {
                if (detail_slime.CurId.IsValid())
                {
                    mCurrentSlime = MLand.GameManager.GetSlime(detail_slime.CurId);
                }
            }

            if (mCurrentSlime != null)
            {
                Vector3 slimePos = new Vector3(mCurrentSlime.transform.position.x, mCurrentSlime.transform.position.y + 0.5f, mCurrentSlime.transform.position.z);

                Vector3 pos = Util.WorldToScreenPoint(slimePos);

                if (mWaitTime <= 0f)
                    mParent.ShowHighlight(pos, OnHighlighTouch);
                else
                    mParent.ShowHighlight(pos, null);
            }
            else
            {
                mIsFinish = true;

                MLand.GameManager.StartTouchBlock(100f);
            }
        }

        float mIntervalTime;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mIntervalTime -= dt;
            if (mIntervalTime <= dt)
            {
                mIntervalTime = 0.5f;

                MLand.CameraManager.SetFollowInfo(new FollowInfo()
                {
                    FollowTm = mCurrentSlime.transform,
                    FollowType = FollowType.Slime,
                });
            }

            UpdateHighlightPos();
        }

        void UpdateHighlightPos()
        {
            Vector3 pos = new Vector3(mCurrentSlime.transform.position.x, mCurrentSlime.transform.position.y + 0.5f, mCurrentSlime.transform.position.z);

            pos = Util.WorldToScreenPoint(pos);

            mParent.SetHighlightObjPos(pos);
        }

        void OnHighlighTouch()
        {
            mCurrentSlime.OnTouchAction();

            mIsFinish = true;

            MLand.GameManager.StartTouchBlock(100f);
        }
    }
}


