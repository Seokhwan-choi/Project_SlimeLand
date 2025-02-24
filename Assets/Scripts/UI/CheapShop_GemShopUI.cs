using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLand
{
    class CheapShop_GemShopUI : CheapShopTabUI
    {
        List<CheapShop_GemItem> mGemItemList;
        public override void Init(CheapShopTabUIManager parent)
        {
            base.Init(parent);

            mGemItemList = new List<CheapShop_GemItem>();

            for (int i = 0; i < MLand.GameData.ShopCommonData.gemShopSlotCount; ++i)
            {
                int slot = i + 1;

                IEnumerable<GemShopData> datas = MLand.GameData.GemShopData.Values.Where(x => x.slot == slot);

                GemShopData data = datas.OrderBy(x => x.priority).FirstOrDefault();

                GameObject itemObj = gameObject.FindGameObject($"GemItem_{slot}");

                CheapShop_GemItem item = itemObj.GetOrAddComponent<CheapShop_GemItem>();

                item.Init(data.id);

                mGemItemList.Add(item);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            float dt = Time.deltaTime;

            foreach(var gemItem in mGemItemList)
            {
                gemItem.OnUpdate(dt);
            }
        }

        public override void Localize()
        {
            foreach (var gemItem in mGemItemList)
            {
                gemItem.Localize();
            }
        }
    }
}