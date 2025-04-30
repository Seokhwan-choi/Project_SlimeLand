using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class Costume_FaceUI : CostumeTabUI
    {
        Dictionary<string, CostumeItemUI> mItemDics;
        public override void Init(CostumeTabUIManager parent)
        {
            GameObject list = gameObject.FindGameObject("CostumeList");

            list.AllChildObjectOff();

            mItemDics = new Dictionary<string, CostumeItemUI>();

            foreach (CostumeData data in DataUtil.GetCostumeDatasByType(CostumeType.Face))
            {
                var costumeItemObj = Util.InstantiateUI("CostumeItemUI", list.transform);

                var costumeItemUI = costumeItemObj.GetOrAddComponent<CostumeItemUI>();

                costumeItemUI.Init(parent, data.id);

                mItemDics.Add(data.id, costumeItemUI);
            }
        }

        public override void OnTabEnter()
        {
            base.OnTabEnter();

            foreach(var item in mItemDics.Values)
            {
                item.Refresh();
            }
        }

        public override void Localize()
        {
            foreach (var item in mItemDics.Values)
            {
                item.Localize();
            }
        }
    }
}