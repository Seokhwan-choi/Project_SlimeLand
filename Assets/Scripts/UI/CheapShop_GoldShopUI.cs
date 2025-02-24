using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class CheapShop_GoldShopUI : CheapShopTabUI
    {
        Dictionary<string, CheapShop_GoldItem> mGoldItems;

        public override void Init(CheapShopTabUIManager parent)
        {
            base.Init(parent);

            mGoldItems = new Dictionary<string, CheapShop_GoldItem>();

            foreach (GoldShopData goldShopData in MLand.GameData.GoldShopData.Values)
            {
                string id = goldShopData.id;

                GameObject goldItemObj = gameObject.FindGameObject(id);

                CheapShop_GoldItem goldItem = goldItemObj.GetOrAddComponent<CheapShop_GoldItem>();

                goldItem.Init(id);

                mGoldItems.Add(id, goldItem);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            float dt = Time.deltaTime;

            foreach (var goldItem in mGoldItems.Values)
            {
                goldItem.OnUpdate(dt);
            }
        }

        public override void Localize()
        {
            foreach (var goldItem in mGoldItems.Values)
            {
                goldItem.Localize();
            }
        }
    }
}