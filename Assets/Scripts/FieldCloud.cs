using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

namespace MLand
{
    class FieldCloud : MonoBehaviour, IPointerClickHandler
    {
        const float MaxPosX = 30f;
        const float MaxPosY = 15f;

        public bool IsAchievementsCloud;

        int mTouchCount;
        bool mIsMotion;
        bool mFadeIn;
        bool mIsLeft;
        float mMoveSpeed;
        SpriteRenderer mSpriteRenderer;
        private void Start()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();

            RefreshMoveForward();
        }

        void HideMotion(Action onComplete)
        {
            mIsMotion = true;

            mSpriteRenderer.DOFade(0f, 0.75f)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();

                    mIsMotion = false;
                });
        }

        void ShowMotion()
        {
            mSpriteRenderer.DOFade(1f, 0.75f);
        }

        void Refresh()
        {
            RefreshMoveForward();

            RefreshRandPos();

            ShowMotion();
        }

        void RefreshMoveForward()
        {
            mIsLeft = Util.Dice();

            mMoveSpeed = UnityEngine.Random.Range(0.25f, 0.75f);
        }

        void RefreshRandPos()
        {
            float randPosY = UnityEngine.Random.Range(-MaxPosY, MaxPosY);
            float posX = mIsLeft ? MaxPosX : -MaxPosX;

            transform.position = new Vector3(posX, randPosY);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (mFadeIn == false)
            {
                if (collision.isTrigger == false || collision.tag == this.tag)
                    return;

                mFadeIn = true;

                mSpriteRenderer.DOFade(0.5f, 0.75f);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (mFadeIn)
            {
                if (collision.isTrigger == false || collision.tag == this.tag)
                    return;

                mFadeIn = false;

                mSpriteRenderer.DOFade(1f, 0.75f);
            }
        }

        private void Update()
        {
            if (mIsMotion)
                return;

            Vector3 moveValue = (mIsLeft ? Vector3.left : Vector3.right) * Time.deltaTime * mMoveSpeed;

            transform.Translate(moveValue);

            if (mIsLeft)
            {
                if ( transform.position.x < -MaxPosX )
                {
                    HideMotion(Refresh);
                }
            }
            else
            {
                if ( transform.position.x > MaxPosX )
                {
                    HideMotion(Refresh);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (mIsMotion)
                return;

            if (IsAchievementsCloud)
            {
                SoundPlayer.PlayMiniGame_TicTacToe_ChangeTurn();

                transform.DORewind();
                transform.DOPunchScale(Vector3.one * 0.25f, 0.35f);

                mTouchCount++;
                if (mTouchCount >= 10)
                {
                    mTouchCount = 0;

                    // 방구 생성하고 사라지자
                    MLand.ParticleManager.Aquire("CloudBoom", pos: transform.position);

                    // 사운드 재생
                    SoundPlayer.PlaySpawnBoom();

                    // 하늘의 솜사탕 업적 확인
                    MLand.SavePoint.CheckAchievements(AchievementsType.CottonCandy);

                    // 위치 재정비
                    HideMotion(Refresh);
                }
            }
        }
    }
}


