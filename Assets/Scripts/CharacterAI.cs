using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class CharacterAI
    {
        ActionManager mActionManager;
        public void Init(ActionManager actionManager)
        {
            mActionManager = actionManager;
        }

        public void OnUpdate(float dt)
        {
            // 어떤 액션도 하고 있지 않다면 새로운 액션을 시작하자.
            if (mActionManager.CurrentAction == null)
                mActionManager.PlayAction(DecideSlimeAction());
        }

        ActionType DecideSlimeAction()
        {
            if (mActionManager.PrevAction is Action_RandomMove)
            {
                return ActionType.Idle;
            }
            else if (mActionManager.PrevAction is Action_Idle || mActionManager.PrevAction is Action_Spawn)
            {
                return ActionType.RandomMove;
            }

            return ActionType.Idle;
        }
    }
}


