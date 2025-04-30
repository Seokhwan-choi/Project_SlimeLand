using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace MLand
{
    static class DataUtil
    {
        public static IEnumerable<BuildingData> GetAllCentralBuildingDatas()
        {
            return MLand.GameData.BuildingData.Values.Where(x => x.isCentralBuilding);
        }
        public static IEnumerable<BuildingData> GetBuildingDatasByType(ElementalType type)
        {
            return MLand.GameData.BuildingData.Values.Where(x => x.elementalType == type);
        }

        public static BuildingStatData GetBuildingStatData(string id)
        {
            int level = MLand.SavePoint.GetBuildingLevel(id);

            level = Math.Max(1, level);

            return GetBuildingStatData(id, level);
        }

        public static int GetBuildingMaxLevel(string id)
        {
            return MLand.GameData.BuildingStatData.Where(x => x.id == id).Max(x => x.level);
        }

        public static BuildingStatData GetBuildingStatData(string id, int level)
        {
            return MLand.GameData.BuildingStatData.Find(x => x.id == id && x.level == level);
        }

        public static BuildingUpgradeData GetBuildingUpgradeData(string id, int nextLevel)
        {
            return MLand.GameData.BuildingUpgradeData.Find(x => x.id == id && x.level == nextLevel);
        }

        public static IEnumerable<CharacterData> GetSlimeDatasByType(ElementalType type)
        {
            return MLand.GameData.SlimeData.Values.Where(x => x.elementalType == type);
        }

        public static string GetBoxImg(string boxShopId)
        {
            BoxShopData boxShopData = MLand.GameData.BoxShopData.TryGet(boxShopId);

            Debug.Assert(boxShopData != null, $"{boxShopId}의 BoxShopData가 존재 하지 않음");

            return boxShopData.spriteImg;
        }

        public static FriendShipItemData GetRandomFriendShipItemData(ItemType type, ItemGrade grade)
        {
            IEnumerable<FriendShipItemData> items = MLand.GameData.FriendShipItemData.Values.Where(x => x.itemType == type && x.grade == grade);

            return GetShuffleDatas(items).FirstOrDefault();
        }

        public static CostumeData GetRandomCostumeData(CostumeType type)
        {
            IEnumerable<CostumeData> items = MLand.GameData.CostumeData.Values.Where(x => x.costumeType == type);

            return GetShuffleDatas(items).FirstOrDefault();
        }

        public static IOrderedEnumerable<T> GetShuffleDatas<T>(IEnumerable<T> datas)
        {
            System.Random random = new System.Random();

            return datas.OrderBy(x => random.Next());
        }

        public static IEnumerable<DailyQuestData> GetDailyQuestDatasByType(QuestType type)
        {
            return MLand.GameData.DailyQuestData.Values.Where(x => x.questType == type);
        }

        public static StepQuestData GetStepQuestData(string id, int step)
        {
            return MLand.GameData.StepQuestData.Where(x => x.id == id && x.step == step).FirstOrDefault();
        }

        public static QuestType GetQuestType(string id)
        {
            DailyQuestData dailyQuestData = MLand.GameData.DailyQuestData.TryGet(id);
            if (dailyQuestData == null)
            {
                StepQuestData stepQuestData = MLand.GameData.StepQuestData.Where(x => x.id == id).FirstOrDefault();
                if (stepQuestData == null)
                    return QuestType.Count;
                else
                    return stepQuestData.type;
            }
            else
            {
                return dailyQuestData.questType;
            }
        }

        public static int GetStepQuestMaxStep(string id)
        {
            return MLand.GameData.StepQuestData.Where(x => x.id == id).Max(x => x.step);
        }

        public static RewardData GetMiniGameRewardData(int score)
        {
            int maxScore = 0;
            string rewardId = string.Empty;
            RewardData rewardData = null;

            foreach (var data in MLand.GameData.MiniGameRewardData.Values)
            {
                MinMaxRange scoreRange = MinMaxRange.Parse(data.score);
                if (scoreRange.IsInRange(score))
                {
                    if (maxScore < scoreRange.max)
                    {
                        maxScore = scoreRange.max;
                        rewardId = data.rewardId;
                    }
                }
            }

            if ( rewardId.IsValid() )
                rewardData = MLand.GameData.RewardData.TryGet(rewardId);

            return rewardData;
        }


        public static SlimeLevelUpRewardData GetLevelUpRewardData(int slimeLevel, int level)
        {
            return GetSlimeLevelUpRewardDataBySlimeLevel(slimeLevel).Where(x => x.level == level).FirstOrDefault();
        }

        public static IEnumerable<SlimeLevelUpRewardData> GetAllLevelUpReward(int slimeLevel, int level)
        {
            foreach(SlimeLevelUpRewardData data in GetSlimeLevelUpRewardDataBySlimeLevel(slimeLevel))
            {
                if (data.level <= level)
                    yield return data;
            }
        }

        public static IEnumerable<SlimeLevelUpRewardData> GetSlimeLevelUpRewardDataBySlimeLevel(int slimeLevel)
        {
            return MLand.GameData.SlimeLevelUpRewardData.Where(x => x.slimeLevel == slimeLevel);
        }

        public static IEnumerable<CharacterData> GetAllSlimeDatas(bool includeGoldSlime = false)
        {
            foreach(var data in MLand.GameData.SlimeData.Values)
            {
                if (includeGoldSlime == false)
                {
                    if (data.id == MLand.GameData.GoldSlimeCommonData.id)
                        continue;
                }

                yield return data;
            }
        }

        public static bool CanAttendMiniGame(string id)
        {
            return MLand.GameData.MiniGameElementalCoursesAttendData.TryGet(id)?.attend ?? false;
        }

        public static double GetMaxSlimeLevelExp()
        {
            return MLand.GameData.SlimeFriendShipLevelUpData.Max(x => x.requireStackedExp);
        }

        public static double GetOfflineLevelUpPrice(int level)
        {
            var data = MLand.GameData.OfflineRewardLevelUpData.Where(x => x.level == level + 1).FirstOrDefault();

            return data?.price ?? 0;
        }

        public static int GetOfflineMaxTimeForMinute(int level)
        {
            int defaultTime = MLand.GameData.OfflineRewardCommonData.defaultMaxMinute;

            var data = MLand.GameData.OfflineRewardLevelUpData.Where(x => x.level == level).FirstOrDefault();
            if (data == null)
            {
                int maxLevel = MLand.GameData.OfflineRewardLevelUpData.Max(x => x.level);

                data = MLand.GameData.OfflineRewardLevelUpData.Where(x => x.level == maxLevel).FirstOrDefault();
            }

            return defaultTime + data?.addTimeForMinute ?? 0;
        }

        public static CharacterData GetCharacterData(string id)
        {
            CharacterData data = MLand.GameData.SlimeData.TryGet(id);
            if (data == null)
                data = MLand.GameData.CharacterData.TryGet(id);

            return data;
        }

        public static IEnumerable<CostumeData> GetCostumeDatasByType(CostumeType type)
        {
            return MLand.GameData.CostumeData.Values.Where(x => x.costumeType == type);
        }

        public static int GetCostumeMaxLevel()
        {
            return MLand.GameData.CostumeUpgradeData.Max(x => x.level);
        }

        public static int GetCostumeTotalRequirePieceToMaxLevel(int level)
        {
            int totalRequirePiece = 0;

            foreach(var data in MLand.GameData.CostumeUpgradeData)
            {
                if (data.level > level)
                {
                    totalRequirePiece += data.requirePiece;
                }
            }

            return totalRequirePiece;
        }

        public static int GetCostumeMaxLevelTotalRequirePiece()
        {
            return MLand.GameData.CostumeUpgradeData.Sum(x => x.requirePiece);
        }

        public static int GetCostumeUpgradeRequirePiece(int currentLevel)
        {
            int nextLevel = currentLevel + 1;

            var levelUpData = MLand.GameData.CostumeUpgradeData.Where(x => x.level == nextLevel).FirstOrDefault();

            return levelUpData?.requirePiece ?? 0;
        }

        public static CostumeStatData GetCostumeStatData(string id, int level)
        {
            return MLand.GameData.CostumeStatData.Where(x => x.id == id && x.level == level).FirstOrDefault();
        }

        public static CostumePosData GetCostumePosData(string id, string costumeId)
        {
            return MLand.GameData.CostumePosData.Where(x => x.id == id && x.costumeId == costumeId).FirstOrDefault();
        }

        public static CostumeData GetCostumeRandomData(CostumeType type)
        {
            var datas = MLand.GameData.CostumeData.Values.Where(x => x.costumeType == type);

            return GetShuffleDatas(datas).First();
        }
    }
}