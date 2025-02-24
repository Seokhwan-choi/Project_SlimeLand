using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    enum FlowerAnimType
    {
        Up,
        UpIdle,
        Down,
        DownIdle,

    }
    class Grass_FlowerFieldAnimManager : MonoBehaviour
    {
        public AnimationClip Down;
        public AnimationClip DownIdle;
        public AnimationClip Up;
        public AnimationClip UpIdle;

        Animation mAnim;
        FlowerAnimType mCurAnim;

        private void Start()
        {
            mAnim = GetComponent<Animation>();

            mCurAnim = FlowerAnimType.UpIdle;

            mAnim.clip = UpIdle;
            mAnim.Play();

            mIntervalTime = Random.Range(2f, 5f);
        }

        float mIntervalTime;

        private void Update()
        {
            float dt = Time.deltaTime;

            mIntervalTime -= dt;
            if (mIntervalTime <= 0f)
            {
                if (mCurAnim == FlowerAnimType.Up)
                {
                    mIntervalTime = Random.Range(2f, 5f);

                    mAnim.clip = UpIdle;
                    mAnim.Play();

                    mCurAnim = FlowerAnimType.UpIdle;

                }
                else if (mCurAnim == FlowerAnimType.UpIdle)
                {
                    mIntervalTime = Down.length;

                    mAnim.clip = Down;
                    mAnim.Play();

                    mCurAnim = FlowerAnimType.Down;
                }
                else if (mCurAnim == FlowerAnimType.Down)
                {
                    mIntervalTime = Random.Range(2f, 5f);

                    mAnim.clip = DownIdle;
                    mAnim.Play();

                    mCurAnim = FlowerAnimType.DownIdle;
                }
                else if (mCurAnim == FlowerAnimType.DownIdle)
                {
                    mIntervalTime = Up.length;

                    mAnim.clip = Up;
                    mAnim.Play();

                    mCurAnim = FlowerAnimType.Up;
                }
            }
        }
    }
}


