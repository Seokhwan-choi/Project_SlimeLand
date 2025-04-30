using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class Loading_Silhouette : MonoBehaviour
    {
        int mCurIdx;
        Image mImgSilhouette;
        public Sprite[] Silhouette;
        // Start is called before the first frame update
        void Start()
        {
            mCurIdx = 0;
            mImgSilhouette = GetComponent<Image>();
        }

        float mInterval;
        void Update()
        {
            float dt = Time.deltaTime;
            mInterval -= dt;
            if (mInterval <= 0f)
            {
                mInterval = 0.25f;

                mCurIdx = (mCurIdx + 1) % Silhouette.Length;

                mImgSilhouette.sprite = Silhouette[mCurIdx];
            }
        }
    }
}


