using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class TutorialAction
    {
        protected bool mIsFinish;
        protected float mWaitTime;
        protected int mStep;
        protected TutorialManager mParent;
        public bool IsFinish => mIsFinish;
        public virtual void Init(TutorialManager parent, int step, float waitTime = 0f) { mParent = parent; mStep = step; mWaitTime = waitTime; }
        public virtual void OnStart() {}
        public virtual void OnUpdate(float dt) { }
        public virtual void OnFinish() { }
    }
}


