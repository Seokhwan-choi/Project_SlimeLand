using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class ExpensiveItem : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mIndex;
        [SerializeField]
        string mId;
        [SerializeField]
        double mPrice;
        [SerializeField]
        bool mIsAlreadyPurchased;

        public ObscuredInt Index;
        public ObscuredString Id;
        public ObscuredDouble Price;
        public ObscuredBool IsAlreadyPurchased;
        public string Key => $"{Id}_{Index}";

        public void OnBeforeSerialize()
        {
            mIndex = Index;
            mId = Id;
            mPrice = Price;
            mIsAlreadyPurchased = IsAlreadyPurchased;
        }

        public void OnAfterDeserialize()
        {
            Index = mIndex;
            Id = mId;
            Price = mPrice;
            IsAlreadyPurchased = mIsAlreadyPurchased;
        }

        public void RandomizeKey()
        {
            Index.RandomizeCryptoKey();
            Id?.RandomizeCryptoKey();
            Price.RandomizeCryptoKey();
            IsAlreadyPurchased.RandomizeCryptoKey();
        }
    }

    [Serializable]
    public class ExpensiveShop : ISerializationCallbackReceiver
    {
        [SerializeField]
        int mLastUpdateTime;
        [SerializeField]
        int mPosAreaInt;
        [SerializeField]
        bool mIsActive;

        public ExpensiveItem[] Items;
        public ObscuredInt LastUpdateTime;
        public ObscuredInt PosAreaInt;
        public ObscuredBool IsActive;
        public DailyCounter DailyCounter;       // 즉시 갱신 횟수 제한
        public ElementalType PosArea => (ElementalType)PosAreaInt.GetDecrypted();
        public void Normalize()
        {
            if(DailyCounter == null)
            {
                DailyCounter = new DailyCounter();
            }
        }
        public void OnBeforeSerialize()
        {
            mLastUpdateTime = LastUpdateTime;
            mPosAreaInt = PosAreaInt;
            mIsActive = IsActive;
        }
        public void OnAfterDeserialize()
        {
            LastUpdateTime = mLastUpdateTime;
            PosAreaInt = mPosAreaInt;
            IsActive = mIsActive;
        }
        public void RandomizeKey()
        {
            if (Items != null)
            {
                foreach (var item in Items)
                    item.RandomizeKey();
            }

            LastUpdateTime.RandomizeCryptoKey();
            PosAreaInt.RandomizeCryptoKey();
            IsActive.RandomizeCryptoKey();
            DailyCounter?.RandomizeKey();
        }

        public bool IsSatisfied()
        {
            string buildingId = MLand.GameData.ShopCommonData.expensiveShopPrecendingBuilding;
            int requireLevel = MLand.GameData.ShopCommonData.expensiveShopPrecendingBuildingLevel;

            bool isUnlocked = MLand.SavePoint.BuildingManager.IsUnlockedBuilding(buildingId);
            if (isUnlocked == false)
                return false;

            int buildingLevel = MLand.SavePoint.BuildingManager.GetBuildingLevel(buildingId);
            if (buildingLevel < requireLevel)
                return false;

            if (IsActive == false)
            {
                IsActive = true;
                LastUpdateTime = TimeUtil.Now;
            }

            return true;
        }
        public int GetUpdateRemainTime()
        {
            int nextUpdateSec = 0;
            if (SavePointBitFlags.Tutorial_5_ExpensiveShop.IsOff())
                nextUpdateSec = MLand.GameData.ShopCommonData.expensiveFirstOpenWaitMinute * TimeUtil.SecondsInMinute;
            else
                nextUpdateSec = MLand.GameData.ShopCommonData.expensiveShopOpenPeriodHour * TimeUtil.SecondsInHour;

            int nextUpdateTime = LastUpdateTime + nextUpdateSec;
            if ( nextUpdateTime > TimeUtil.Now )
                return nextUpdateTime - TimeUtil.Now;
            else
                return 0;
        }

        public void PurchaseItem(string key)
        {
            if (IsSatisfied() == false)
                return;

            ExpensiveItem item = Items.Where(x => x.Key == key).FirstOrDefault();
            if (item == null)
                return;

            item.IsAlreadyPurchased = true;
        }

        public bool CheckUpdateShop()
        {
            if (IsSatisfied() == false)
                return false;

            if (GetUpdateRemainTime() <= 0)
            {
                UpdateShop(immediately:false);

                return true;
            }

            return false;
        }

        public bool IsEnoughImmediatelyUpdateCount()
        {
            int maxCount = MLand.GameData.ShopCommonData.expensiveShopUpdateDailyCount;

            return DailyCounter?.IsMaxCount(maxCount) == false;
        }

        public void UpdateShop(bool immediately)
        {
            if (IsSatisfied() == false)
                return;

            if (immediately)
            {
                int maxCount = MLand.GameData.ShopCommonData.expensiveShopUpdateDailyCount;

                if (DailyCounter.StackCount(maxCount))
                {
                    InternalUpdate();
                }
            }
            else
            {
                InternalUpdate();
                // 즉시 갱신이 아니라면 위치를 바꿔주자
                PosAreaInt = UnityEngine.Random.Range(0, (int)ElementalType.Count);
            }

            void InternalUpdate()
            {
                CreateNewItems();

                LastUpdateTime = TimeUtil.Now;
            }
        }

        void CreateNewItems()
        {
            int amount = MLand.GameData.ShopCommonData.expensiveShopItemCount;

            Items = new ExpensiveItem[amount];

            var data = MLand.GameData.ExpensiveShopItemProbData;

            for (int i = 0; i < amount; ++i)
            {
                ItemType type = (ItemType)Util.RandomChoose(data.itemTypeProb);
                ItemGrade grade = (ItemGrade)Util.RandomChoose(data.gradeProb);

                FriendShipItemData itemData = DataUtil.GetRandomFriendShipItemData(type, grade);

                double slimeCoreForMinute = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(itemData.goldPriceforMinute);
                double goldForMinute = slimeCoreForMinute * MLand.GameData.ShopCommonData.slimeCoreDefaultPrice;

                Items[i] = new ExpensiveItem()
                {
                    Index = i,
                    Id = itemData.id,
                    Price = goldForMinute,
                    IsAlreadyPurchased = false,
                };
            }
        }
    }
}