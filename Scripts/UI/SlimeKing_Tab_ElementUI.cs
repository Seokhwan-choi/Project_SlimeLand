using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class SlimeKing_Tab_ElementUI : MonoBehaviour
    {
        protected GameObject mLockObj;        // ��ȯ �Ұ��� ����
        protected GameObject mUnlockObj;      // ��ȯ ���� or ��ȯ�� ����
        protected GameObject mActiveObj;      // ��ȯ�� ����
        protected GameObject mInactiveObj;    // ��ȯ ���� ����

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