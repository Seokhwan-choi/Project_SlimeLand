using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

namespace MLand
{
    enum ActionType
    {
        Idle,
        Spawn,
        RandomMove,
        Communication,
    }

    class ActionManager
    {
        Character mCharacter;
        CharacterAction mPrevAction;    // 직전에 어떤 액션을 했는지 저장
        CharacterAction mCurrentAction;
        List<CharacterAction> mActionList;

        public Character Character => mCharacter;
        public CharacterAction PrevAction => mPrevAction;
        public CharacterAction CurrentAction => mCurrentAction;
        public bool IsMove => CurrentAction != null ? CurrentAction is Action_RandomMove : false;
        public bool IsIdle => CurrentAction != null ? CurrentAction is Action_Idle : false;
        public bool IsCommunication => CurrentAction != null ? CurrentAction is Action_Communication : false;
        public void Init(Character character)
        {
            mCharacter = character;

            mActionList = new List<CharacterAction>();

            mPrevAction = null;
            mCurrentAction = null;
        }

        public void OnUpdate(float dt)
        {
            if (mCurrentAction == null)
            {
                mCurrentAction = PopAction();

                mCurrentAction?.OnStart(this);
            }
            else
            {
                mCurrentAction.OnUpdate(dt);

                if (mCurrentAction.IsFinish)
                {
                    mCurrentAction.OnFinish();
                    mPrevAction = mCurrentAction;
                    mCurrentAction = null;
                }
            }
        }

        CharacterAction PopAction()
        {
            CharacterAction popAction = null;

            if (mActionList.Count >= 1)
            {
                popAction = mActionList[0];

                mActionList.RemoveAt(0);
            }

            return popAction;
        }

        public void PlayAction(ActionType actionType)
        {
            CharacterAction playAction = null;

            switch (actionType)
            {
                case ActionType.Idle:
                    playAction = new Action_Idle();
                    break;
                case ActionType.Spawn:
                    playAction = new Action_Spawn();
                    break;
                case ActionType.RandomMove:
                    playAction = new Action_RandomMove();
                    break;
                case ActionType.Communication:
                    playAction = new Action_Communication();
                    break;
                default:
                    break;
            }

            StartAction(playAction);
        }

        public void Communicate()
        {
            if (IsCommunication == false)
                return;

            Action_Communication action = mCurrentAction as Action_Communication;

            action.Interaction();
        }

        public void ChangeEmotion(int emotionNum = -1)
        {
            if (IsCommunication == false)
                return;

            Action_Communication action = mCurrentAction as Action_Communication;

            action.ChangeEmotion(emotionNum);
        }

        public void RefreshCommunicationTime()
        {
            if (IsCommunication == false)
                return;

            Action_Communication action = mCurrentAction as Action_Communication;

            action.RefreshTime();
        }

        void StartAction(CharacterAction startAction)
        {
            mCurrentAction?.OnFinish();

            mPrevAction = mCurrentAction;

            mCurrentAction = startAction;

            startAction?.OnStart(this);
        }
    }
}