using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    public class ItemInfo
    {
        string mId;
        double mAmount;
        ItemType mType;
        ElementalType mElementalType;
        ItemGrade mGrade;
        public string Id => mId;
        public ItemType Type => mType;
        public double Amount => mAmount;
        public bool IsSlimeCore => mType == ItemType.SlimeCore;
        public bool IsFriendShipItem => mType == ItemType.Toy || mType == ItemType.Food;
        public static ItemInfo CreateFriendShip(FriendShipItemData data, int count = 1)
        {
            ItemInfo itemInfo = new ItemInfo(data.id)
                .SetType(data.itemType)
                .SetGrade(data.grade)
                .SetAmount(count);

            return itemInfo;
        }

        public static ItemInfo CreateCostume(CostumeData data, int count)
        {
            ItemInfo itemInfo = new ItemInfo(data.id)
                .SetType(ItemType.Costume)
                .SetAmount(count);

            return itemInfo;
        }

        public static ItemInfo CreateFriendShip(string id)
        {
            FriendShipItemData data = MLand.GameData.FriendShipItemData.TryGet(id);
            if (data == null)
                return null;

            return CreateFriendShip(data);
        }

        public static ItemInfo CreateCostume(string id, int count)
        {
            CostumeData data = MLand.GameData.CostumeData.TryGet(id);
            if (data == null)
                return null;

            return CreateCostume(data, count);
        }

        public static ItemInfo CreateRandomBox(string id)
        {
            BoxData data = MLand.GameData.BoxData.TryGet(id);
            if (data == null)
                return null;

            ItemInfo itemInfo = new ItemInfo(id)
                .SetType(ItemType.RandomBox)
                .SetAmount(data.openCount);

            return itemInfo;
        }

        public static ItemInfo CreateRewardInfo(RewardData data)
        {
            if (data.gemReward > 0)
            {
                return new ItemInfo(ItemType.Gem, data.gemReward);
            }
            else if (data.friendShipReward.IsValid())
            {
                return CreateFriendShip(data.friendShipReward);
            }
            else if (data.boxReward.IsValid())
            {
                return CreateRandomBox(data.boxReward);
            }
            else
            {
                return new ItemInfo(ItemType.Gold, data.goldReward);
            }
        }

        public ItemInfo(ItemType type, double amount)
        {
            mType = type;
            mAmount = amount;
        }

        public ItemInfo(string id)
        {
            mId = id;
        }

        public ItemInfo SetElementalType(ElementalType elementalType)
        {
            mElementalType = elementalType;

            return this;
        }

        public ItemInfo SetType(ItemType type)
        {
            mType = type;

            return this;
        }

        public ItemInfo SetGrade(ItemGrade grade)
        {
            mGrade = grade;

            return this;
        }

        public string GetAmountString()
        {
            if (ItemType.Gold == mType || IsSlimeCore)
            {
                return mAmount.ToAlphaString();
            }
            else
            {
                return mAmount.ToString();
            }
        }

        public ItemInfo SetAmount(double amount)
        {
            mAmount = amount;

            return this;
        }

        public string GetNameStr()
        {
            if (IsFriendShipItem || Type == ItemType.Costume)
            {
                return StringTableUtil.GetName(mId);
            }
            else if (IsSlimeCore)
            {
                return $"{StringTableUtil.GetName(mElementalType.ToString())} {StringTableUtil.GetName("SlimeCore")}";
            }
            else if (Type == ItemType.RandomBox)
            {
                var boxData = MLand.GameData.BoxData.TryGet(mId);

                return StringTableUtil.GetName($"{boxData.boxType}Box");
            }
            else
            {
                return StringTableUtil.GetName($"{mType}");
            }
        }

        public string GetDesc()
        {
            if (IsFriendShipItem || Type == ItemType.Costume )
            {
                return StringTableUtil.GetDesc(mId);
            }
            else if (IsSlimeCore)
            {
                return StringTableUtil.GetDesc($"{mElementalType}SlimeCore");
            }
            else if (Type == ItemType.RandomBox)
            {
                var boxData = MLand.GameData.BoxData.TryGet(mId);

                return StringTableUtil.GetDesc($"{boxData.boxType}Box");
            }
            else
            {
                return StringTableUtil.GetDesc($"{mType}");
            }
        }

        public string GetGradeStr()
        {
            return $"{StringTableUtil.GetName("Grade")} : {StringTableUtil.GetGrade(mGrade)}"; 
        }

        public string GetFriendShipExpStr()
        {
            if (mId.IsValid() == false)
                return string.Empty;

            FriendShipItemData itemData = MLand.GameData.FriendShipItemData.TryGet(mId);
            if (itemData == null)
                return string.Empty;

            return $"{StringTableUtil.GetName("FriendShipExp")} : {itemData.friendShipExp}";
        }

        public Sprite GetGradeImg()
        {
            if (Type != ItemType.Costume)
                return MLand.Atlas.GetUISprite($"Grade_{mGrade}");
            else
                return MLand.Atlas.GetUISprite($"Grade_Costume_Normal");
        }

        public Sprite GetCircleImg()
        {
            return MLand.Atlas.GetUISprite($"Grade_{mGrade}_InCircle");
        }

        public Sprite GetIconImg()
        {
            switch (mType)
            {
                case ItemType.Food:
                case ItemType.Toy:
                    return GetFriendShipSpriteImg();
                case ItemType.Costume:
                    return GetCostumeSpriteImg();
                case ItemType.SlimeCore:
                    return GetSlimeCoreSpriteImg();
                case ItemType.Gold:
                    return MLand.Atlas.GetCurrencySprite("Currency_Gold");
                case ItemType.Gem:
                    return MLand.Atlas.GetCurrencySprite("Currency_Gem");
                case ItemType.FriendShipExp:
                    return MLand.Atlas.GetUISprite("FriendShipLevel");
                case ItemType.RandomBox:
                    return GetRandomBoxSpriteImg();
                default:
                    return null;
            }
        }

        Sprite GetFriendShipSpriteImg()
        {
            if (mId.IsValid() == false)
                return null;

            FriendShipItemData data = MLand.GameData.FriendShipItemData.TryGet(mId);
            if (data == null)
                return null;

            return MLand.Atlas.GetFriendShipSprite(data.spriteImg);
        }

        Sprite GetCostumeSpriteImg()
        {
            if (mId.IsValid() == false)
                return null;

            CostumeData data = MLand.GameData.CostumeData.TryGet(mId);
            if (data == null)
                return null;

            return MLand.Atlas.GetUISprite(data.thumbnail);
        }

        Sprite GetRandomBoxSpriteImg()
        {
            if (mId.IsValid() == false)
                return null;

            BoxData data = MLand.GameData.BoxData.TryGet(mId);
            if (data == null)
                return null;

            return MLand.Atlas.GetUISprite(data.spriteCloseImg);
        }

        Sprite GetSlimeCoreSpriteImg()
        {
            return MonsterLandUtil.GetSlimeCoreImg(mElementalType);
        }
    }
}