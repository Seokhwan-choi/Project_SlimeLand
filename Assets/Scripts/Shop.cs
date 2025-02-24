using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;


namespace MLand
{
	[Serializable]
	public class Shop : ISerializationCallbackReceiver
	{
		[SerializeField]
		int mPriceChangeTime;
		[SerializeField]
		int[] mSlimeCorePrices;
		[SerializeField]
		float[] mSlimeCorePriceValues;

		public ObscuredInt PriceChangeTime;
		public ObscuredInt[] SlimeCorePrices;
		public ObscuredFloat[] SlimeCorePriceValues;
		public BoxShopItem[] BoxShopItems;
		public GemShopItem GemShopItem;
		public int UpdateRemainTime => Math.Max(0, PriceChangeTime - TimeUtil.Now);

        public void Init()
        {
			Normalize();
		}

		public void Normalize()
        {
			int count = (int)ElementalType.Count;

			if (SlimeCorePrices == null || SlimeCorePriceValues == null)
            {
				SlimeCorePrices = new ObscuredInt[count];
				SlimeCorePriceValues = new ObscuredFloat[count];
			}

			if (GemShopItem == null)
			{
				var gemShopData = MLand.GameData.GemShopData.Values.Where(x => x.watchAdDailyCount > 0).FirstOrDefault();
				if (gemShopData == null)
					return;

				GemShopItem = new GemShopItem()
				{
					Id = gemShopData.id,
					DailyWatchAdCounter = new DailyCounter()
				};
			}
			else
			{
				var gemShopData = MLand.GameData.GemShopData.Values.Where(x => x.watchAdDailyCount > 0).FirstOrDefault();
				if (gemShopData == null)
					return;

				GemShopItem.Id = gemShopData.id;
			}

			if (BoxShopItems == null)
            {
				InitBoxShopItems();
			}
			else
            {
				if (BoxShopItems.Length != MLand.GameData.BoxShopData.Count)
                {
					InitBoxShopItems();
				}
				else if (BoxShopItems.Where(x => x.Id == "BoxShop_Costume").FirstOrDefault() == null)
                {
					InitBoxShopItems();
				}
            }

			void InitBoxShopItems()
            {
				int boxShopItemCount = MLand.GameData.BoxShopData.Count;

				BoxShopItems = new BoxShopItem[boxShopItemCount];

				int index = 0;

				foreach (var boxShopData in MLand.GameData.BoxShopData.Values)
				{
					var boxShopItem = new BoxShopItem()
					{
						Id = boxShopData.id,
						DailyWatchAdCounter = new DailyCounter()
					};

					BoxShopItems[index] = boxShopItem;

					index++;
				}
			}
        }
		public void OnBeforeSerialize()
		{
			mPriceChangeTime = PriceChangeTime;

			if (SlimeCorePrices != null)
            {
				mSlimeCorePrices = SlimeCorePrices.Select(x => (int)x).ToArray();
			}

			if (SlimeCorePriceValues != null)
            {
				mSlimeCorePriceValues = SlimeCorePriceValues.Select(x => (float)x).ToArray();
			}
		}

		public void OnAfterDeserialize()
		{
            PriceChangeTime = mPriceChangeTime;

            if (mSlimeCorePrices != null)
            {
                SlimeCorePrices = mSlimeCorePrices.Select(x => (ObscuredInt)x).ToArray();
            }

            if (mSlimeCorePriceValues != null)
            {
                SlimeCorePriceValues = mSlimeCorePriceValues.Select(x => (ObscuredFloat)x).ToArray();
            }
        }

		public void RandomizeKey()
        {
			PriceChangeTime.RandomizeCryptoKey();

			if (SlimeCorePrices != null)
            {
				for (int i = 0; i < SlimeCorePrices.Length; ++i)
				{
					SlimeCorePrices[i].RandomizeCryptoKey();
				}
			}

			if (SlimeCorePriceValues != null)
            {
				for(int i = 0; i < SlimeCorePriceValues.Length; ++i)
                {
					SlimeCorePriceValues[i].RandomizeCryptoKey();
				}
            }

			if (BoxShopItems != null)
            {
				for(int i = 0; i < BoxShopItems.Length; ++i)
                {
					BoxShopItems[i].RandomizeKey();
				}
            }

			GemShopItem?.RandomizeKey();
		}

		public bool UpdateSlimeCorePrice()
		{
			if (TimeUtil.Now >= PriceChangeTime)
			{
				PriceChangeTime = TimeUtil.Now + (MLand.GameData.ShopCommonData.slimeCorePriceChangeHour * TimeUtil.SecondsInHour);

				RefreshSlimeCorePrice();

				return true;
			}

			return false;
		}

		void RefreshSlimeCorePrice()
		{
			if (SlimeCorePrices == null)
				SlimeCorePrices = new ObscuredInt[(int)ElementalType.Count];

			if (SlimeCorePriceValues == null)
				SlimeCorePriceValues = new ObscuredFloat[(int)ElementalType.Count];

			float rangeValue = MLand.GameData.ShopCommonData.slimeCorePriceRangeValue;

			for (int i = 0; i < (int)ElementalType.Count; ++i)
			{
				float randValue = UnityEngine.Random.Range(1f - rangeValue,1f + rangeValue);

				SlimeCorePriceValues[i] = randValue;
				SlimeCorePrices[i] = Mathf.RoundToInt(MLand.GameData.ShopCommonData.slimeCoreDefaultPrice * randValue);
			}
		}

		public double GetSlimeCoreTotalPrice(ElementalType type)
        {
			double amount = MLand.SavePoint.GetSlimeCoreAmount(type);

			return GetSlimeCorePrice(type) * amount;
		}

		public double GetSlimeCorePrice(ElementalType type)
        {
			if (SlimeCorePrices == null)
				return 0;

			if (SlimeCorePrices.Length <= (int)type)
				return 0;

			return SlimeCorePrices[(int)type];
        }

		BoxOpenResult[] GetBoxOpenResults(string id, BoxOpenType openType)
        {
			BoxShopData boxShopData = MLand.GameData.BoxShopData.TryGet(id);
			if (boxShopData == null)
				return null;

			string boxId = boxShopData.boxId[(int)openType];

			BoxData boxData = MLand.GameData.BoxData.TryGet(boxId);
			if (boxData == null)
				return null;

			bool isFriendShip = boxShopData.boxType == BoxType.FriendShip;

			return BuyBoxUtil.OpenBox(boxData, isFriendShip).ToArray();
		}

		public BoxOpenResult[] BuyBox(string id, BoxOpenType openType)
        {
            BoxShopData boxShopData = MLand.GameData.BoxShopData.TryGet(id);
			if (boxShopData == null)
				return null;

			bool isFriendShip = boxShopData.boxType == BoxType.FriendShip;
			BoxOpenResult[] boxOpenResults = GetBoxOpenResults(id, openType);
			if (boxOpenResults == null)
				return null;

			if (openType == BoxOpenType.Ad)
			{
				bool usedWatchAdCount = MLand.SavePoint.StackWathAdBoxItem(boxShopData.id);
				if (usedWatchAdCount == false)
					return null;
			}
            else
            {
				double gemPrice = boxShopData.gemPrice[(int)openType];

				bool usedGem = MLand.SavePoint.UseGem(gemPrice);
				if (usedGem == false)
					return null;
			}

			if (isFriendShip)
            {
				MLand.SavePoint.AddFriendShipItem(boxOpenResults);
			}
			else
            {
				// 초과 되어서 획득한 코스튬이 있다면 골드로 치환해주자
				foreach (var boxOpenResult in boxOpenResults)
                {
					MLand.SavePoint.AddCostumeItem(boxOpenResult);
					MLand.SavePoint.AddGold(boxOpenResult.ReturnGold);
				}
			}

			MLand.Lobby.RefreshGemText();
			MLand.Lobby.RefreshGoldText();

			MLand.SavePoint.Save();

			return boxOpenResults;
        }

		public bool StackWatchAdBoxItem(string id)
        {
			if (BoxShopItems == null)
				return false;

            BoxShopItem boxItem = BoxShopItems.Where(x => x.Id == id).FirstOrDefault();
			if (boxItem == null)
				return false;

			return boxItem.StackWathAdCount();
        }

        public bool CanWatchAdBoxItem(string boxShopId)
        {
			if (BoxShopItems == null)
				return false;

			BoxShopItem boxItem = BoxShopItems.Where(x => x.Id == boxShopId).FirstOrDefault();
			if (boxItem == null)
				return false;

			return boxItem.CanWatchAd();
		}

		public int GetRemainWatchAdCount(string boxShopId)
        {
			if (BoxShopItems == null)
				return 0;

			BoxShopItem boxItem = BoxShopItems.Where(x => x.Id == boxShopId).FirstOrDefault();
			if (boxItem == null)
				return 0;

			return boxItem.GetRemainWatchAdCount();
		}

		public bool StackWatchAdGemItem(string id)
		{
			if (GemShopItem == null)
				return false;

			if (GemShopItem.Id != id)
				return false;

			return GemShopItem.StackWatchAdCount();
		}

		public bool CanWatchAdGemItem(string gemShopId)
		{
			if (GemShopItem == null)
				return false;

			if (GemShopItem.Id != gemShopId)
				return false;

			return GemShopItem.CanWatchAd();
		}
	}

    [Serializable]
    public class BoxShopItem : ISerializationCallbackReceiver
    {
		[SerializeField]
		string mId;

		public ObscuredString Id;
		public DailyCounter DailyWatchAdCounter;
		public BoxShopData Data => MLand.GameData.BoxShopData.TryGet(Id);
		public bool StackWathAdCount()
        {
			if (Data.watchAdDailyCount <= 0)
				return false;

			return DailyWatchAdCounter.StackCount(Data.watchAdDailyCount);
		}

		public void RandomizeKey()
        {
			Id?.RandomizeCryptoKey();
			DailyWatchAdCounter?.RandomizeKey();
		}

		public void OnBeforeSerialize()
		{
			if (Id != null)
				mId = Id;
		}

		public void OnAfterDeserialize()
        {
			if (mId.IsValid())
				Id = mId;
		}

		public int GetRemainWatchAdCount()
        {
			return DailyWatchAdCounter.GetRemainCount(Data.watchAdDailyCount);
		}

        public bool CanWatchAd()
        {
			return DailyWatchAdCounter.IsMaxCount(Data.watchAdDailyCount) == false;
        }
    }

	[Serializable]
	public class GemShopItem : ISerializationCallbackReceiver
	{
		[SerializeField]
		string mId;

		public ObscuredString Id;
		public DailyCounter DailyWatchAdCounter;
		GemShopData Data => MLand.GameData.GemShopData.TryGet(Id);
		public bool StackWatchAdCount()
		{
			if (Data.watchAdDailyCount <= 0)
				return false;

			return DailyWatchAdCounter.StackCount(Data.watchAdDailyCount);
		}

		public void RandomizeKey()
		{
			Id?.RandomizeCryptoKey();
			DailyWatchAdCounter?.RandomizeKey();
		}

		public void OnBeforeSerialize()
		{
			if (Id != null)
				mId = Id;
		}

		public void OnAfterDeserialize()
		{
			if (mId.IsValid())
				Id = mId;
		}

		public bool CanWatchAd()
		{
			return DailyWatchAdCounter.IsMaxCount(Data.watchAdDailyCount) == false;
		}
	}

	public class BoxOpenResult
    {
		public string Id;
		public ItemType Type;
		public int Amount;
		public double ReturnGold;
    }
}