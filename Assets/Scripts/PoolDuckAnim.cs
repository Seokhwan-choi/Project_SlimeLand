using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class PoolDuckAnim : MonoBehaviour
    {
        public float IntervalTime;
        public Sprite[] DuckImages;

        SpriteRenderer mRenderer;
        private void Start()
        {
            mRenderer = GetComponent<SpriteRenderer>();
            mIntervalTime = IntervalTime;
            mIndex = 0;
            mRenderer.sprite = DuckImages[mIndex];
        }

        float mIntervalTime;
        int mIndex;
        private void Update()
        {
            mIntervalTime -= Time.deltaTime;
            if (mIntervalTime <= 0f)
            {
                mIndex = (mIndex + 1) % DuckImages.Length;

                mRenderer.sprite = DuckImages[mIndex];

                mIntervalTime = IntervalTime;
            }
        }
    }
}


