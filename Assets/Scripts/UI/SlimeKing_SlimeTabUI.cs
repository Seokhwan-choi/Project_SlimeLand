using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class SlimeKing_SlimeTabUI : SlimeKingTabUI
    {
        ScrollToTarget mScrollToTarget;
        Dictionary<string, SlimeKing_SlimeTab_ElementUI> mElementDics;
        public override void Init()
        {
            var scrollRect = gameObject.FindComponent<ScrollRect>("ScrollView");

            mScrollToTarget = new ScrollToTarget();
            mScrollToTarget.Init(scrollRect);

            mElementDics = new Dictionary<string, SlimeKing_SlimeTab_ElementUI>();

            foreach(CharacterData slimeData in DataUtil.GetAllSlimeDatas())
            {
                GameObject slimeElementObj = gameObject.FindGameObject(slimeData.id);

                var slimeElement = slimeElementObj.GetOrAddComponent<SlimeKing_SlimeTab_ElementUI>();

                slimeElement.Init(slimeData.id);

                mElementDics.Add(slimeData.id, slimeElement);
            }
        }

        public override void OnTabEnter()
        {
            Refresh();
        }

        public override void Refresh()
        {
            foreach (var elementListUI in mElementDics.Values)
            {
                elementListUI.Refresh();
            }
        }

        public override void Localize()
        {
            foreach (var elementListUI in mElementDics.Values)
            {
                elementListUI.Localize();
            }
        }

        public override void ScrollToTarget(string id)
        {
            var element = mElementDics.TryGet(id);
            if (element == null)
                return;

            var tm = element.GetComponent<RectTransform>();
            if (tm == null)
                return;

            mScrollToTarget.Scroll(tm);
        }

        public SlimeKing_SlimeTab_ElementUI GetElement(string id)
        {
            return mElementDics.TryGet(id);
        }
    }
}