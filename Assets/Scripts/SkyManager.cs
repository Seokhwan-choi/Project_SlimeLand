using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MLand
{
    class SkyManager : MonoBehaviour
    {
        SpriteRenderer mRenderer;

        // 6�� ~ 9��
        public Sprite Sky6to9;
        // 9�� ~ 12��
        public Sprite Sky9to12;
        // 12�� ~ 15��
        public Sprite Sky12to15;
        // 15�� ~ 18��
        public Sprite Sky15to18;
        // 18�� ~ 21��
        public Sprite Sky18to21;
        // 21�� ~ 24��
        public Sprite Sky21to24;
        // 24�� ~ 6��
        public Sprite Sky24to6;

        void Start()
        {
            mRenderer = GetComponent<SpriteRenderer>();
        }

        float mIntervalTime;
        void Update()
        {
            mIntervalTime -= Time.deltaTime;
            if (mIntervalTime <= 0f)
            {
                mIntervalTime = 1f;

                UpdateSky();
            }
        }

        void UpdateSky()
        {
            int nowHour = DateTime.Now.Hour;

            // �ð��뺰�� �̹����� ��ü�ȴ�.

            // 6 ~ 9
            if (nowHour >= 6 && nowHour < 9)
            {
                mRenderer.sprite = Sky6to9;
            }
            // 9 ~ 12
            else if  (nowHour >= 9 && nowHour < 12)
            {
                mRenderer.sprite = Sky9to12;
            }
            // 12 ~ 15
            else if (nowHour >= 12 && nowHour < 15)
            {
                mRenderer.sprite = Sky12to15;
            }
            // 15 ~ 18
            else if (nowHour >= 15 && nowHour < 18)
            {
                mRenderer.sprite = Sky15to18;
            }
            // 18 ~ 21
            else if (nowHour >= 18 && nowHour < 21)
            {
                mRenderer.sprite = Sky18to21;
            }
            // 21 ~ 24
            else if (nowHour >= 21 && nowHour < 24)
            {
                mRenderer.sprite = Sky21to24;
            }
            // 24 ~ 6
            else
            {
                mRenderer.sprite = Sky24to6;
            }
        }
    }
}