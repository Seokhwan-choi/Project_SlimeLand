using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace MLand
{
    class ChattingSelectManager
    {
        int[] mRandomFriendShipExps;
        Button[] mSelectButtons;
        TextMeshProUGUI[] mSelectTexts;

        int mSelectedIndex;
        public bool AnySelected => mSelectedIndex != -1;
        public void Init(GameObject parent)
        {
            mSelectedIndex = -1;

            InitRandomFriendShipExps();

            mSelectButtons = new Button[3];
            mSelectTexts = new TextMeshProUGUI[3];

            for (int i = 0; i < 3; ++i)
            {
                int index = i;

                mSelectButtons[index] = parent.FindComponent<Button>($"Select_{index}");
                mSelectButtons[index].SetButtonAction(() => OnButtonAction(index));

                mSelectTexts[index] = parent.FindComponent<TextMeshProUGUI>($"Text_Select_{index}");
                mSelectTexts[index].text = StringTableUtil.Get($"ChattingMessage_SelectList_{index + 1}");
            }
        }

        void InitRandomFriendShipExps()
        {
            mRandomFriendShipExps = new int[3];

            for(int i = 0; i < 3; ++i)
            {
                mRandomFriendShipExps[i] = Random.Range(10, 50);
            }
        }

        void OnButtonAction(int index)
        {
            mSelectedIndex = index;
        }

        public (string, int) GetSelectResult()
        {
            return (GetSelectMessage(), GetSelectFriendShipExp());
        }

        string GetSelectMessage()
        {
            return mSelectTexts[mSelectedIndex].text;
        }

        int GetSelectFriendShipExp()
        {
            return mRandomFriendShipExps[mSelectedIndex];
        }
    }
}