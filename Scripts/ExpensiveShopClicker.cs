using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MLand
{
    class ExpensiveShopClicker : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        public void OnPointerClick()
        {
            if (MLand.SavePoint.ExpensiveShop.IsSatisfied() == false)
            {
                SoundPlayer.PlayErrorSound();
            }
            else
            {
                SoundPlayer.PlayUIButtonTouchSound();

                transform.DORewind();
                transform.DOPunchScale(Vector3.one * 0.25f, 0.35f)
                    .SetAutoKill(false);

                var popup = MLand.PopupManager.CreatePopup<Popup_ExpensiveShopUI>();
                popup.Init();
            }
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
