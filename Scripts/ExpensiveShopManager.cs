using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class ExpensiveShopManager
    {
        BuildingManager mBuildingManager;
        ExpensiveShopClicker[] mExpensiveShops;
        public void Init(BuildingManager buildingManager)
        {
            int shopCount = (int)ElementalType.Count;

            GameObject parent = GameObject.Find("Buildings");
            mBuildingManager = buildingManager;
            mExpensiveShops = new ExpensiveShopClicker[shopCount];
            for(int i = 0; i < shopCount; ++i)
            {
                ElementalType type = (ElementalType)i;

                var shopObj = parent.FindGameObject($"ExpensiveShop_{type}");

                mExpensiveShops[i] = shopObj.GetOrAddComponent<ExpensiveShopClicker>();
                mExpensiveShops[i].SetActive(false);
            }

            RefreshShops(isShowMessage:false);
        }

        public ExpensiveShopClicker GetActiveExpensiveShop()
        {
            foreach(var expensiveShop in mExpensiveShops)
            {
                if (expensiveShop.gameObject.activeSelf)
                    return expensiveShop;
            }

            return null;
        }

        public void OnUpdate()
        {
            Popup_ExpensiveShopUI popup = MLand.PopupManager.GetPopup<Popup_ExpensiveShopUI>();
            if ( popup != null )
            {
                if ( popup.CheckUpdateShop() )
                    RefreshShops();
            }
            else
            {
                if ( MLand.SavePoint.ExpensiveShop.CheckUpdateShop() )
                    RefreshShops();
            }
        }

        public void RefreshShops(bool isShowMessage = true, bool buildNavMesh = true)
        {
            if (MLand.SavePoint.ExpensiveShop.IsSatisfied() == false)
                return;

            for(int i = 0; i < mExpensiveShops.Length; ++i)
            {
                bool isActive = i == (int)MLand.SavePoint.ExpensiveShop.PosArea;

                mExpensiveShops[i].SetActive(isActive);

                if (isActive)
                {
                    foreach(var collider in mExpensiveShops[i].GetComponentsInChildren<Collider2D>())
                    {
                        collider.enabled = true;
                    }
                }
            }

            if (SavePointBitFlags.Tutorial_5_ExpensiveShop.IsOff())
                MLand.Lobby?.StartTutorial($"{SavePointBitFlags.Tutorial_5_ExpensiveShop}");

            if (isShowMessage)
            {
                string message = StringTableUtil.GetSystemMessage("AppearExpensiveShop");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayAppearExpensiveShop();
            }

            if (buildNavMesh)
            {
                mBuildingManager.BuildNavMesh();
            }
        }
    }
}