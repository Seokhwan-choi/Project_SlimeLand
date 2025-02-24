using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class MiniGameFaceChanger : MonoBehaviour
    {
        const float Interval = 10f;

        SpriteRenderer mNoise;

        int mCurIdx;
        SpriteRenderer mFace;
        

        public Sprite[] Faces;
        public Sprite[] Noises;

        void Start()
        {
            mInterval = Random.Range(0f, Interval);

            mNoise = gameObject.FindComponent<SpriteRenderer>("Image_Noise");
            mNoise.gameObject.SetActive(false);

            mFace = gameObject.FindComponent<SpriteRenderer>("Image_Face");

            SetRandFace();
        }

        float mInterval;
        void Update()
        {
            if (mChanging)
                return;

            float dt = Time.deltaTime;

            mInterval -= dt;
            if (mInterval <= 0f)
            {
                mInterval = Random.Range(0f, Interval);

                ChangeFace();
            }
        }

        bool mChanging;
        void ChangeFace()
        {
            mChanging = true;

            StartCoroutine(ChangeFaceRoutine());
        }

        IEnumerator ChangeFaceRoutine()
        {
            mNoise.gameObject.SetActive(true);

            int count = 3;
            while(count > 0)
            {
                mNoise.sprite = Noises[0];

                yield return new WaitForSeconds(0.1f);

                mNoise.sprite = Noises[1];

                yield return new WaitForSeconds(0.1f);

                mNoise.sprite = Noises[2];

                yield return new WaitForSeconds(0.1f);

                mNoise.sprite = Noises[3];

                yield return new WaitForSeconds(0.1f);

                count--;
            }

            mNoise.gameObject.SetActive(false);

            SetRandFace();

            mChanging = false;
        }

        void SetRandFace()
        {
            while(true)
            {
                int randIdx = Random.Range(0, Faces.Length);
                if (mCurIdx != randIdx)
                {
                    mCurIdx = randIdx;
                    break;
                }
            }

            mFace.sprite = Faces[mCurIdx];
        }
    }
}