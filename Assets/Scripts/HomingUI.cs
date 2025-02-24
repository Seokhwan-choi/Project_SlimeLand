using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

namespace MLand
{
    class HomingUI
    {
        double mAmount;
        float mTime;
        float mDelay;
        float mDuration;
        
        Transform mTm;
        Vector3 mDir;
        Vector3 mTarget;
        Action mOnArriveAction;
        bool mIsArrive;
        public bool IsArrive => mIsArrive;
        public static HomingUI Play(Transform tm, Vector3 target)
        {
            HomingUI homing = new HomingUI(tm, target);

            return homing;
        }

        HomingUI(Transform tm, Vector3 target)
        {
            mTm = tm;
            mTarget = target;
            mDir = UnityEngine.Random.insideUnitCircle;
            mIsArrive = false;
        }

        public HomingUI SetAmount(double amount)
        {
            mAmount = amount;

            return this;
        }

        public HomingUI SetDuration(float duration)
        {
            mDuration = duration;

            return this;
        }

        public HomingUI SetDelay(float delay)
        {
            mDelay = delay;

            return this;
        }

        public HomingUI SetOnArriveAction(Action onArriveAction)
        {
            mOnArriveAction = onArriveAction;

            return this;
        }

        public void OnUpdate(float dt)
        {
            if (mIsArrive)
                return;

            mDelay -= dt;
            if (mDelay <= 0)
            {
                mTime += dt;
                // 구형보간
                mTm.position = Vector3.Slerp(mTm.position, mTarget, mTime / mDuration);
            }
            else
            {
                // 앞으로 직진
                mTm.position = Vector3.Slerp(mTm.position, mTm.position + (mDir * 4.5f), 1f);
            }

            if (Util.GetDistanceBy2D(mTm.position, mTarget) <= 0.1f)
            {
                mOnArriveAction?.Invoke();

                mIsArrive = true;
            }
        }

        public void Release()
        {
            MLand.ObjectPool.ReleaseUI(mTm.gameObject);
        }
    }
}