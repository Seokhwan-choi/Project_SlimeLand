using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class CheapShop_GoldItem : CheapShop_CurrencyItem
    {
        double mGoldAmountForMinute;
        GoldShopData mData;
        public override void Init(string id)
        {
            base.Init(id);

            mData = MLand.GameData.GoldShopData.TryGet(id);

            CalcGoldAmountForMinute();

            SetTextAmount(mGoldAmountForMinute, toAlphaStr:true);
            SetTextPrice($"{mData.gemPrice}");
            SetImgItem(MLand.Atlas.GetUISprite(mData.spriteImg));
        }

        public override void Localize()
        {
            CalcGoldAmountForMinute();

            SetTextAmount(mGoldAmountForMinute, toAlphaStr: true);
            SetTextPrice($"{mData.gemPrice}");
        }

        float mInterval;
        public override void OnUpdate(float dt)
        {
            mInterval -= dt;
            if (mInterval <= 0f)
            {
                mInterval = 1f;

                CalcGoldAmountForMinute();

                SetTextAmount(mGoldAmountForMinute, toAlphaStr: true);
            }
        }

        public override void OnBuyButton()
        {
            if (MLand.SavePoint.IsEnoughGem(mData.gemPrice)== false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGem");

                return;
            }

            var popup = MLand.PopupManager.CreatePopup<Popup_PurchaseConfirmUI>();

            popup.InitGold(mData.id, false, OnConfirm);

            void OnConfirm()
            {
                if (MLand.SavePoint.UseGem(mData.gemPrice))
                {
                    MLand.Lobby.RefreshGemText();

                    SoundPlayer.PlayGetGold();

                    MLand.GameManager.AddGold(mGoldAmountForMinute);
                }
            }
        }

        void CalcGoldAmountForMinute()
        {
            var slimeCoreDropAmountForMinute = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(mData.goldAmountForMinute);

            mGoldAmountForMinute = slimeCoreDropAmountForMinute * MLand.GameData.ShopCommonData.slimeCoreDefaultPrice;
        }
    }
}