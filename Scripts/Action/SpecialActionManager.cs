using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    //class SpecialActionManager
    //{
    //    float mSpecialActionDelay;
    //    ActionManager mActionManager;
    //    //SpecialCondition mCondition;
    //    // 일정 시간 마다 무조건 실행
    //    public void Init(ActionManager actionManager)
    //    {
    //        mActionManager = actionManager;

    //        mSpecialActionDelay = actionManager.SpecialActionDelay;

    //        mCondition = SpecialConditionUtil.CreateCondition(actionManager.Character.Data.specialConditionType);
    //    }
    //    public void OnUpdate(float dt)
    //    {
    //        if (mActionManager.IsSpecialAction)
    //            return;

    //        mCondition?.OnUpdate(dt);

    //        mSpecialActionDelay -= dt;
    //        if (mSpecialActionDelay <= 0f && mCondition.IsSatisfy())
    //        {
    //            mSpecialActionDelay = mActionManager.SpecialActionDelay;

    //            mActionManager.PlayAction(ActionType.Special);
    //        }
    //    }
    //}
    //class Action_Special : CharacterAction { }
    //// 마소를 찾아서 수집하자
    //class Action_MarsoCollect : Action_Special
    //{
    //    public override void OnStart(ActionManager parent)
    //    {
    //        base.OnStart(parent);

    //        // 맵에 마소가 없으면 아무대나 돌아 다닌다.
    //        if (mCharacter.TargetMarso == null)
    //        {
    //            mCharacter.MoveToRandomPos(onArrive:() =>
    //            {
    //                mIsFinish = true;
    //            });
    //        }
    //    }
    //    public override void OnUpdate(float dt)
    //    {
    //        // 제일 가까운 마소를 찾아서 이동한다.
    //        if (mCharacter.TargetMarso != null)
    //        {
    //            // 마소를 향해서 접근한다.
    //            // 마소에 가까워지면 해당 마소를 습득한다.
    //            mCharacter.MoveToTarget(mCharacter.TargetMarso.transform.position, 
    //                onArrive:() =>
    //            {
    //                mCharacter.TargetMarso?.Acquire();

    //                mIsFinish = true;
    //            });
    //        }
    //    }
    //}

    //class Action_Fight : Action_Special
    //{
    //    Action_Attack mAction_Attack;
    //    public override void OnStart(ActionManager parent)
    //    {
    //        mAction_Attack = new Action_Attack();

    //        mAction_Attack.OnStart(parent);
    //    }

    //    public override void OnUpdate(float dt)
    //    {
    //        mAction_Attack.OnUpdate(dt);

    //        mIsFinish = mAction_Attack.IsFinish;
    //    }
    //}

    //// 마소를 추가 드롭
    //class Action_MarsoDrop : Action_Special
    //{
    //    float mDelay;
    //    public override void OnStart(ActionManager parent)
    //    {
    //        base.OnStart(parent);

    //        mDelay = 2f;

    //        // 애니메이션
    //        parent.Character.Anim.PlaySpecial();

    //        // 모든 슬라임 & 건물이 마소를 드랍한다.
    //        MLand.GameManager.DropMarsoAll();
    //    }

    //    public override void OnUpdate(float dt)
    //    {
    //        base.OnUpdate(dt);

    //        mDelay -= dt;
    //        if (mDelay <= 0f)
    //        {
    //            mIsFinish = true;
    //        }
    //    }
    //}
}