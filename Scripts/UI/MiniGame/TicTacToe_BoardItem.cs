using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MLand
{
    class BoardItem : MonoBehaviour
    {
        GameObject mMarkO;
        GameObject mMarkX;
        Popup_MiniGame_TicTacToeUI mParent;

        PlayerSymbol mSymbol;
        public PlayerSymbol Symbol => mSymbol;
        public bool IsEmpty => mSymbol == PlayerSymbol.None;
        public void Init(Popup_MiniGame_TicTacToeUI parent)
        {
            mParent = parent;

            mMarkO = gameObject.FindGameObject("Mark_O");
            mMarkX = gameObject.FindGameObject("Mark_X");

            OnReset();

            Button button = gameObject.GetComponent<Button>();
            button.SetButtonAction(() =>
            {
                if (IsEmpty == false)
                    return;

                if (mParent.IsPlayerTurn == false)
                    return;

                OnSelect(parent.PlayerSymbol);
            });
        }

        public void OnReset()
        {
            mSymbol = PlayerSymbol.None;

            mMarkO.SetActive(false);
            mMarkX.SetActive(false);
        }

        public void OnSelect(PlayerSymbol symbol)
        {
            mSymbol = symbol;

            SoundPlayer.PlayMiniGame_TicTacToe_MakeMark(symbol);

            if (symbol == PlayerSymbol.O)
            {
                PlayMarkOMotion();
            }
            else
            {
                PlayMarkXMotion();
            }

            mParent.OnSelectedBoardItem();
        }

        Tweener mMarkOTweener;
        void PlayMarkOMotion()
        {
            mMarkO.SetActive(true);

            var imgMarkO = mMarkO.FindComponent<Image>("Image_Mark_O");
            imgMarkO.fillAmount = 0f;

            mMarkOTweener?.Rewind();
            mMarkOTweener = DOTween.To((f) => imgMarkO.fillAmount = f, 0f, 1f, 0.5f)
                .SetAutoKill(false);
        }

        Sequence mMarkXSeqence;
        void PlayMarkXMotion()
        {
            mMarkX.SetActive(true);

            var imgMarkX_1 = mMarkX.FindComponent<Image>("Image_Mark_X_1");
            imgMarkX_1.fillAmount = 0f;
            var imgMarkX_2 = mMarkX.FindComponent<Image>("Image_Mark_X_2");
            imgMarkX_2.fillAmount = 0f;

            mMarkXSeqence?.Rewind();
            mMarkXSeqence = DOTween.Sequence()
                .Append(DOTween.To((f) => imgMarkX_1.fillAmount = f, 0f, 1f, 0.25f))
                .Append(DOTween.To((f) => imgMarkX_2.fillAmount = f, 0f, 1f, 0.25f))
                .SetAutoKill(false);
        }
    }
}
