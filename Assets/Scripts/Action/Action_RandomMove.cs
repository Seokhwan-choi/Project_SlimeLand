using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Action_RandomMove : CharacterAction
    {
        public override void OnStart(ActionManager parent)
        {
            base.OnStart(parent);

            mDuration = 15f;

            mCharacter.Anim.PlayMove();

            mCharacter.MoveToRandomPos(onArrive:() =>
            {
                mIsFinish = true;
            });
        }

        float mDuration;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            mDuration -= dt;
            if (mDuration <= 0f)
            {
                OnStart(mParent);
            }
        }

        public override void OnFinish()
        {
            base.OnFinish();

            mCharacter.PathFinder.ResetPath();
        }
    }
}
