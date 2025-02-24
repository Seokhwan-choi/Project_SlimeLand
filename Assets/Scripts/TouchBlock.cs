using UnityEngine;

namespace MLand
{
    class TouchBlock : MonoBehaviour
    {
        GameObject mLoadingObj;
        float mBlockTime;
        public bool IsActive => mBlockTime > 0f;

        public void Init()
        {
            mLoadingObj = gameObject.FindGameObject("Loading");
            mLoadingObj.SetActive(false);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            mBlockTime -= dt;
            if (mBlockTime <= 0)
            {
                EndBlock();
            }
        }

        public void StartBlock(float time = 3f, bool showLoadingImg = false)
        {
            mBlockTime = time;

            gameObject.SetActive(true);

            mLoadingObj.SetActive(showLoadingImg);
        }

        public void EndBlock()
        {
            mBlockTime = 0f;

            gameObject.SetActive(false);

            mLoadingObj.SetActive(false);
        }
    }
}