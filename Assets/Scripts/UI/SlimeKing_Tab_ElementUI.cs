using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class SlimeKing_Tab_ElementUI : MonoBehaviour
    {
        protected GameObject mLockObj;        // 소환 불가능 상태
        protected GameObject mUnlockObj;      // 소환 가능 or 소환된 상태
        protected GameObject mActiveObj;      // 소환된 상태
        protected GameObject mInactiveObj;    // 소환 가능 상태

        public virtual void Init(string id)
        {
            mUnlockObj = gameObject.FindGameObject("Unlock");
            mLockObj = gameObject.FindGameObject("Lock");
            mActiveObj = mUnlockObj.FindGameObject("Active");
            mInactiveObj = mUnlockObj.FindGameObject("Inactive");
        }

        public virtual void Refresh() { }
        public virtual void Localize() { }
        protected void SetLock(bool setLock)
        {
            mLockObj.SetActive(setLock);
            mUnlockObj.SetActive(!setLock);
        }

        protected void SetActive(bool setActive)
        {
            mActiveObj.SetActive(setActive);
            mInactiveObj.SetActive(!setActive);
        }
    }
}