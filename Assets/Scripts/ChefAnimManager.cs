using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class ChefAnimManager : MonoBehaviour
    {
        public Sprite[] ChefAnim;
        public float IntervalTime;

        int mIndex;
        float mIntervalTime;
        SpriteRenderer mRednerer;
        void Start()
        {
            mRednerer = GetComponent<SpriteRenderer>();
            mIntervalTime = IntervalTime;
            mIndex = 0;

            mRednerer.sprite = ChefAnim[mIndex];
        }

        void Update()
        {
            mIntervalTime -= Time.deltaTime;
            if (mIntervalTime <= 0f)
            {
                mIndex = (mIndex + 1) % ChefAnim.Length;

                mRednerer.sprite = ChefAnim[mIndex];

                mIntervalTime = IntervalTime;
            }
        }
    }
}


