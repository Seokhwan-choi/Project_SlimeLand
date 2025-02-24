using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MLand
{
    class MiniGameClicker : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        public void OnPointerClick()
        {
            transform.DORewind();
            transform.DOPunchScale(Vector3.one * 0.25f, 0.35f)
                .SetAutoKill(false);

            Popup_MiniGameHomeUI miniGamePopup = MLand.PopupManager.CreatePopup<Popup_MiniGameHomeUI>();

            miniGamePopup.Init();

            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = transform,
                FollowType = FollowType.Building,
            });
        }
    }
}


