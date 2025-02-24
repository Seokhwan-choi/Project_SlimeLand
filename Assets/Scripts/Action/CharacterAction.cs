using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class CharacterAction
    {
        protected bool mIsFinish;
        protected Character mCharacter;
        protected ActionManager mParent;
        public virtual bool IsFinish => mIsFinish;
        public virtual void OnStart(ActionManager parent) 
        {
            mParent = parent;
            mCharacter = parent.Character;
            mIsFinish = false;
        }
        public virtual void OnUpdate(float dt) { }
        public virtual void OnFinish() { }
    }
}