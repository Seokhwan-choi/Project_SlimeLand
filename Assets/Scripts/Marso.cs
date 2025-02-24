using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MLand
{
    class Marso : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        const string MarsoOrgName = "Marso";

        double mAmount;
        float mLifeTime;      // 시간이 다 되면 그냥 사라지자
        bool mIsDisappearing; // 사라지는 중
        MarsoManager mParent;
        public bool IsDisappearing => mIsDisappearing;
        public void Init(MarsoManager parent, double amount)
        {
            mParent = parent;
            mAmount = amount;
            mLifeTime = 10f;
            mIsDisappearing = false;

            InitSprite(amount);
        }

        public void OnUpdate(float dt)
        {
            if (mIsDisappearing)
                return;

            mLifeTime -= dt;
            if (mLifeTime <= 0f)
            {
                Release();
            }
        }

        void InitSprite(double amount)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            Debug.Assert(spriteRenderer != null, "마소에 SpriteRenderer가 없습니다.");

            spriteRenderer.sprite = MLand.Atlas.GetCurrencySprite(GetMarsoName(amount));
        }

        string GetMarsoName(double amount)
        {
            if (amount >= 1000)
            {
                return $"{MarsoOrgName}3";
            }
            else if (amount >= 100)
            {
                return $"{MarsoOrgName}2";
            }
            else
            {
                return $"{MarsoOrgName}1";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Acquire();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if ( Input.GetMouseButton(0) )
            {
                Acquire();
            }
        }

        public void Acquire()
        {
            if (mIsDisappearing)
                return;

            mIsDisappearing = true;

            Release();
        }

        void Release()
        {
            mIsDisappearing = true;

            transform.localScale = Vector3.one;

            MLand.ObjectPool.ReleaseObject(gameObject);

            mParent.RemoveMarso(this);
        }
    }
}