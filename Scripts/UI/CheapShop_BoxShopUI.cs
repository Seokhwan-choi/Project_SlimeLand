using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class CheapShop_BoxShopUI : CheapShopTabUI
    {
        Dictionary<string, CheapShop_BoxItem> mBoxItems;
        public override void Init(CheapShopTabUIManager parent)
        {
            base.Init(parent);

            InitBoxItems();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            float dt = Time.deltaTime;

            foreach(var boxItem in mBoxItems.Values)
            {
                boxItem?.OnUpdate(dt);
            }
        }

        public override void Localize()
        {
            foreach (var boxItem in mBoxItems.Values)
            {
                boxItem?.Localize();
            }
        }

        void InitBoxItems()
        {
            mBoxItems = new Dictionary<string, CheapShop_BoxItem>();

            foreach(var data in MLand.GameData.BoxShopData.Values)
            {
                GameObject boxItemObj = gameObject.FindGameObject($"{data.boxType}");

                CheapShop_BoxItem boxItem = boxItemObj.GetOrAddComponent<CheapShop_BoxItem>();

                boxItem.Init(data);

                mBoxItems.Add(data.id, boxItem);
            }
        }
    }
}