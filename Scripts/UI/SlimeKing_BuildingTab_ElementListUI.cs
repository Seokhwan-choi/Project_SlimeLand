using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace MLand
{
    class SlimeKing_BuildingTab_ElementListUI : MonoBehaviour
    {
        Dictionary<string, SlimeKing_BuildingTab_ElementUI> mBuildingDics;
        public void Init(ElementalType type)
        {
            mBuildingDics = new Dictionary<string, SlimeKing_BuildingTab_ElementUI>();

            BuildingData[] datas = DataUtil.GetBuildingDatasByType(type).ToArray();
            for (int i = 0; i < datas.Length; ++i)
            {
                bool isCentral = datas[i].isCentralBuilding;

                string name = isCentral ? "Central" : $"Building_{i}";

                GameObject buildingElementObj = gameObject.FindGameObject(name);

                var buildingElement = buildingElementObj.GetOrAddComponent<SlimeKing_BuildingTab_ElementUI>();

                buildingElement.Init(datas[i].id);

                mBuildingDics.Add(datas[i].id, buildingElement);
            }

            Refresh();
        }

        public void Refresh()
        {
            foreach(var element in mBuildingDics.Values)
            {
                element.Refresh();
            }
        }

        public void Localize()
        {
            foreach (var element in mBuildingDics.Values)
            {
                element.Localize();
            }
        }

        public SlimeKing_BuildingTab_ElementUI GetElement(string id)
        {
            return mBuildingDics.TryGet(id);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}