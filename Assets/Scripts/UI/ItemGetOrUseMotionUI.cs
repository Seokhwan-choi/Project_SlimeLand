using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class ItemGetOrUseMotionUI : MonoBehaviour
    {
        const float MotionPower = 10f;

        float mPosOffsetY;
        Transform mTarget;
        Transform mMotionBase;
        Transform mFollowBase;

        Tweener mPunchTweener;
        Tweener mVisibleTweener;
        public void Init(Transform target, float offsetY, ItemInfo itemInfo)
        {
            mTarget = target;
            mPosOffsetY = offsetY;
            mFollowBase = gameObject.FindGameObject("Follower").transform;
            mMotionBase = gameObject.FindGameObject("Motion").transform;

            SetPos();

            InitItemInfo(itemInfo);

            StartMotion();
        }

        void InitItemInfo(ItemInfo itemInfo)
        {
            Image imgIcon = gameObject.FindComponent<Image>("Image_ItemIcon");
            imgIcon.color = Color.white;
            imgIcon.sprite = itemInfo.GetIconImg();

            TextMeshProUGUI textAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");
            if (SavePointBitFlags.ShowSlimeCoreGetAmount.IsOn())
            {
                textAmount.gameObject.SetActive(true);
                string prefix = itemInfo.Amount > 0 ? "+" : "-";
                textAmount.color = Color.white;
                textAmount.text = $"{prefix}{itemInfo.GetAmountString()}";
            }
            else
            {
                textAmount.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            SetPos();
        }

        void SetPos()
        {
            Vector3 pos = new Vector3(mTarget.position.x, mTarget.position.y + mPosOffsetY, mTarget.position.z);

            pos = Util.WorldToScreenPoint(pos);

            mFollowBase.position = pos;
        }
        
        void StartMotion()
        {
            if (mPunchTweener == null)
            {
                mPunchTweener = mMotionBase.DOPunchPosition(Vector3.one * MotionPower, 0.5f);
                mPunchTweener.OnComplete(CloseMotion)
                    .SetAutoKill(false);
            }
            else
            {
                mPunchTweener.Rewind();
                mPunchTweener.Play();
            }
        }

        void CloseMotion()
        {
            if (mVisibleTweener == null)
            {
                mVisibleTweener = DOTween.To((f) =>
                {
                    Color color = new Color(1, 1, 1, f);

                    Image imgIcon = gameObject.FindComponent<Image>("Image_ItemIcon");
                    imgIcon.color = color;

                    TextMeshProUGUI textAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_Amount");
                    textAmount.color = color;
                }, 1f, 0f, 0.5f);
                mVisibleTweener.OnComplete(
                    () =>
                    {
                        MLand.ObjectPool.ReleaseUI(gameObject);
                    })
                    .SetAutoKill(false);
            }
            else
            {
                mVisibleTweener.Rewind();
                mVisibleTweener.Play();
            }
        }
    }
}