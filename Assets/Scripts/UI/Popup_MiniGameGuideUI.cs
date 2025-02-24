using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Popup_MiniGameGuideUI : PopupBase
    {
        public void Init(MiniGameType type)
        {
            GameObject contentsParent = gameObject.FindGameObject("Contents");

            for (int i = 0; i < (int)MiniGameType.Count; ++i)
            {
                MiniGameType miniGameType = (MiniGameType)i;

                GameObject content = contentsParent.FindGameObject($"{miniGameType}");

                content.SetActive(miniGameType == type);
            }

            var buttonConfirm = gameObject.FindComponent<Button>("Button_Confirm");
            buttonConfirm.SetButtonAction(() => Close());
        }
    }
}