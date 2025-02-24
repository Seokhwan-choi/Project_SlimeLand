using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    //class SpecialCondition
    //{
    //    protected bool mIsSatisfy;
    //    public virtual void Init() { mIsSatisfy = true; }
    //    public virtual bool IsSatisfy() { return mIsSatisfy; }
    //    public virtual void OnUpdate(float dt) { }
    //}

    //class SpecialConditionUtil
    //{
    //    public static SpecialCondition CreateCondition(SpecialConditionType type)
    //    {
    //        SpecialCondition condition = null;

    //        switch(type)
    //        {
    //            case SpecialConditionType.CheckInvade:
    //                condition = new Cnd_CheckInvade();
    //                break;
    //            case SpecialConditionType.CheckMarsoOnField:
    //                condition = new Cnd_CheckMarsoOnField();
    //                break;
    //            default:
    //                condition = new SpecialCondition();
    //                break;
    //        }

    //        condition?.Init();

    //        return condition;
    //    }
    //}

    //class Cnd_CheckMarsoOnField : SpecialCondition
    //{
    //    public override void OnUpdate(float dt)
    //    {
    //        mIsSatisfy = MLand.GameManager.ActiveMarsoCount > 0;
    //    }
    //}

    //class Cnd_CheckInvade : SpecialCondition
    //{
    //    public override void OnUpdate(float dt)
    //    {
    //        mIsSatisfy = false;
    //    }
    //}
}