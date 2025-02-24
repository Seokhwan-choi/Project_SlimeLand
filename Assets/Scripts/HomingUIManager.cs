using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace MLand
{
    class HomingUIManager
    {
        List<HomingUI> mHomingMotionList;
        public void Init()
        {
            mHomingMotionList = new List<HomingUI>();
        }

        public void AddHoming(Vector3 startPos, Vector3 endPos, double marso, Action onArriveAction)
        {
            GameObject motionObj = MLand.ObjectPool.AcquireUI("UIMotion");

            motionObj.transform.position = startPos;

            HomingUI homing = HomingUI.Play(motionObj.transform, endPos).
                SetAmount(marso).
                SetDelay(0.75f).
                SetDuration(1f).
                SetOnArriveAction(onArriveAction);

            mHomingMotionList.Add(homing);
        }

        public void OnUpdate(float dt)
        {
            List<HomingUI> arriveList = new List<HomingUI>();

            foreach (var homing in mHomingMotionList)
            {
                homing.OnUpdate(dt);

                if (homing.IsArrive)
                    arriveList.Add(homing);
            }

            foreach (HomingUI arrive in arriveList)
            {
                mHomingMotionList.Remove(arrive);

                arrive.Release();
            }
        }
    }
}