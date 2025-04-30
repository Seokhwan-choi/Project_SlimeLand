using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class DetailUI : MonoBehaviour
    {
        protected string mCurId;
        protected DetailListUI mParent;

        Button mList_Button;                  // 좌측 목록 버튼
        Button mClose_Button;                 // 우측 종료 버튼
        TextMeshProUGUI mTitleName_Text;

        Image mImgSlimeCore1;
        TextMeshProUGUI mSlimeCoreAmountValue;
        TextMeshProUGUI mSlimeCoreAmountBonus;

        Image mImgSlimeCore2;
        TextMeshProUGUI mSlimeCoreCoolTimeValue;
        TextMeshProUGUI mSlimeCoreCoolTimeBonus;

        Image mImgEmblem;
        TextMeshProUGUI mSlimeCoreElementalType;

        RectTransform mDetailRectTm;

        Image mImgPotrait;
        public string CurId => mCurId;
        public virtual void Init(DetailListUI parent) 
        {
            mParent = parent;

            mDetailRectTm = gameObject.FindComponent<RectTransform>("Detail");
            mList_Button = gameObject.FindComponent<Button>("Btn_List");
            mClose_Button = gameObject.FindComponent<Button>("Btn_Close");

            mTitleName_Text = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            mImgSlimeCore1 = gameObject.FindComponent<Image>("Image_SlimeCore1");
            mImgSlimeCore2 = gameObject.FindComponent<Image>("Image_SlimeCore2");
            mImgEmblem = gameObject.FindComponent<Image>("Image_Emblem");
            mImgPotrait = gameObject.FindComponent<Image>("Image_Portrait");

            mSlimeCoreAmountValue = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCoreAmountValue");
            mSlimeCoreAmountBonus = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCoreAmountBonus");
            mSlimeCoreCoolTimeValue = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCoreCoolTimeValue");
            mSlimeCoreCoolTimeBonus = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeCoreCoolTimeBonus");
            mSlimeCoreElementalType = gameObject.FindComponent<TextMeshProUGUI>("Text_ElementalType");

            mList_Button.SetButtonAction(OnListButtonAction);
            mClose_Button.SetButtonAction(OnCloseButtonAction);
        }

        protected bool mIsShow;
        Sequence mFirstShowMotionSequence;
        Sequence mChangeShowMotionSequence;
        public virtual void OnShow(string showId) 
        {
            if (mIsShow == false)
            {
                SoundPlayer.PlayShowPopup();

                mCurId = showId;
                mIsShow = true;
                gameObject.SetActive(true);

                Refresh();

                if (mFirstShowMotionSequence != null)
                {
                    mChangeShowMotionSequence?.Rewind();
                    mHideMotionSequence?.Rewind();
                    mFirstShowMotionSequence.Rewind();
                    mFirstShowMotionSequence.Play();
                }
                else
                {
                    mFirstShowMotionSequence = DOTween.Sequence()
                    .Append(mDetailRectTm.DOAnchorPosY(-720f, 0f))
                    .Join(mDetailRectTm.DOAnchorPosY(-100f, 0.75f).SetEase(Ease.OutBack))
                    .SetAutoKill(false);
                }
            }
            else
            {
                if (mCurId != showId)
                {
                    mCurId = showId;

                    SoundPlayer.PlayHidePopup();

                    // 내려갔다가 다시 올라오자
                    if (mChangeShowMotionSequence != null)
                    {
                        mFirstShowMotionSequence?.Rewind();
                        mHideMotionSequence?.Rewind();
                        mChangeShowMotionSequence.Rewind();
                        mChangeShowMotionSequence.Play();
                    }
                    else
                    {
                        mChangeShowMotionSequence = DOTween.Sequence()
                        .Append(mDetailRectTm.DOAnchorPosY(-100f, 0f))
                        .Append(mDetailRectTm.DOAnchorPosY(-720f, 0.25f).OnComplete(HideDetailOnComplete).SetEase(Ease.InBack))
                        .Append(mDetailRectTm.DOAnchorPosY(-100f, 0.25f).SetEase(Ease.OutBack))
                        .SetAutoKill(false);
                    }

                    void HideDetailOnComplete()
                    {
                        Refresh();

                        SoundPlayer.PlayShowPopup();
                    }
                }
                else
                {
                    Refresh();
                }
            }
        }

        Sequence mHideMotionSequence;
        public virtual void OnHide() 
        {
            if (mCurId.IsValid())
            {
                SoundPlayer.PlayHidePopup();

                mCurId = string.Empty;

                mIsShow = false;

                if (mHideMotionSequence != null)
                {
                    mChangeShowMotionSequence?.Rewind();
                    mFirstShowMotionSequence?.Rewind();
                    mHideMotionSequence.Rewind();
                    mHideMotionSequence.Play();
                }
                else
                {
                    mHideMotionSequence = DOTween.Sequence()
                    .Append(mDetailRectTm.DOAnchorPosY(-100f, 0f))
                    .Append(mDetailRectTm.DOAnchorPosY(-720f, 0.5f).SetEase(Ease.InBack))
                    .SetAutoKill(false)
                    .OnComplete(() => gameObject.SetActive(false));
                }    
            }
        }

        public virtual void Refresh() { }
        public virtual void RefreshNewDot() { }
        public virtual void OnListButtonAction() 
        {
            mParent.HideCurDetail();

            MLand.CameraManager.ResetFollowInfo();
        }
        public virtual void OnCloseButtonAction() 
        { 
            mParent.HideCurDetail();

            MLand.CameraManager.ResetFollowInfo();
        }

        public void SetNameText(string nameText)
        {
            mTitleName_Text.text = nameText;
        }

        public void SetSlimeCoreAmountText(string amountText, string bonusText)
        {
            mSlimeCoreAmountValue.text = amountText;
            if (bonusText.IsValid())
            {
                mSlimeCoreAmountBonus.text = $"(+{bonusText})";
                mSlimeCoreAmountBonus.gameObject.SetActive(true);
            }
            else
            {
                mSlimeCoreAmountBonus.gameObject.SetActive(false);
            }
        }

        public void SetSlimeCoreCoolTimeText(string coolTimeText, string bonusText)
        {
            mSlimeCoreCoolTimeValue.text = coolTimeText;
            if (bonusText.IsValid())
            {
                mSlimeCoreCoolTimeBonus.text = $"(-{bonusText}s)";
                mSlimeCoreCoolTimeBonus.gameObject.SetActive(true);
            }
            else
            {
                mSlimeCoreCoolTimeBonus.gameObject.SetActive(false);
            }
        }

        public void SetSlimeCoreElementalTypeText(ElementalType type)
        {
            var name = StringTableUtil.GetName(type.ToString());

            mSlimeCoreElementalType.text = $"{name}";
        }

        public void SetSlimeCore1Img(ElementalType type)
        {
            var sprite = MLand.Atlas.GetCurrencySprite($"SlimeCore_{type}");

            mImgSlimeCore1.sprite = sprite;
        }

        public void SetSlimeCore2Img(ElementalType type)
        {
            var sprite = MonsterLandUtil.GetSlimeCoreImg(type);

            mImgSlimeCore2.sprite = sprite;
        }

        public void SetEmblemImg(ElementalType type)
        {
            var sprite = MonsterLandUtil.GetEmblemImg(type);

            mImgEmblem.sprite = sprite;
        }

        public void SetPotraitImg(Sprite sprite)
        {
            mImgPotrait.sprite = sprite;
        }

        public void SetActiveListButton(bool active)
        {
            mList_Button.gameObject.SetActive(active);
        }
    }
}