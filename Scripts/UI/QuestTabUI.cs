using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class QuestTabUI : MonoBehaviour
    {
        protected QuestTabUIManager mParent;
        public virtual void Init(QuestTabUIManager parent) { mParent = parent; }
        public virtual void OnUpdate() { }
        public virtual void OnTabEnter() { }
        public virtual void OnTabLeave() { }
        public virtual void Refresh() { }
    }
}