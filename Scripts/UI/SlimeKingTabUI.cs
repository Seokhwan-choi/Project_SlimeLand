using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class SlimeKingTabUI : MonoBehaviour
    {
        public virtual void Init() { }
        public virtual void Refresh() { }
        public virtual void OnTabEnter() { }
        public virtual void OnTabLeave() { }
        public virtual void Localize() { }
        public virtual void ScrollToTarget(string id) { }
        public virtual void MoveToTab(string id) { }
    }
}


