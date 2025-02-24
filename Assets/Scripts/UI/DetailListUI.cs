using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace MLand
{
    public enum DetailType
    {
        Slime,
        Building,
    }

    class DetailListUI
    {
        bool mIsShow;
        DetailType mCurDetail;

        Transform mParent;
        RectTransform mBackgroundTm;
        List<DetailUI> mDetailList;
        public void Init(LobbyUI lobby)
        {
            mIsShow = false;
            mParent = lobby.Find("DetailList").transform;
            mBackgroundTm = mParent.gameObject.FindComponent<RectTransform>("Background");
            mBackgroundTm.gameObject.SetActive(false);

            mDetailList = new List<DetailUI>();
            InitDetail<Detail_SlimeUI>();
            InitDetail<Detail_BuildingUI>();
        }

        void InitDetail<T>() where T : DetailUI
        {
            string name = typeof(T).Name;

            GameObject popupObj = mParent.gameObject.Find(name, true);

            Debug.Assert(popupObj != null, $"{name}의 DetailUI가 없다.");

            T statusPopup = popupObj.GetOrAddComponent<T>();

            statusPopup.Init(this);

            popupObj.SetActive(false);

            mDetailList.Add(statusPopup);
        }

        public DetailUI GetCurrentDetail()
        {
            return mDetailList[(int)mCurDetail];
        }

        Sequence mHideMotionSequnece;
        public void HideCurDetail()
        {
            HideDetail(mCurDetail);

            mIsShow = false;

            if (mHideMotionSequnece != null)
            {
                mShowMotionSequnece?.Rewind();
                mHideMotionSequnece.Rewind();
                mHideMotionSequnece.Play();
            }
            else
            {
                mHideMotionSequnece = DOTween.Sequence()
                    .Append(mBackgroundTm.DOAnchorPosY(435f, 0f))
                    .Append(mBackgroundTm.DOAnchorPosY(-100f, 0.5f).SetEase(Ease.InBack))
                    .SetAutoKill(false)
                    .OnComplete(() => mBackgroundTm.gameObject.SetActive(false));
            }
        }

        public void Refresh()
        {
            GetCurrentDetail()?.Refresh();
        }

        public void RefreshNewDot()
        {
            GetCurrentDetail()?.RefreshNewDot();
        }

        public void ChangeDetail(string showId, DetailType detail)
        {
            if (mCurDetail != detail)
            {
                HideDetail(mCurDetail);
            }

            ShowDetail(showId, detail);
        }

        Sequence mShowMotionSequnece;
        void ShowDetail(string id, DetailType detail)
        {
            mCurDetail = detail;

            DetailUI showDetail = mDetailList[(int)detail];

            showDetail.OnShow(id);

            if (mIsShow == false)
            {
                mIsShow = true;

                mBackgroundTm.gameObject.SetActive(true);

                if (mShowMotionSequnece != null)
                {
                    mHideMotionSequnece?.Rewind();
                    mShowMotionSequnece.Rewind();
                    mShowMotionSequnece.Play();
                }
                else
                {
                    mShowMotionSequnece = DOTween.Sequence()
                    .Append(mBackgroundTm.DOAnchorPosY(-100f, 0f))
                    .Append(mBackgroundTm.DOAnchorPosY(435f, 0.5f).SetEase(Ease.OutBack))
                    .SetAutoKill(false);
                }
            }
        }

        void HideDetail(DetailType detail)
        {
            DetailUI hideDetail = mDetailList[(int)detail];

            hideDetail.OnHide();
        }
    }
}