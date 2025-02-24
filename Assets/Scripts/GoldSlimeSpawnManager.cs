using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class GoldSlimeSpawnManager
    {
        float mSpawnRemainTime;
        BuildingManager mBuildingManager;
        CharacterManager mCharacterManager;
        public void Init(BuildingManager buildingManager, CharacterManager characterManager)
        {
            mBuildingManager = buildingManager;
            mCharacterManager = characterManager;

            RefreshTime();
        }
        
        void RefreshTime()
        {
            mSpawnRemainTime = 
                TimeUtil.SecondsInMinute * 
                MLand.GameData.GoldSlimeCommonData.spawnIntervalMinute;
        }

        public void OnUpdate(float dt)
        {
            if (mCharacterManager.IsSpawnedGoldSlime)
                return;

            // 특정 건물이 지어져 있어야지만
            if (mBuildingManager.IsUnlockedBuilding(MLand.GameData.GoldSlimeCommonData.precendingBuilding) == false)
                return;

            mSpawnRemainTime -= dt;
            if (mSpawnRemainTime <=0)
            {
                RefreshTime();

                // 골드 슬라임 소환
                SpawnGoldSlime();
            }
        }

        void SpawnGoldSlime()
        {
            if (MLand.GameManager.SpawnGoldSlime())
            {
                MLand.Lobby.SetActiveGoldSlimeButton(true);

                string message = StringTableUtil.GetSystemMessage("AppearGoldSlime");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayAppearGoldSlime();

                if (SavePointBitFlags.Tutorial_6_GoldSlime.IsOff())
                    MLand.Lobby.StartTutorial($"{SavePointBitFlags.Tutorial_6_GoldSlime}");
            }
        }
    }
}