using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


namespace MLand
{
    class CharacterPathFinder
    {
        bool mActive;
        
        float mArriveOffset;
        Character mCharacter;
        NavMeshAgent mAgent;
        Action mArriveAction;

        Transform mPathFindTarget;
        Vector3? mPathFindTargetPos;
        public void Init(Character character)
        {
            mCharacter = character;
            mAgent = character.GetComponent<NavMeshAgent>();

            Debug.Assert(mAgent != null, $"{character.name}에 NavMeshAgent가 없습니다.");

            mActive = false;
            mAgent.updateRotation = false;
            mAgent.updateUpAxis = false;
            mAgent.enabled = false;
            mAgent.avoidancePriority = (50 + UnityEngine.Random.Range(-10, 10));

            mArriveAction = null;

            SetMoveSpeed(mCharacter.Data.moveSpeed);
        }

        public void StartActive(Vector3 pos)
        {
            mCharacter.StartCoroutine(PlayActiveRoutine(pos));
        }

        IEnumerator PlayActiveRoutine(Vector3 startPos)
        {
            yield return new WaitForEndOfFrame(); // 한 프레임 기다리고
            yield return new WaitForEndOfFrame(); // 한 프레임 기다리고
            //yield return new WaitForEndOfFrame(); // 한 프레임 기다리고
            //yield return new WaitForEndOfFrame(); // 한 프레임 기다리고
            //yield return new WaitForEndOfFrame(); // 한 프레임 기다리고

            mActive = true;

            mAgent.enabled = true;

            mAgent.Warp(startPos);
        }


        public void SetMoveSpeed(float moveSpeed)
        {
            mAgent.speed = moveSpeed;
        }

        public void ResetPath()
        {
            mAgent.ResetPath();

            mAgent.velocity = Vector3.zero;
            mAgent.updatePosition = false;
        }

        float mFlipInterval;
        public void OnUpdate(float dt)
        {
            if (mActive)
            {
                if (mPathFindTarget != null)
                {
                    if (mPathFindTargetPos.Value != mPathFindTarget.position)
                    {
                        mPathFindTargetPos = mPathFindTarget.position;

                        mAgent.SetDestination(mPathFindTargetPos.Value);
                    }
                }

                if (mAgent.isStopped == false)
                {
                    mFlipInterval -= dt;
                    if (mFlipInterval <= 0f)
                    {
                        Vector3 dir = mAgent.velocity.normalized;

                        bool isRight = dir.x > 0;

                        mCharacter.ChangeSpriteFlipX(isRight);

                        mFlipInterval = 0.25f;
                    }
                }

                if (mArriveAction != null && IsArrive())
                {
                    mArriveAction.Invoke();

                    ResetPath();
                }
            }
        }

        public bool IsArrive()
        {
            if (mPathFindTargetPos != null)
            {
                float distance = Util.GetDistanceBy2D(mPathFindTargetPos.Value, mCharacter.transform.position);

                return distance <= mArriveOffset;
            }
            else
            {
                return false;
            }
        }

        public void SetPathFindTarget(Transform target, float arriveOffset, Action onArrive = null)
        {
            SetPathFindTarget(target.position, arriveOffset, onArrive);

            mPathFindTarget = target;
        }

        public void SetPathFindTarget(Vector3 targetPos, float arriveOffset, Action onArrive = null)
        {
            mPathFindTarget = null;
            mPathFindTargetPos = targetPos;
            mArriveOffset = arriveOffset;
            mArriveAction = onArrive;

            if (mPathFindTargetPos != null)
            {
                mAgent.SetDestination(mPathFindTargetPos.Value);

                mAgent.updatePosition = true;

                SetMoveSpeed(mCharacter.Data.moveSpeed);
            }
            else
            {
                mArriveAction?.Invoke();

                ResetPath();
            }
        }
    }
}