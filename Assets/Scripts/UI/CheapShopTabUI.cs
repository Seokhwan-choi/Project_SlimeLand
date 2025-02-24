using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class CheapShopTabUI : MonoBehaviour
    {
        protected CheapShopTabUIManager mParent;
        public virtual void Init(CheapShopTabUIManager parent) { mParent = parent; }
        public virtual void OnUpdate() { }
        public virtual void OnTabEnter() { }
        public virtual void OnTabLeave() { }
        public virtual void Localize() { }
    }
}


