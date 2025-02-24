using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
	[Serializable]
	public class Currency : ISerializationCallbackReceiver
	{
		[SerializeField]
		double mTotalGold;
		[SerializeField]
		double mTotalGem;
		[SerializeField]
		double[] mTotalSlimeCores;

		public ObscuredDouble TotalGold;
        public ObscuredDouble TotalGem;
        public ObscuredDouble[] TotalSlimeCores;
		public void Init()
        {
			Normalize();
		}

		public void Normalize()
        {
			if (TotalSlimeCores == null)
            {
				int count = (int)ElementalType.Count;

				mTotalSlimeCores = new double[count];
				TotalSlimeCores = new ObscuredDouble[count];
			}
		}

		public void OnBeforeSerialize()
		{
            mTotalGold = TotalGold;
            mTotalGem = TotalGem;

            if (TotalSlimeCores != null)
            {
                mTotalSlimeCores = TotalSlimeCores.Select(x => (double)x).ToArray();
            }
        }

		public void OnAfterDeserialize()
		{
            TotalGold = mTotalGold;
            TotalGem = mTotalGem;

            if (mTotalSlimeCores != null)
            {
                TotalSlimeCores = mTotalSlimeCores.Select(x => (ObscuredDouble)x).ToArray();
            }
        }

		public void RandomizeKey()
        {
			TotalGold.RandomizeCryptoKey();
			TotalGem.RandomizeCryptoKey();

			if (TotalSlimeCores != null)
            {
				for(int i = 0; i < TotalSlimeCores.Length; ++i)
                {
					TotalSlimeCores[i].RandomizeCryptoKey();
				}
			}
		}

		public double GetTotalGold()
        {
			return TotalGold;
        }

		public void AddGold(double gold)
		{
			TotalGold += gold;
		}

		public bool IsEnoughGold(double gold)
		{
			return TotalGold >= gold;
		}

		public bool UseGold(double gold)
		{
			if (IsEnoughGold(gold))
			{
				TotalGold -= gold;

				return true;
			}

			return false;
		}

		public double GetTotalGem()
        {
			return TotalGem;
        }

		public void AddGem(double gem)
		{
			TotalGem += gem;
		}

		public bool IsEnoughGem(double gem)
		{
			return TotalGem >= gem;
		}

		public bool UseGem(double gem)
		{
			if (IsEnoughGem(gem))
			{
				TotalGem -= gem;

				return true;
			}

			return false;
		}

		public double GetSlimeCoreAmount(ElementalType type)
		{
			return TotalSlimeCores[(int)type];
		}

		public void AddSlimeCores(double[] addSlimeCores)
		{
			if (TotalSlimeCores.Length != addSlimeCores.Length)
				return;

			for (int i = 0; i < TotalSlimeCores.Length; ++i)
			{
				TotalSlimeCores[i] += addSlimeCores[i];
			}
		}

		public void AddSlimeCore(ElementalType addType, double amount)
		{
			if (TotalSlimeCores.Length <= (int)addType ||
				TotalSlimeCores.Length < 0)
				return;

			TotalSlimeCores[(int)addType] += amount;
		}

		public bool IsEnoughSlimeCore(ElementalType checkType, double amount)
		{
			if (TotalSlimeCores.Length <= (int)checkType ||
				TotalSlimeCores.Length < 0)
				return false;

			return TotalSlimeCores[(int)checkType] >= amount;
		}

		public bool UseSlimeCore(ElementalType useType, double amount)
		{
			if (TotalSlimeCores.Length <= (int)useType ||
				TotalSlimeCores.Length < 0)
				return false;

			if (IsEnoughSlimeCore(useType, amount) == false)
				return false;

			TotalSlimeCores[(int)useType] -= amount;

			return true;
		}
    }
}