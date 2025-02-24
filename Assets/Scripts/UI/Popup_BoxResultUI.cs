using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Popup_BoxResultUI : PopupBase
    {
        bool mInMotion;
        BoxShopData mBoxShopData;
        BoxOpenType mOpenType;
        Coroutine mMotionRoutine;

        GameObject mInMotionObj;
        GameObject mButtonsObj;
        GameObject mFinishMotionObj;
        TextMeshProUGUI mTextBuyPrice;
        public void Init(string boxShopId, BoxOpenType openType, string boxId, BoxOpenResult[] results)
        {
            mBoxShopData = boxShopId.IsValid() ? MLand.GameData.BoxShopData.TryGet(boxShopId) : null;
            mOpenType = openType;
            mInMotionObj = gameObject.FindGameObject("InMotion");
            mFinishMotionObj = gameObject.FindGameObject("FinishMotion");
            mButtonsObj = mFinishMotionObj.FindGameObject("Buttons");
            mTextBuyPrice = mButtonsObj.FindComponent<TextMeshProUGUI>("Text_Price");

            BoxResultInfo[] infos = results.Select(x =>
            {
                if (x.Type == ItemType.Costume)
                {
                    return new BoxResultInfo()
                    {
                        ItemInfo = ItemInfo.CreateCostume(x.Id, x.Amount),
                        ReturnGold = x.ReturnGold,
                    };
                }
                else
                {
                    return new BoxResultInfo()
                    {
                        ItemInfo = ItemInfo.CreateFriendShip(x.Id),
                        ReturnGold = x.ReturnGold,
                    };
                }
            }).ToArray();

            SetButtonActions(infos);

            RefreshBuyPriceText();

            mMotionRoutine = StartCoroutine(PlayMotion(boxId, infos));
        }

        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            if (mInMotion)
                return;

            base.Close(immediate, hideMotion);
        }

        void SetButtonActions(BoxResultInfo[] results)
        {
            Button buttonSkip = gameObject.FindComponent<Button>("Button_Skip");
            buttonSkip.SetButtonAction(() => OnSkipButtonAction(results));

            Button buttonClose = mButtonsObj.FindComponent<Button>("Button_Close");
            buttonClose.SetButtonAction(() => Close());

            Button buttonBuy = mButtonsObj.FindComponent<Button>("Button_Buy");
            buttonBuy.SetButtonAction(OnBuyButtonAction);
            buttonBuy.gameObject.SetActive(mBoxShopData != null && mOpenType != BoxOpenType.Reward && mOpenType != BoxOpenType.Ad);
        }

        void OnSkipButtonAction(BoxResultInfo[] infos)
        {
            StopCoroutine(mMotionRoutine);

            StartCoroutine(FinishMotion(infos));
        }

        void OnBuyButtonAction()
        {
            // 보상으로 받은 상자 or 광고로 연 상자는
            // 바로 재구매 하지 못한다.
            if (mBoxShopData == null || 
                mOpenType == BoxOpenType.Reward ||
                mOpenType == BoxOpenType.Ad )
            {
                SoundPlayer.PlayErrorSound();

                return;
            }

            double gemPrice = mBoxShopData.gemPrice[(int)mOpenType];

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
                this.Close(true);

                // 박스 구매
                BuyBoxUtil.BuyBox(mBoxShopData, mOpenType);
            }
        }

        void RefreshBuyPriceText()
        {
            if (mBoxShopData != null)
            {
                double gemPrice = mBoxShopData.gemPrice[(int)mOpenType];

                bool isEnoughGem = MLand.SavePoint.IsEnoughGem(gemPrice);

                mTextBuyPrice.text = isEnoughGem ? $"{gemPrice}" : $"<color=red>{gemPrice}</color>";
            }
        }

        IEnumerator PlayMotion(string boxId, BoxResultInfo[] infos)
        {
            mInMotion = true;

            mInMotionObj.SetActive(true);
            mFinishMotionObj.SetActive(false);

            BoxData boxData = MLand.GameData.BoxData.TryGet(boxId);

            Image imageBox = gameObject.FindComponent<Image>("Image_Box");
            imageBox.sprite = MLand.Atlas.GetUISprite(boxData.spriteCloseImg);

            // ====== 박스가 등장하고 ======
            float punchDuration = 0.5f;

            imageBox.transform.DOPunchScale(Vector3.one, punchDuration);

            SoundPlayer.PlayBoxAppear();

            yield return new WaitForSeconds(punchDuration);

            // ====== 박스 좌우로 흔들 흔들 ======
            Vector3 endValue = new Vector3(0, 0, 10f);
            Vector3 endValue2 = new Vector3(0, 0, -10f);

            float shakeDuraotion = 0.1f;
            int shakeCount = 5;

            Sequence shakeSequence = DOTween.Sequence();
            shakeSequence
                .Append(imageBox.transform.DORotate(endValue, shakeDuraotion).OnComplete(SoundPlayer.PlayBoxShake))
                .Append(imageBox.transform.DORotate(endValue2, shakeDuraotion))
                .SetLoops(shakeCount);

            yield return shakeSequence.WaitForCompletion();

            // ====== 박스 오픈 ======
            float punchDuration2 = 0.5f;

            imageBox.transform.localRotation = Quaternion.Euler(Vector3.zero);
            imageBox.transform.DOPunchScale(Vector3.one * 1.5f, punchDuration2);

            SoundPlayer.PlayBeforeBoxOpen();

            yield return new WaitForSeconds(punchDuration2 * 1.5f);

            imageBox.sprite = MLand.Atlas.GetUISprite(boxData.spriteOpenImg);

            //SoundPlayer.PlayBoxOpening();
            SoundPlayer.PlayBoxOpen();

            // ====== 화면 화이트 아웃 ======
            Image whiteOut = gameObject.FindComponent<Image>("WhiteOut");

            Color colorEndValue = new Color(whiteOut.color.r, whiteOut.color.g, whiteOut.color.b, 1f);
            float whiteOurDuration = 0.25f;

            whiteOut.DORewind();
            whiteOut.DOColor(colorEndValue, whiteOurDuration)
                .SetAutoKill(false);

            yield return new WaitForSeconds(whiteOurDuration * 2f);

            // ====== 화면 화이트 아웃 원복 ======
            whiteOut.DOPlayBackwards();

            yield return new WaitForSeconds(whiteOurDuration * 0.5f);

            // 마무리 결과물 연출
            yield return FinishMotion(infos);
        }

        IEnumerator FinishMotion(BoxResultInfo[] infos)
        {
            mInMotionObj.SetActive(false);
            mFinishMotionObj.SetActive(true);
            mButtonsObj.SetActive(false);

            // 보상 몇개 보여줄지 결정
            GameObject rewardOne = gameObject.FindGameObject("Reward_One");
            GameObject rewardTen = gameObject.FindGameObject("Reward_Ten");

            bool isOneReward = infos.Length == 1;

            rewardOne.SetActive(isOneReward);
            rewardTen.SetActive(!isOneReward);

            ItemInfo[] itemInfos = infos.Select(x => x.ItemInfo).ToArray();

            List<ItemSlotUI> itemSlots = ItemSlotUtil.CreateItemSlotList(isOneReward ? rewardOne : rewardTen, itemInfos, showTextAmount:true);
            // 보상이 순서대로 위에서 떨어지도록
            foreach (ItemSlotUI itemSlot in itemSlots)
            {
                itemSlot.gameObject.SetActive(true);
                itemSlot.transform.localScale = Vector3.one * 3f;
                itemSlot.transform.DOScale(Vector3.one, 0.3f);

                bool isCostumeBox = false;

                if (mBoxShopData != null)
                {
                    isCostumeBox = mBoxShopData.boxType == BoxType.Costume;
                }

                var textAmount = itemSlot.gameObject.FindGameObject("Text_Amount");
                textAmount.SetActive(isCostumeBox);

                yield return new WaitForSeconds(0.125f);

                SoundPlayer.PlayItemDrop();
            }

            yield return new WaitForSeconds(0.25f);

            // 돌려줄 골드가 있다면 보여주자
            if (infos.Any(x => x.ReturnGold > 0))
            {
                SoundPlayer.PlayMiniGame_TicTacToe_LineDraw();

                int index = 0;
                foreach(var info in infos)
                {
                    if (info.ReturnGold > 0)
                    {
                        var itemSlot = itemSlots[index];

                        var returnGoldObj = itemSlot.gameObject.FindGameObject("ReturnGold");
                        returnGoldObj.SetActive(true);
                        returnGoldObj.transform.localScale = Vector3.zero;
                        returnGoldObj.transform.DOScale(Vector3.one, 0.3f)
                            .SetEase(Ease.OutBack);

                        var textReturnGoldAmount = returnGoldObj.FindComponent<TextMeshProUGUI>("Text_ReturnGoldAmount");
                        textReturnGoldAmount.text = info.ReturnGold.ToAlphaString();

                        var textOverPiece = returnGoldObj.FindComponent<TextMeshProUGUI>("Text_OverPiece");
                        textOverPiece.text = StringTableUtil.Get("UIString_OverPiece");

                        yield return new WaitForSeconds(0.125f);

                        SoundPlayer.PlayGetGold();
                    }
                    index++;
                }
            }

            // 보상이 모두 떨어지면 자연스럽게 위아래로 흔들리도록
            GameObject motionObj = isOneReward ? rewardOne : rewardTen;
            motionObj.transform.DOLocalMoveY(30f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo);

            mButtonsObj.SetActive(true);

            mInMotion = false;
        }
    }

    class BoxResultInfo
    {
        public ItemInfo ItemInfo;
        public double ReturnGold;
    }
}


