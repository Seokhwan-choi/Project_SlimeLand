using UnityEngine.U2D;
using UnityEngine;
using System.Resources;
using System;
using TMPro;

namespace MLand
{
    class AtlasManager
    {
        SpriteAtlas mFieldAtlas;
        SpriteAtlas mUIAtlas;
        public void Init()
        {
            mFieldAtlas = Resources.Load<SpriteAtlas>("Atlas/Field");
            mUIAtlas = Resources.Load<SpriteAtlas>("Atlas/UI");
        }

        public Sprite GetCharacterSprite(string name)
        {
            return mFieldAtlas.GetSprite(name);
        }

        public Sprite GetBuildingSprite(string name)
        {
            return mFieldAtlas.GetSprite(name);
        }

        public Sprite GetCostumeSprite(string name)
        {
            return mFieldAtlas.GetSprite(name);
        }

        public Sprite GetCharacterUISprite(string name)
        {
            return mUIAtlas.GetSprite(name);
        }

        public Sprite GetBuildingUISprite(string name)
        {
            return mUIAtlas.GetSprite(name);
        }

        public Sprite GetCurrencySprite(string name)
        {
            return mUIAtlas.GetSprite(name);
        }

        public Sprite GetFriendShipSprite(string name)
        {
            return mUIAtlas.GetSprite(name);
        }

        public Sprite GetUISprite(string name)
        {
            return mUIAtlas.GetSprite(name);
        }
    }
}

