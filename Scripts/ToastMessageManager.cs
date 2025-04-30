using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

namespace MLand
{
    class ToastMessageManager
    {
        const float DefaultMovePosY = 200f;

        TextMeshProUGUI mTextDesc;
        RectTransform mMoveTm;
        CanvasGroup mCanvasGroup;
        ToastMessageItem mCurrItem;
        List<ToastMessageItem> mItemList;
        public void Init(LobbyUI lobby)
        {
            var toastMessageObj = lobby.Find("ToastMessage");
            mMoveTm = toastMessageObj.FindComponent<RectTransform>("Move");
            mCanvasGroup = toastMessageObj.GetComponent<CanvasGroup>();
            mTextDesc = toastMessageObj.FindComponent<TextMeshProUGUI>("Text_Desc");

            mItemList = new List<ToastMessageItem>();
        }

        public void OnUpdate(float dt)
        {
            if (mCurrItem != null)
            {
                mCurrItem.OnUpdate(dt);

                if (mCurrItem.IsDone)
                {
                    mCurrItem = null;
                }
            }
            else
            {
                mCurrItem = PopItem();
                if (mCurrItem != null)
                {
                    RefreshInfo();

                    PlayAppearMotion();
                }
            }
        }

        public void AddToastMessage(string desc)
        {
            ToastMessageItem item = new ToastMessageItem();

            item.Init(this, desc);

            mItemList.Add(item);
        }

        Sequence mAppearSequence;
        public void PlayAppearMotion()
        {
            float duration = MLand.GameData.CommonData.toastMessageLiftTime * 0.5f;

            mAppearSequence?.Rewind();
            mAppearSequence = DOTween.Sequence()
                .Join(mMoveTm.DOAnchorPosY(-25f, duration).SetEase(Ease.OutExpo))
                .Join(DOTween.To((f) => mCanvasGroup.alpha = f, 0f, 1f, duration))
                .SetAutoKill(false);
        }

        Sequence mHideSequence;
        public void PlayHideMotion()
        {
            float duration = MLand.GameData.CommonData.toastMessageLiftTime * 0.5f;

            mHideSequence?.Rewind();
            mHideSequence = DOTween.Sequence()
                .Join(mMoveTm.DOAnchorPosY(DefaultMovePosY, duration).SetEase(Ease.InExpo))
                .Join(DOTween.To((f) => mCanvasGroup.alpha = f, 1f, 0f, duration))
                .SetAutoKill(false);
        }

        void RefreshInfo()
        {
            if (mCurrItem != null)
            {
                mTextDesc.text = mCurrItem.Desc;
            }
        }

        ToastMessageItem PopItem()
        {
            ToastMessageItem item = null;

            if (mItemList.Count > 0)
            {
                item = mItemList.FirstOrDefault();

                mItemList.RemoveAt(0);
            }

            return item;
        }
    }

    class ToastMessageItem
    {
        float mLifeTime;
        string mDesc;
        ToastMessageManager mParent;
        public string Desc => mDesc;
        public bool IsDone => mLifeTime <= 0;
        public void Init(ToastMessageManager parent, string desc)
        {
            mParent = parent;
            mDesc = desc;
            mLifeTime = MLand.GameData.CommonData.toastMessageLiftTime;
        }

        public void OnUpdate(float dt)
        {
            float prevLifeTime = mLifeTime;

            mLifeTime -= dt;

            if (mLifeTime <= 0)
            {
                mLifeTime = 0f;
            }
            else if (mLifeTime < MLand.GameData.CommonData.toastMessageLiftTime * 0.5f &&
                     prevLifeTime >= MLand.GameData.CommonData.toastMessageLiftTime * 0.5f)
            {
                mParent.PlayHideMotion();
            }
        }
    }
}