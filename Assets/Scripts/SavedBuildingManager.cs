using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace MLand
{
	[Serializable]
	public class SavedBuildingManager : ISerializationCallbackReceiver
	{
		[SerializeField]
		List<string> mBuildingKeys;
		[SerializeField]
		List<int> mBuildingLevels;

        public Dictionary<ObscuredString, ObscuredInt> BuildingLevels;	// 건설된 건물들
        public SavedBuildingManager()
        {
			mBuildingKeys = new List<string>();
			mBuildingLevels = new List<int>();

			BuildingLevels = new Dictionary<ObscuredString, ObscuredInt>();

            foreach (string id in DataUtil.GetAllCentralBuildingDatas()?.Select(x => x.id))
            {
                AddBuilding(id);
            }
        }

		public void OnBeforeSerialize()
		{
			if (BuildingLevels != null)
			{
				mBuildingKeys = BuildingLevels.Select(x => (string)x.Key).ToList();
				mBuildingLevels = BuildingLevels.Select(x => (int)x.Value).ToList();
			}
		}

		public void OnAfterDeserialize()
		{
            if (mBuildingLevels != null)
            {
				if (mBuildingKeys.Count != mBuildingLevels.Count)
					return;

				BuildingLevels = new Dictionary<ObscuredString, ObscuredInt>();
				for (int i = 0; i < mBuildingKeys.Count; ++i)
                {
					BuildingLevels.Add(mBuildingKeys[i], mBuildingLevels[i]);
				}
            }
        }

		public void RandomizeKey()
        {
			if (BuildingLevels != null)
            {
				var keys = BuildingLevels.Select(x => x.Key).ToArray();

				foreach (var key in keys)
                {
					var value = BuildingLevels[key];

					value.RandomizeCryptoKey();

					BuildingLevels[key] = value;
				}
            }
        }

		public int GetBuildingLevel(string buildingId)
		{
			return BuildingLevels.TryGet(buildingId);
		}

		public bool IsUnlockedBuilding(string buildingId)
		{
			return BuildingLevels.ContainsKey(buildingId);
		}

		public void AddBuilding(string buildingId)
		{
			if (IsUnlockedBuilding(buildingId))
			{
				Debug.LogError("이미 추가된 건물을 또 추가하려고 했다.");
				return;
			}

			BuildingLevels.Add(buildingId, 1);
		}

		public bool LevelUpBuilding(string buildingId)
		{
			if (IsUnlockedBuilding(buildingId) == false)
			{
				Debug.LogError("추가된적 없는 건물을 레벨업을 했다.");
				return false;
			}

			BuildingLevels[buildingId] += 1;

			return true;
		}

        public bool IsExpandedField(ElementalType type)
        {
			var centralBuildingData = MLand.GameData.BuildingData.Values
				.Where(x => x.isCentralBuilding && x.elementalType == type)
				.FirstOrDefault();

			string id = centralBuildingData?.id ?? string.Empty;
			int level = GetBuildingLevel(id);

            BuildingStatData buildingStatData = DataUtil.GetBuildingStatData(id, level);

			return buildingStatData?.expandField ?? false;
		}
    }
}