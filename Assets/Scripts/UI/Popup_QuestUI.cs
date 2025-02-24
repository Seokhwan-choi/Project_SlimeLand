using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class Popup_QuestUI : PopupBase
    {
        QuestTabUIManager mTabManager;
        public void Init()
        {
            SetUpCloseAction();

            mTabManager = new QuestTabUIManager();
            mTabManager.Init(this);

            MLand.SavePoint.Save();
        }

        public void RefreshNewDot()
        {
            mTabManager.RefreshNewDot();
        }

        private void Update()
        {
            mTabManager.OnUpdate();
        }
    }
}