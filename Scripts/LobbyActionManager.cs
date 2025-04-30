using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MLand
{
    class LobbyActionManager
    {
        LobbyAction mCurrentLobbyAction;
        List<LobbyAction> mQueuingLobbyAction;

        bool mIsLock;
        public void Init()
        {
            mCurrentLobbyAction = null;
            mQueuingLobbyAction = new List<LobbyAction>();
        }

        public void SetLock(bool isLock)
        {
            mIsLock = isLock;
        }

        public void OnUpdate()
        {
            if (mIsLock)
                return;

            if (mCurrentLobbyAction == null)
            {
                mCurrentLobbyAction = PopLobbyAction();
                mCurrentLobbyAction?.Start();
            }
            else
            {
                if (mCurrentLobbyAction.IsFinish)
                {
                    mCurrentLobbyAction = null;
                }
            }
        }

        public void AddLobbyAction(Action<LobbyAction> action)
        {
            LobbyAction lobbyAction = new LobbyAction(action);

            mQueuingLobbyAction.Add(lobbyAction);
        }

        public void EnqueueLobbyAction(Action<LobbyAction> action)
        {
            LobbyAction lobbyAction = new LobbyAction(action);

            mQueuingLobbyAction.Insert(0, lobbyAction);
        }

        LobbyAction PopLobbyAction()
        {
            LobbyAction lobbyAction = null;
            if (mQueuingLobbyAction.Count > 0)
            {
                lobbyAction = mQueuingLobbyAction[0];
                mQueuingLobbyAction.RemoveAt(0);
            }

            return lobbyAction;
        }
    }

    class LobbyAction
    {
        bool mIsFinish;
        Action<LobbyAction> mAction;
        public bool IsFinish => mIsFinish;
        public LobbyAction(Action<LobbyAction> action)
        {
            mAction = action;
        }

        public void Start()
        {
            mAction?.Invoke(this);
        }

        public void Done()
        {
            mIsFinish = true;
        }
    }
}


