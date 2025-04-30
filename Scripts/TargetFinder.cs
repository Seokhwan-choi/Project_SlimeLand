using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class TargetFinder
    {
        Character mParent;
        Character mTarget;

        Marso mTargetMarso;
        public Character Target => mTarget;
        public Marso TargetMarso => mTargetMarso;
        public void Init(Character parent)
        {
            mParent = parent;
            mTarget = null;
            mTargetMarso = null;
        }

        public void OnUpdate()
        {
            //if (mTarget == null)
            //{
            //    mTarget = mParent.IsWarrior ?
            //        MLand.GameManager.FindClosestSlime(mParent) :
            //        MLand.GameManager.FindClosestWarrior(mParent);
            //}
            //else
            //{
            //    if (mTarget.IsDead || mTarget.IsStun)
            //        mTarget = null;
            //}

            //if (mTargetMarso == null)
            //{
            //    mTargetMarso = MLand.GameManager.FindClosestMarso(mParent);
            //}
            //else
            //{
            //    if (mTargetMarso.IsDisappearing)
            //        mTargetMarso = null;
            //}
        }
    }
}


