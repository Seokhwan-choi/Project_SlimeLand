using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

namespace MLand
{
    class Popup_ExpensiveShopUI : PopupBase
    {
        bool mAnyShopping;
        TextMeshProUGUI mTextTotalPrice;
        TextMeshProUGUI mTextUpdateTime;
        TextMeshProUGUI mTextUpdatePrice;
        TextMeshProUGUI mTextUpdateCount;
        Image mImgButtonUpdate;
        SpeechBalloonUI mSpeechBalloonUI;
        List<Popup_ExpensiveShop_ElementUI> mItemList;
        ExpensiveShop Shop => MLand.SavePoint.ExpensiveShop;
        int ItemCount => MLand.GameData.ShopCommonData.expensiveShopItemCount;
        public void Init()
        {
            mAnyShopping = false;

            CreateNewItems();

            var textUpdate = gameObject.FindComponent<TextMeshProUGUI>("Text_Update");
            textUpdate.text = StringTableUtil.Get("UIString_ExpensiveShopUpdate");

            mTextTotalPrice = gameObject.FindComponent<TextMeshProUGUI>("Text_TotalPrice");
            mTextUpdateTime = gameObject.FindComponent<TextMeshProUGUI>("Text_UpdateTime");
            mTextUpdatePrice = gameObject.FindComponent<TextMeshProUGUI>("Text_UpdatePrice");
            mTextUpdateCount = gameObject.FindComponent<TextMeshProUGUI>("Text_UpdateCount");
            mImgButtonUpdate = gameObject.FindComponent<Image>("Button_Update");

            var speechBalloonObj = gameObject.FindGameObject("SpeechBalloon");

            mSpeechBalloonUI = speechBalloonObj.GetOrAddComponent<SpeechBalloonUI>();
            mSpeechBalloonUI.Init(StringTableUtil.GetName("BiSsada"));

            var message = StringTableUtil.Get("BiSsadaMessage_FindMe");

            mSpeechBalloonUI.PlayTextMotion(message);

            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_ExpensiveShop"));

            SetButtonAction();
            SetTotalPriceText();
            SetUpdateTimeText();
            SetUpdatePriceText();
            SetUpdateCountText();
            SetUpdateButtonImg();

            MLand.SavePoint.CheckQuests(QuestType.VisitExpensiveShop);
        }

        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            base.Close(immediate, hideMotion);

            if (mAnyShopping == false)
                MLand.SavePoint.CheckAchievements(AchievementsType.EyeShopping);
        }
        
        void CreateNewItems()
        {
            mItemList = new List<Popup_ExpensiveShop_ElementUI>();

            for (int i = 0; i < ItemCount; ++i)
            {
                GameObject itemObj = gameObject.FindGameObject($"Item_{i}");

                Popup_ExpensiveShop_ElementUI item = itemObj.GetOrAddComponent<Popup_ExpensiveShop_ElementUI>();

                item.Init(Shop.Items[i], OnSelectItem);

                mItemList.Add(item);
            }
        }

        void SetButtonAction()
        {
            var buttonUpdate = gameObject.FindComponent<Button>("Button_Update");
            buttonUpdate.SetButtonAction(OnUpdateButton);  // 누르면 잼을 소모하고 아이템 목록 초기화

            var buttonPurchase = gameObject.FindComponent<Button>("Button_Purchase");
            buttonPurchase.SetButtonAction(OnPurchaseButton); // 현재 선택한 목록을 모두 구매
        }

        void SetTotalPriceText()
        {
            double totalPrice = GetTotalPrice();

            bool isEnoughGold = MLand.SavePoint.IsEnoughGold(totalPrice);

            mTextTotalPrice.text = isEnoughGold ? $"{totalPrice.ToAlphaString()}" : $"<color=red>{totalPrice.ToAlphaString()}</color>";
        }

        void SetUpdateTimeText()
        {
            var time = TimeUtil.GetTimeStr(Shop.GetUpdateRemainTime());
            var param = new StringParam("time", time);
            var message = StringTableUtil.Get("ExpensiveShop_UpdateRemainTime", param);

            mTextUpdateTime.text = message;
        }

        void SetUpdatePriceText()
        {
            double gemPrice = MLand.GameData.ShopCommonData.expensiveShopUpdateGemPrice;

            bool isEnoughGem = MLand.SavePoint.IsEnoughGem(gemPrice);

            mTextUpdatePrice.text = isEnoughGem ? $"{gemPrice}" : $"<color=red>{gemPrice}</color>";
        }

        void SetUpdateCountText()
        {
            int maxCount = MLand.GameData.ShopCommonData.expensiveShopUpdateDailyCount;
            int remainCount = Shop.DailyCounter.GetRemainCount(maxCount);
            int currentCount = Shop.DailyCounter.StackedCount;

            mTextUpdateCount.text = $"({remainCount}/{maxCount})";
        }

        void SetUpdateButtonImg()
        {
            int maxCount = MLand.GameData.ShopCommonData.expensiveShopUpdateDailyCount;

            bool isMaxCount = Shop.DailyCounter.IsMaxCount(maxCount);

            double gemPrice = MLand.GameData.ShopCommonData.expensiveShopUpdateGemPrice;

            bool isEnoughGem = MLand.SavePoint.IsEnoughGem(gemPrice);

            string btnName = isEnoughGem && isMaxCount == false ? "Btn_Square_Yellow" : "Btn_Square_LightGray";

            mImgButtonUpdate.sprite = MLand.Atlas.GetUISprite(btnName);
        }

        public bool CheckUpdateShop()
        {
            if ( Shop.CheckUpdateShop() )
            {
                MLand.GameManager.UpdateExpensiveShop();

                MLand.SavePoint.Save();

                CreateNewItems();

                return true;
            }

            SetUpdateTimeText();

            return false;
        }

        void OnUpdateButton()
        {
            if (Shop.IsEnoughImmediatelyUpdateCount() == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughExpensiveShopUpdateCount");

                return;
            }

            double gemPrice = MLand.GameData.ShopCommonData.expensiveShopUpdateGemPrice;

            if (MLand.SavePoint.IsEnoughGem(gemPrice) == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGem");

                return;
            }

            string title = StringTableUtil.Get("Title_Confirm");

            StringParam param = new StringParam("gem", gemPrice.ToString());
            string desc = StringTableUtil.Get("Confirm_UseGem", param);

            MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);

            void OnConfirm()
            {
                if (MLand.SavePoint.UseGem(gemPrice))
                {
                    Shop.UpdateShop(immediately:true);

                    MLand.GameManager.UpdateExpensiveShop(showMessage: false);

                    var message = StringTableUtil.GetSystemMessage("ExpensiveShopUpdate");

                    MonsterLandUtil.ShowSystemMessage(message);

                    SoundPlayer.PlayUpdateExpensiveShop();

                    CreateNewItems();

                    MLand.SavePoint.Save();

                    MLand.Lobby.RefreshGemText();

                    SetUpdateCountText();
                    SetUpdateButtonImg();
                }
            }
        }

        void OnPurchaseButton()
        {
            if (MLand.SavePoint.IsEnoughGold(GetTotalPrice()) == false)
            {
                var message = StringTableUtil.GetSystemMessage("NotEnoughGold");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            var items = GetSelectedItems();
            if (items.Count() <= 0)
            {
                var message = StringTableUtil.GetSystemMessage("NeedSelectedtItems");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            if (items.Any(x => x.IsAlreadyPurchased))
            {
                var message = StringTableUtil.GetSystemMessage("AlreadyPurchasedItemTryPurchase");

                MonsterLandUtil.ShowSystemMessage(message);

                SoundPlayer.PlayErrorSound();

                return;
            }

            double totalPrice = GetTotalPrice();
            if (MLand.GameManager.UseGold(totalPrice))
            {
                string[] resultIds = items.Select(x => x.Id).ToArray();
                if (resultIds != null)
                {
                    MLand.SavePoint.AddFriendShipItem(resultIds);

                    ItemInfo[] result = resultIds.Select(x => ItemInfo.CreateFriendShip(x)).ToArray();

                    MonsterLandUtil.ShowRewardPopup(result);

                    foreach (Popup_ExpensiveShop_ElementUI item in items)
                        item.OnPurchased();

                    MLand.SavePoint.Save();

                    RefreshTotalPriceText();

                    mAnyShopping = true;
                }
            }
        }

        void OnSelectItem()
        {
            RefreshTotalPriceText();
        }

        void RefreshTotalPriceText()
        {
            SetTotalPriceText();
        }

        double GetTotalPrice()
        {
            return GetSelectedItems().Sum(x => x.Price);
        }

        IEnumerable<Popup_ExpensiveShop_ElementUI> GetSelectedItems()
        {
            return mItemList.Where(x => x.IsSelected);
        }
    }
}


