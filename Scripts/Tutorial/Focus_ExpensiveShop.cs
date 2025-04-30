using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Focus_ExpensiveShop : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.GameManager.SetFollwExpensiveShop();

            mIsFinish = true;
        }
    }
}


