using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    // 실수로 저장이 안되고 있지만 그냥 냅두자
    // 인게임에서 나름 자연스럽다..
    public class SavedLuckySymbol : ISerializationCallbackReceiver
    {
        [SerializeField]
        double mStackedExp;

        public ObscuredDouble StackedExp;
        public int Level => GetLevel();
        public bool IsMaxLevel => Level == MLand.GameData.CharacterFriendShipLevelUpData.Max(x => x.level);
        public void StackExp(double exp)
        {
            StackedExp += exp;

            MLand.SavePoint.CheckAchievements(AchievementsType.LuckySymbol, exp);
        }

        int GetLevel()
        {
            int level = 1;

            foreach (SlimeFriendShipLevelUpData data in MLand.GameData.CharacterFriendShipLevelUpData)
            {
                if (data.requireStackedExp <= StackedExp)
                {
                    if (level <= data.level)
                    {
                        level = data.level;
                    }
                }
            }

            return level;
        }

        public void RandomizeCryptoKey()
        {
            StackedExp.RandomizeCryptoKey();
        }

        public void OnBeforeSerialize()
        {
            mStackedExp = StackedExp;
        }

        public void OnAfterDeserialize()
        {
            StackedExp = mStackedExp;
        }
    }
}