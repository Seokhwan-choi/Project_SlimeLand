using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    [Serializable]
    public class Inventory : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> mFriendShipItemKeys;
        [SerializeField]
        List<int> mFriendShipItemAmounts;

        public Dictionary<ObscuredString, ObscuredInt> FriendShipItems;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (FriendShipItems == null)
            {
                mFriendShipItemKeys = new List<string>();
                mFriendShipItemAmounts = new List<int>();
                FriendShipItems = new Dictionary<ObscuredString, ObscuredInt>();
            }
        }

        public void OnBeforeSerialize()
        {
            if (FriendShipItems != null)
            {
                mFriendShipItemKeys = FriendShipItems.Select(x => (string)x.Key).ToList();
                mFriendShipItemAmounts = FriendShipItems.Select(x => (int)x.Value).ToList();
            }
        }

        public void OnAfterDeserialize()
        {
            if (mFriendShipItemKeys.Count == mFriendShipItemAmounts.Count)
            {
                FriendShipItems = new Dictionary<ObscuredString, ObscuredInt>();

                for(int i = 0; i < mFriendShipItemKeys.Count; ++i)
                {
                    FriendShipItems.Add(mFriendShipItemKeys[i], mFriendShipItemAmounts[i]);
                }
            }
        }

        public void RandomizeKey()
        {
            if (FriendShipItems.Count > 0)
            {
                ObscuredString[] keys = FriendShipItems.Keys.ToArray();

                foreach (ObscuredString key in keys)
                {
                    key.RandomizeCryptoKey();

                    var value = FriendShipItems[key];

                    value.RandomizeCryptoKey();

                    FriendShipItems[key] = value;
                }
            }
        }
        
        public void AddFriendShipItem(string id, int amount)
        {
            if (FriendShipItems.ContainsKey(id))
            {
                FriendShipItems[id] += amount;
            }
            else
            {
                FriendShipItems.Add(id, amount);
            }
        }

        public void AddFriendShipItem(BoxOpenResult[] results)
        {
            if (results != null)
            {
                for (int i = 0; i < results.Length; ++i)
                {
                    var result = results[i];

                    if (FriendShipItems.ContainsKey(result.Id))
                    {
                        FriendShipItems[result.Id] += result.Amount;
                    }
                    else
                    {
                        FriendShipItems.Add(result.Id, result.Amount);
                    }
                }
            }
            
        }

        public void AddFriendShipItem(string[] ids)
        {
            for(int i = 0; i <ids.Length; ++i)
            {
                if (FriendShipItems.ContainsKey(ids[i]))
                {
                    FriendShipItems[ids[i]] += 1;
                }
                else
                {
                    FriendShipItems.Add(ids[i], 1);
                }
            }
        }

        public double UseFriendShipItem(string id, int amount)
        {
            // 호감도 아이템이 충분한지 확인
            if (IsEnoughFriendShipItem(id, amount) == false)
                return 0;

            FriendShipItemData data = MLand.GameData.FriendShipItemData.TryGet(id);
            if (data == null)
            {
                Debug.LogError($"{id}의 FriendShipItemData가 존재하지 않음");
                return 0;
            }

            // 호감도 아이템 사용
            FriendShipItems[id] -= amount;

            // 총 호감도 경험치
            return data.friendShipExp * amount;
        }

        public bool IsEnoughFriendShipItem(string id, int amount)
        {
            if (FriendShipItems.TryGetValue(id, out ObscuredInt itemAmount))
            {
                return itemAmount >= amount;
            }

            return false;
        }

        public int GetFriendShipItemAmount(string id)
        {
            if (FriendShipItems.TryGetValue(id, out ObscuredInt itemAmount))
            {
                return itemAmount;
            }

            return 0;
        }
    }
}


