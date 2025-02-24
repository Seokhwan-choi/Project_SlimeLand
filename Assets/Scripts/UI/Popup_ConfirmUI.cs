using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Popup_ConfirmUI : PopupBase
    {
        public void Init(string title, string desc, Action onConfirm, Action onCancel, bool ignoreModalTouch = false)
        {
            Button closeButton = gameObject.FindComponent<Button>("Button_Close");
            closeButton.SetButtonAction(() => Close());

            if (ignoreModalTouch == false)
            {
                Button modalButton = gameObject.FindComponent<Button>("Button_Modal");
                modalButton.SetButtonAction(() => Close());
            }

            SetTitleText(title);

            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = desc;

            Button buttonCancel = gameObject.FindComponent<Button>("Button_Cancel");
            buttonCancel.SetButtonAction(() =>
            {
                onCancel?.Invoke();

                Close();
            });

            Button buttonConfirm = gameObject.FindComponent<Button>("Button_Confirm");
            buttonConfirm.SetButtonAction(() =>
            {
                onConfirm?.Invoke();

                Close();
            });
            
            // 킹 슬라임 등장 연출!
            Image imgGoldSlime = gameObject.FindComponent<Image>("Image_KingSlime");

            MonsterLandUtil.PlayUpAppearMotion(imgGoldSlime.rectTransform, -300f);
        }

        public void SetHideCancelButton()
        {
            var buttonCancel = gameObject.FindComponent<Button>("Button_Cancel");

            buttonCancel.gameObject.SetActive(false);
        }
    }
}


