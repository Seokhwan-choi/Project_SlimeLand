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
    //    // ���� �ð� ���� ������ ����
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
    //// ���Ҹ� ã�Ƽ� ��������
    //class Action_MarsoCollect : Action_Special
    //{
    //    public override void OnStart(ActionManager parent)
    //    {
    //        base.OnStart(parent);

    //        // �ʿ� ���Ұ� ������ �ƹ��볪 ���� �ٴѴ�.
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
    //        // ���� ����� ���Ҹ� ã�Ƽ� �̵��Ѵ�.
    //        if (mCharacter.TargetMarso != null)
    //        {
    //            // ���Ҹ� ���ؼ� �����Ѵ�.
    //            // ���ҿ� ��������� �ش� ���Ҹ� �����Ѵ�.
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

    //// ���Ҹ� �߰� ���
    //class Action_MarsoDrop : Action_Special
    //{
    //    float mDelay;
    //    public override void OnStart(ActionManager parent)
    //    {
    //        base.OnStart(parent);

    //        mDelay = 2f;

    //        // �ִϸ��̼�
    //        parent.Character.Anim.PlaySpecial();

    //        // ��� ������ & �ǹ��� ���Ҹ� ����Ѵ�.
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