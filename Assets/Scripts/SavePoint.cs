using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;

namespace MLand
{
	[Serializable]
	public class SavePoint : ISerializationCallbackReceiver
	{
		[SerializeField]
		ulong mBitFlags;
		[SerializeField]
		int mLastLoginDateNum;
		[SerializeField]
		int mSavedMTYTHighScore;

		public ObscuredULong BitFlags;					// 비트 플래그
		public ObscuredInt LastLoginDateNum;            // 로그인 날짜 저장
		public ObscuredInt SavedMTYTHighScore;			// 니편내편 최고 점수

		public Shop Shop;                               // 싸다 상점 관리
		public ExpensiveShop ExpensiveShop;				// 비싸다 상점 관리
		public Currency Currency;						// 재화 관리
		public Inventory Inventory;                     // 인벤토리 ( 호감도 아이템 )
		public Attendance Attendance;                   // 출석부
		public DailyQuest DailyQuest;                   // 일일 퀘스트
		public StepQuest StepQuest;						// 일반 단계 퀘스트
		public MiniGame MiniGame;						// 미니게임
		public SavedSlimeManager SlimeManager;          // 소환된 슬라임 관리
		public SavedBuildingManager BuildingManager;	// 건축된 건물 관리	
		public SavedBuffManager BuffManager;            // 버프 관리
		public OfflineRewardManager OfflineRewardManager;   // 오프라인 보상 관리
		public Achievements Achievements;                   // 업적
		public SavedLuckySymbol LuckySymbol;            // 네잎클로버 관련 저장
		public CostumeInventory CostumeInventory;       // 코스튬 인벤토리
		public string LangCodeStr;
		public LangCode LangCode;
		public SavePoint Normalize()
        {
			if (Shop == null)
            {
				Shop = new Shop();
				Shop.Init();
			}
			else
            {
				Shop.Normalize();
            }

			if (ExpensiveShop == null)
            {
				ExpensiveShop = new ExpensiveShop();
				ExpensiveShop.Normalize();
			}
			else
            {
				ExpensiveShop.Normalize();
            }

			if (Currency == null)
            {
				Currency = new Currency();
				Currency.Init();
			}
			else
            {
				Currency.Normalize();
            }
				

			if (Inventory == null)
            {
				Inventory = new Inventory();
				Inventory.Init();
			}
			else
            {
				Inventory.Normalize();
            }

			if (Attendance == null)
            {
				Attendance = new Attendance();
				Attendance.Init();
				Attendance.Check();
			}
			else
            {
				Attendance.Normalize();
				Attendance.Check();
			}
				

			if (DailyQuest == null)
            {
				DailyQuest = new DailyQuest();
				DailyQuest.Update();
			}
			else
            {
				DailyQuest.Update();
            }

			if (StepQuest == null)
            {
				StepQuest = new StepQuest();
				StepQuest.Init();
			}
			else
            {
				StepQuest.Normalize();
            }

			if (MiniGame == null)
            {
				MiniGame = new MiniGame();
				MiniGame.Init();
			}
			else
            {
				MiniGame.Normalize();
            }

			if (SlimeManager == null)
            {
				SlimeManager = new SavedSlimeManager();
				SlimeManager.Init();
			}
			else
            {
				SlimeManager.Normalize();
			}

			if (BuildingManager == null)
				BuildingManager = new SavedBuildingManager();

			if (BuffManager == null)
            {
				BuffManager = new SavedBuffManager();
				BuffManager.Normalize();
			}
			else
            {
				BuffManager.Normalize();
			}

			// 최초로 만든 계정이라면 플래그를 다음과 같이 정의해주자
			if (LastLoginDateNum == 0)
            {
				SavePointBitFlags.OnBGMSound.Set(true);
				SavePointBitFlags.OnEffectSound.Set(true);
			}

			if (OfflineRewardManager == null)
			{
				OfflineRewardManager = new OfflineRewardManager();
				OfflineRewardManager.Init();
			}
			else
            {
                OfflineRewardManager.Normalize();
			}

			if (Achievements == null)
            {
				Achievements = new Achievements();
				Achievements.Init();
			}
			else
            {
				Achievements.Normalize();
			}

			if (LuckySymbol == null)
            {
				LuckySymbol = new SavedLuckySymbol();
			}

			if (CostumeInventory == null)
            {
				CostumeInventory = new CostumeInventory();
				CostumeInventory.Init();

				// 업적 보상을 통해서 획득하는 코스튬이 있기때문에
				// 이미 업적 보상을 획득한 사람들은 코스튬을 바로 넣어주자
			}
			else
            {
				CostumeInventory.Normalize();

				// 업적 보상을 통해서 획득하는 코스튬이 있기때문에
				// 이미 업적 보상을 획득한 사람들은 코스튬을 바로 넣어주자
			}

			return this;
		}

		public bool AnyCanReceiveLevelReward()
        {
			return SlimeManager.AnyCanReceiveLevelReward();
		}

        public bool AnyCanReceiveLevelReward(string id)
        {
			return SlimeManager.AnyCanReceiveLevelReward(id);
        }

		public bool CanReceiveLevelReward(string id, int level)
        {
			return SlimeManager.CanReceiveLevelReward(id, level);
        }

		public void OnBeforeSerialize()
		{
			mBitFlags = BitFlags;
			mLastLoginDateNum = LastLoginDateNum;
			mSavedMTYTHighScore = SavedMTYTHighScore;
		}

		public void OnAfterDeserialize()
		{
			BitFlags = mBitFlags;
			LastLoginDateNum = mLastLoginDateNum;
			SavedMTYTHighScore = mSavedMTYTHighScore;
		}

		public void RandomizeKey()
        {
			BitFlags.RandomizeCryptoKey();
			LastLoginDateNum.RandomizeCryptoKey();
			SavedMTYTHighScore.RandomizeCryptoKey();

			Shop?.RandomizeKey();
			ExpensiveShop?.RandomizeKey();
			Currency?.RandomizeKey();
			Inventory?.RandomizeKey();
			Attendance?.RandomizeKey();
			DailyQuest?.RandomizeKey();
			StepQuest?.RandomizeKey();
			MiniGame?.RandomizeKey();
			SlimeManager?.RandomizeKey();
			BuildingManager?.RandomizeKey();
			BuffManager?.RandomizeKey();
			OfflineRewardManager?.RandomizeCryptoKey();
			Achievements?.RandomizeKey();
			LuckySymbol?.RandomizeCryptoKey();
			CostumeInventory?.RandomizeKey();

			this.Save();
		}

		public double GetTotalGold()
        {
			return Currency.GetTotalGold();
        }

        public void AddGold(double gold)
        {
			if ( gold > 0 )
            {
				Currency.AddGold(gold);
			}
        }

		public bool IsEnoughGold(double gold)
        {
			return Currency.IsEnoughGold(gold);
        }

        public bool UseGold(double gold)
        {
			return Currency.UseGold(gold);
        }

		public double GetTotalGem()
        {
			return Currency.GetTotalGem();
        }

		public void AddGem(double gem)
		{
			if ( gem > 0 )
            {
				Currency.AddGem(gem);
			}
		}

		public bool IsEnoughGem(double gem)
		{
			return Currency.IsEnoughGem(gem);
		}

		public bool UseGem(double gem)
		{
			return Currency.UseGem(gem);
		}

		public double GetSlimeCoreAmount(ElementalType type)
        {
			return Currency.GetSlimeCoreAmount(type);
        }

		public void AddSlimeCore(ElementalType type, double amount)
		{
			Currency.AddSlimeCore(type, amount);
		}

		public void AddSlimeCores(double[] amounts)
        {
			Currency.AddSlimeCores(amounts);
        }

		public bool IsEnoughSlimeCore(ElementalType type, double amount)
		{
			return Currency.IsEnoughSlimeCore(type, amount);
		}

        public float GetBuffValue(BuffType type)
        {
			return BuffManager.GetBuffValue(type);
        }

        public bool UseSlimeCore(ElementalType type, double amount)
		{
			return Currency.UseSlimeCore(type, amount);
		}

		public bool IsUnlockedSlime(string slimeId)
        {
			return SlimeManager.IsUnlockedSlime(slimeId);
		}

		public void AddSlime(string slimeId)
        {
			SlimeManager.AddSlime(slimeId);
        }

		public void RemoveSlime(string slimeId)
        {
			SlimeManager.RemoveSlime(slimeId);
        }

		public int GetSlimeLevel(string slimeId)
        {
			return SlimeManager.GetSlimeLevel(slimeId);
        }

		public void StackFriendShipExp(string slimeId, float exp)
        {
			SlimeManager.StackFriendShipExp(slimeId, exp);
        }

		public void StackLuckySymbolFriendShipExp(double exp)
        {
			LuckySymbol.StackExp(exp);
		}

		public double GetStatckedFriendShipExp(string slimeId)
        {
			return SlimeManager.GetStackedFriendShipExp(slimeId);
        }

		public int GetBuildingLevel(string buildingId)
        {
			return BuildingManager.GetBuildingLevel(buildingId);
        }

		public bool IsUnlockedBuilding(string buildingId)
        {
			return BuildingManager.IsUnlockedBuilding(buildingId);
		}

		public void AddBuilding(string buildingId)
        {
			BuildingManager.AddBuilding(buildingId);
		}

		public bool LevelUpBuilding(string buildingId)
        {
			return BuildingManager.LevelUpBuilding(buildingId);
		}

		public void AddCostumeItem(BoxOpenResult result)
        {
			CostumeInventory.AddCostume(result);
		}

		public void AddFriendShipItem(BoxOpenResult[] results)
        {
			Inventory.AddFriendShipItem(results);
		}

		public void AddFriendShipItem(string[] ids)
		{
			Inventory.AddFriendShipItem(ids);
		}

		public double UseFriendShipItem(string id, int amount)
        {
			return Inventory.UseFriendShipItem(id, amount);
        }

		public bool IsEnoughFriendShipItem(string id, int amount)
        {
			return Inventory.IsEnoughFriendShipItem(id, amount);
        }

		public void ActiveBuff(BuffType type, float duration)
        {
			BuffManager.ActiveBuff(type, duration);
		}

		public void ClearBuff(BuffType type)
        {
			BuffManager.ClearBuff(type);
		}

		public void SetCoolTimeBuff(BuffType type, float coolTime)
        {
			BuffManager.SetCoolTimeBuff(type, coolTime);
        }

		public bool IsReadyForReceiveAttendanceReward(int day)
        {
			return Attendance.IsReadyForReceiveReward(day);
        }

		public bool IsAlreadyReceiveAttendanceReward(int day)
        {
			return Attendance.IsAlreadyReceivedReward(day);
        }

		public void ReceiveAttendanceReward(int day)
        {
            AttendanceRewardData data = MLand.GameData.AttendanceRewardData.TryGet(day);
			if (data == null)
				return;

            RewardData rewardData = MLand.GameData.RewardData.TryGet(data.rewardId);
			if (rewardData == null)
				return;

			if ( Attendance.ReceiveReward(day) )
            {
				ReceiveReward(rewardData);

				MonsterLandUtil.ShowRewardPopup(rewardData);

				if (day == MLand.GameData.AttendanceRewardData.Values.Max(x => x.day))
                {
					// 개근상 업적 확인
					CheckAchievements(AchievementsType.PerfectAttendance);
                }

				this.Save();
			}
        }

		public void ReceiveDailyQuestReward(string id)
        {
			DailyQuestData dailyQuestData = MLand.GameData.DailyQuestData.TryGet(id);
			if (dailyQuestData == null)
				return;

			RewardData rewardData = MLand.GameData.RewardData.TryGet(dailyQuestData.rewardId);
			if (rewardData == null)
				return;

			if (DailyQuest.ReceiveReward(id))
            {
				ReceiveReward(rewardData);

				MonsterLandUtil.ShowRewardPopup(rewardData);

				CheckQuests(QuestType.DailyquestClear);
			}
        }

		public void ReceiveStepQuestReward(string id)
        {
			var info = StepQuest.StepQuestInfoDics.TryGet(id);
			if (info == null)
            {
				SoundPlayer.PlayErrorSound();

				return;
            }

			var data = info.CurrentData;
			if (data == null)
            {
				SoundPlayer.PlayErrorSound();

				return;
            }

			var rewardData = MLand.GameData.RewardData.TryGet(data.rewardId);
			if (rewardData == null)
            {
				SoundPlayer.PlayErrorSound();

				return;
            }

			if (StepQuest.ReceiveCurrentStepReward(id))
            {
				ReceiveReward(rewardData);

				MonsterLandUtil.ShowRewardPopup(rewardData);

				this.Save();
			}
        }

		public bool IsReceivedSlimeLevelReward(string id, int level)
        {
			var slimeInfo = SlimeManager.GetSlimeInfo(id);

			return slimeInfo?.IsReceivedLevelReward(level) ?? false;
        }

		public bool ReceiveSlimeLevelUpReward(string id, int level)
		{
			var slimeInfo = SlimeManager.GetSlimeInfo(id);
			if (slimeInfo == null)
				return false;

			var data = slimeInfo.ReceiveLevelReward(level);
			if (data == null)
				return false;

			if (data.rewardGem > 0)
            {
				RewardData rewardData = new RewardData()
				{
					gemReward = data.rewardGem,
				};

				ReceiveReward(rewardData);

				MonsterLandUtil.ShowRewardPopup(rewardData);

				this.Save();
			}

			return true;
		}

		public void ReceiveAchievementsReward(string id)
		{
			var info = Achievements.AchievementsDics.TryGet(id);
			if (info == null)
			{
				SoundPlayer.PlayErrorSound();

				return;
			}

			AchievementsData data = MLand.GameData.AchievementsData.TryGet(id);
			if (data == null)
				return;

			RewardData rewardData = MLand.GameData.RewardData.TryGet(data.rewardId);
			if (rewardData == null)
				return;

			if (Achievements.ReceiveReward(id))
			{
				ReceiveReward(rewardData);

				MonsterLandUtil.ShowRewardPopup(rewardData);

				this.Save();
			}
		}

		public BoxOpenResult[] ReceiveReward(RewardData reward)
        {
			AddGold(reward.goldReward);
			AddGem(reward.gemReward);

			if (reward.friendShipReward.IsValid())
            {
				for(int i = 0; i < reward.friendShipRewardCount; ++i)
					AddFriendShipItem(new string[] { reward.friendShipReward });
			}

			BoxOpenResult[] boxResults = null;
			if (reward.boxReward.IsValid())
            {
				List<BoxOpenResult> boxResultList = new List<BoxOpenResult>();
				for(int i = 0; i < reward.boxRewardCount; ++i)
                {
					var boxData = MLand.GameData.BoxData.TryGet(reward.boxReward);
					if (boxData != null)
                    {
						BoxOpenResult[] results = BuyBoxUtil.OpenBox(boxData, isFriendShip:true).ToArray();

						boxResultList.AddRange(results);

						AddFriendShipItem(results);
					}
				}

				boxResults = boxResultList.ToArray();
			}

			return boxResults;
		}

		public void CheckQuests(QuestType type, int count = 1)
        {
			bool quest = DailyQuest.CheckQuest(type, count);
			bool stepQuest = StepQuest.Stack(type, count);

			if (quest || stepQuest)
            {
				this.Save();

				MLand.Lobby?.RefreshNewDot();
			}
        }

		public void CheckAchievements(AchievementsType type, double count = 1)
        {
			if (Achievements.CheckAchievemets(type, count))
            {
				this.Save();

				MLand.Lobby?.RefreshNewDot();
            }
        }

		public bool StackWathAdBoxItem(string boxShopId)
        {
			return Shop.StackWatchAdBoxItem(boxShopId);
        }

		public bool CanWatchAdBoxItem(string boxShopId)
        {
			return Shop.CanWatchAdBoxItem(boxShopId);
        }

		public bool StackWatchAdGemItem(string gemShopId)
		{
			return Shop.StackWatchAdGemItem(gemShopId);
		}

		public bool CanWatchAdGemItem(string gemShopId)
		{
			return Shop.CanWatchAdGemItem(gemShopId);
		}

		public bool EquipCostume(string costumeId, string slimeId)
        {
			var costumeInfo = MLand.SavePoint.CostumeInventory.GetCostumeInfo(costumeId);
			if (costumeInfo == null)
            {
				// 아직 획득하지 못한 코스튬 입니다.
				return false;
            }

			var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(slimeId);
			if (slimeInfo == null)
            {
				// 아직 소환하지 않은 슬라임 입니다.
				return false;
            }

			var costumeData = MLand.GameData.CostumeData.TryGet(costumeId);
			if (costumeData == null)
            {
				return false;
            }

			// 1. 착용하려는 코스튬을 장착하고 있는 슬라임이 있는지 확인한다.
			string equippedSlime = costumeInfo.EquipedSlimeId;
			if (equippedSlime.IsValid())
            {
				// 1-1. 착용중인 슬라임이 있다면 장착을 해제해준다.
				var equippedSlimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(equippedSlime);
				if (equippedSlimeInfo == null)
					return false;

				costumeInfo.UnEquipCostume();
				equippedSlimeInfo.UnEquipCostume(costumeData.costumeType);
            }

			// 2. 착용하려는 슬라임의 착용 부위에 착용중인 코스튬이 있는지 확인한다.
			CostumeInfo equippedCostumeInfo = MLand.SavePoint.CostumeInventory.FindEquippedCostume(slimeId, costumeData.costumeType);
			if (equippedCostumeInfo != null)
            {
				// 2-1. 이미 착용중인 코스튬이 있다면 장착을 해제해준다.
				equippedCostumeInfo.UnEquipCostume();
				slimeInfo.UnEquipCostume(costumeData.costumeType);
			}

			// 3. 코스튬을 장착한다.
			slimeInfo.EquipCostume(costumeData.costumeType, costumeId);
			costumeInfo.EquipCostume(slimeId);

			this.Save();

			return true;
		}

		public bool UnEquipCostume(string costumeId, string slimeId)
        {
			var costumeInfo = MLand.SavePoint.CostumeInventory.GetCostumeInfo(costumeId);
			if (costumeInfo == null)
			{
				// 아직 획득하지 못한 코스튬 입니다.
				return false;
			}

			var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(slimeId);
			if (slimeInfo == null)
			{
				// 아직 소환하지 않은 슬라임 입니다.
				return false;
			}

			var costumeData = MLand.GameData.CostumeData.TryGet(costumeId);
			if (costumeData == null)
			{
				return false;
			}

			// 해제
			slimeInfo.UnEquipCostume(costumeData.costumeType);
			costumeInfo.UnEquipCostume();

			this.Save();

			return true;
		}

		IEnumerable<CostumeStatData> GetSlimeEquippedCostumeStatDatas(string slimeId)
        {
			SavedSlimeInfo slimeInfo = SlimeManager.GetSlimeInfo(slimeId);
			if (slimeInfo != null)
			{
				if (slimeInfo.Costumes != null)
				{
					foreach (var costume in slimeInfo.Costumes)
					{
						string costumeId = costume;
						if (costumeId.IsValid())
						{
							CostumeInfo costumeInfo = CostumeInventory.GetCostumeInfo(costumeId);
							if (costumeInfo != null)
							{
								yield return DataUtil.GetCostumeStatData(costumeId, costumeInfo.Level);
							}
						}
					}
				}
			}
		}

		public double GetSlimeCoreDropAmountByCostume(string slimeId)
        {
			double totalDropAmount = 0;

			foreach (var data in GetSlimeEquippedCostumeStatDatas(slimeId))
				totalDropAmount += data.slimeCoreDropAmount;

			return totalDropAmount;
		}

		public float GetSlimeCoreDropCoolTimeByCostume(string slimeId)
		{
			float totalDropCoolTime = 0;

			foreach (var data in GetSlimeEquippedCostumeStatDatas(slimeId))
				totalDropCoolTime += data.slimeCoreDropCoolTime;

			return totalDropCoolTime;
		}
	}

	static class SavePointUtil
	{
		const string DataPrefKey = "3094kdfjskd";            // 아무 의미없는 값
		public static SavePoint Load()
		{
			try
			{
				string base64 = ObscuredPrefs.GetString(DataPrefKey, null);
				if (base64 != string.Empty)
				{
					SavePoint data = JsonUtil.Base64FromJsonUnity<SavePoint>(base64);
					if (data != null)
					{
						return data;
					}
				}
			}
			catch
			{
			}

			return MakeNewSavePoint(null);
		}

		static SavePoint MakeNewSavePoint(SavePoint prev)
		{
            SavePoint newSavePoint = new SavePoint();

			return newSavePoint;
		}

		public static void Save(this SavePoint data)
		{
			string base64 = data.ChangeToSavedStr();

			ObscuredPrefs.SetString(DataPrefKey, base64);
		}

		public static string ChangeToSavedStr(this SavePoint data)
        {
			string json = JsonUtil.ToJsonUnity(data);

			return JsonUtil.JsonToBase64String(json);
		}

		public static void Reset()
		{
			var prev = MLand.SavePoint;

			MLand.SavePoint = MakeNewSavePoint(prev);
			MLand.SavePoint.Normalize();

			Debug.Log("SavePoint: Reset");
		}

		public static void Delete()
		{
			PlayerPrefs.DeleteKey(DataPrefKey);
		}
	}

    public enum SavePointBitFlags
    {
		OnBGMSound,
		OnEffectSound,

		Tutorial_1_SpawnSlime,
		Tutorial_2_BuildBuilding,
		Tutorial_3_CheapShop,
		Tutorial_4_MiniGame,
		Tutorial_5_ExpensiveShop,
		Tutorial_6_GoldSlime,

		// 아래로 추가할 것~
		RequestReview,
		AchievementsHelpShow,
		ShowSlimeCoreGetAmount,

		Count,
    }

    static class SavePointBitFlagsExt
    {
        public static void Set(this SavePointBitFlags flag, bool value = true)
        {
            ulong mask = 1UL << (int)flag;

            if (value)
                MLand.SavePoint.BitFlags |= mask;
            else
                MLand.SavePoint.BitFlags &= ~mask;

            MLand.SavePoint.Save();
        }

        public static bool Get(this SavePointBitFlags flag)
        {
            ulong mask = 1UL << (int)flag;

            return (MLand.SavePoint.BitFlags & mask) != 0;
        }

        public static void Toggle(this SavePointBitFlags flag)
        {
            flag.Set(!flag.Get());
        }

        public static bool IsOn(this SavePointBitFlags flag)
        {
            return flag.Get();
        }

        public static bool IsOff(this SavePointBitFlags flag)
        {
            return flag.Get() == false;
        }
    }
}