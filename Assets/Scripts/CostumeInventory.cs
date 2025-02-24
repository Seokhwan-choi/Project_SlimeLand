using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Linq;
using System;

namespace MLand
{
    [Serializable]
    public class CostumeInventory : ISerializationCallbackReceiver
    {
        public List<CostumeInfo> CostumeInfoList;
        public void Init()
        {
            Normalize();
        }

        public void Normalize()
        {
            if (CostumeInfoList == null)
            {
                CostumeInfoList = new List<CostumeInfo>();
            }
        }

        public bool IsActiveCostume(string id)
        {
            var info = GetCostumeInfo(id);

            return (info?.Level ?? 0) > 0;
        }

        public void AddCostume(BoxOpenResult result)
        {
            string id = result.Id;

            CostumeInfo info = GetCostumeInfo(id);
            if (info != null)
            {
                int overPiece = info.AddPiece(result.Amount);

                result.ReturnGold = CalcOverPieceToGold(overPiece);
            }
            else
            {
                var newInfo = new CostumeInfo();

                newInfo.Init(id);

                int overPiece = newInfo.AddPiece(result.Amount);

                result.ReturnGold = CalcOverPieceToGold(overPiece);

                CostumeInfoList.Add(newInfo);
            }
        }

        public CostumeInfo FindEquippedCostume(string slimeId, CostumeType type)
        {
            return CostumeInfoList.Where(x => x.EquipedSlimeId == slimeId && x.Type == type).FirstOrDefault();
        }

        public void EquipCostume(string costumeId, string equipSlimeId)
        {
            GetCostumeInfo(costumeId)?.EquipCostume(equipSlimeId);
        }

        public void UnEquipCostume(string id)
        {
            GetCostumeInfo(id)?.UnEquipCostume();
        }

        public CostumeInfo GetCostumeInfo(string id)
        {
            return CostumeInfoList?.Where(x => x.Id == id).FirstOrDefault();
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }

        public void RandomizeKey()
        {
            foreach(var costumeInfo in CostumeInfoList)
            {
                costumeInfo.RandomizeKey();
            }
        }

        // 초과 획득한 코스튬을 골드로 환산해준다.
        public double CalcOverPieceToGold(int overPiece)
        {
            if (overPiece == 0)
                return 0;

            float overPieceTotalGem = overPiece * MLand.GameData.ShopCommonData.costumeReturnValuePerPiece;

            // 100 : 6 = overPieceTotalGem : x
            // x = 6 * overPieceTotalGem / 100
            int goldForMinute = Mathf.RoundToInt((6 * overPieceTotalGem) / 100);

            var slimeCoreDropAmountForMinute = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(goldForMinute);

            return slimeCoreDropAmountForMinute * MLand.GameData.ShopCommonData.slimeCoreDefaultPrice;
        }
    }

    [Serializable]
    public class CostumeInfo : ISerializationCallbackReceiver
    {
        [SerializeField]
        string mId;
        [SerializeField]
        string mEquipedSlimeId;
        [SerializeField]
        int mLevel;
        [SerializeField]
        int mPiece;

        public ObscuredString Id;
        public ObscuredString EquipedSlimeId;
        public ObscuredInt Level;
        public ObscuredInt Piece;
        public CostumeType Type => MLand.GameData.CostumeData.TryGet(Id)?.costumeType ?? CostumeType.Face;
        public void Init(string id)
        {
            Id = id;
        }

        public void EquipCostume(string equipSlimeId)
        {
            EquipedSlimeId = equipSlimeId;
        }

        public void UnEquipCostume()
        {
            EquipedSlimeId = string.Empty;
        }

        public int AddPiece(int piece)
        {
            if (IsMaxLevel())
                return piece;

            // 현재 레벨에서 최고 레벨까지 필요한 코스튬 조각 갯수
            int totalRequirePiece = DataUtil.GetCostumeTotalRequirePieceToMaxLevel(Level);

            // 최고 레벨 까지 필요한 코스튬 조각 갯수보다
            // 초과 된다면 초과된 부분 만큼 반환
            if ( totalRequirePiece < Piece + piece )
            {
                int overPiece = Piece + piece - totalRequirePiece;

                Piece += (piece - overPiece);

                return overPiece;
            }
            else
            {
                Piece += piece;

                return 0;
            }
        }

        public bool CanUpgrade()
        {
            // 최고 레벨인지 확인
            if (IsMaxLevel())
                return false;

            // 조각 충분한지 확인
            return IsEnoughPieceCurrentUpgrade();
        }

        public bool IsMaxLevel()
        {
            return DataUtil.GetCostumeMaxLevel() <= Level;
        }

        public bool Upgrade()
        {
            // 레벌업 가능한지 확인하기
            if (CanUpgrade() == false)
                return false;

            // 조각 소모
            int requirePiece = DataUtil.GetCostumeUpgradeRequirePiece(Level);
            if (UsePiece(requirePiece) == false)
                return false;

            // 레벨 업
            Level += 1;

            return true;
        }

        public bool UsePiece(int piece)
        {
            if (IsEnoughPiece(piece) == false)
                return false;

            Piece -= piece;

            return true;
        }

        public bool IsEnoughPieceCurrentUpgrade()
        {
            int requirePiece = DataUtil.GetCostumeUpgradeRequirePiece(Level);
            if (IsEnoughPiece(requirePiece) == false)
                return false;

            return true;
        }

        bool IsEnoughPiece(int piece)
        {
            return Piece >= piece;
        }

        public void RandomizeKey()
        {
            Id?.RandomizeCryptoKey();
            EquipedSlimeId?.RandomizeCryptoKey();
            Level.RandomizeCryptoKey();
            Piece.RandomizeCryptoKey();
        }

        public void OnBeforeSerialize()
        {
            if (Id != null)
            {
                mId = Id;
            }

            if (EquipedSlimeId != null)
            {
                mEquipedSlimeId = EquipedSlimeId;
            }

            mLevel = Level;
            mPiece = Piece;
        }

        public void OnAfterDeserialize()
        {
            if (mId.IsValid())
            {
                Id = mId;
            }

            if (mEquipedSlimeId.IsValid())
            {
                EquipedSlimeId = mEquipedSlimeId;
            }

            Level = mLevel;
            Piece = mPiece;
        }
    }
}