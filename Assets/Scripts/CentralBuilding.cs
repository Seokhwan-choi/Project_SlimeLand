using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MLand
{
    class CentralBuilding : Building
    {
        CentralBuildingObj[] mCentralObjs;

        GameObject mStarVortex;
        GameObject mLevelUpNova;
        
        public override void Init(BuildingData data)
        {
            int maxLevel = DataUtil.GetBuildingMaxLevel(data.id);

            mStarVortex = gameObject.FindGameObject("StarVortex");
            mLevelUpNova = gameObject.FindGameObject("LevelupNova");
            

            mStarVortex.SetActive(false);
            mLevelUpNova.SetActive(false);

            mCentralObjs = new CentralBuildingObj[maxLevel];
            for (int i = 0; i < maxLevel; ++i)
            {
                GameObject go = transform.GetChild(i).gameObject;

                mCentralObjs[i] = new CentralBuildingObj();
                mCentralObjs[i].Init(go);
            }

            base.Init(data);
        }

        protected override void RefreshObj()
        {
            for (int i = 0; i < mCentralObjs.Length; ++i)
            {
                mCentralObjs[i].SetActive(i == mLevel - 1);
            }
        }

        public IEnumerator PlayUpgradeMotion()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = transform,
                FollowType = FollowType.Building
            });

            // 하얗게 변하면서
            mCentralObjs[mLevel - 1].DoFadeOut(2f);
            // DoTween 잠시 멈춤
            mCentralObjs[mLevel - 1].RewindDoTween();

            // 기모으는 연출
            yield return PlayStartVortex();
        }

        IEnumerator PlayStartVortex()
        {
            mStarVortex.SetActive(true);

            foreach (var ps in mStarVortex.GetComponentsInChildren<ParticleSystem>())
                ps.Play();

            SoundPlayer.PlayCentralBuildingChargeStart();

            yield return new WaitForSeconds(2f);

            mStarVortex.SetActive(false);
        }

        public IEnumerator PlayLevelupNova(Action onBuildFinish)
        {
            mLevelUpNova.SetActive(true);

            foreach (var ps in mLevelUpNova.GetComponentsInChildren<ParticleSystem>())
                ps.Play();

            // 사운드 재생
            SoundPlayer.PlayCentralBuildingChargeFinish();
            SoundPlayer.PlaySpawnSlime();

            // 터지면서 모습 보여주기
            mCentralObjs[mLevel - 1].DoFadeIn(2f);

            onBuildFinish?.Invoke();

            yield return new WaitForSeconds(2f);

            mLevelUpNova.SetActive(false);
        }
    }

    class CentralBuildingObj
    {
        GameObject mObj;
        SpriteRenderer mWhiteOut;
        public void Init(GameObject go)
        {
            mObj = go;
            mWhiteOut = go.FindComponent<SpriteRenderer>("WhiteOut");
            mWhiteOut.gameObject.SetActive(false);
        }

        public void DoFadeIn(float duration)
        {
            DoFade(1f, 0f, duration);
        }

        public void DoFadeOut(float duration)
        {
            DoFade(0f, 1f, duration);
        }

        void DoFade(float startValue, float endValue, float duration)
        {
            mWhiteOut.gameObject.SetActive(true);

            mWhiteOut.DORewind();

            Color startColor = new Color(mWhiteOut.color.a, mWhiteOut.color.g, mWhiteOut.color.b, startValue);

            mWhiteOut.color = startColor;
            mWhiteOut.DOFade(endValue, duration)
                .OnComplete(() => mWhiteOut.gameObject.SetActive(false))
                .SetAutoKill(false);
        }

        public void RewindDoTween()
        {
            foreach(var doTween in mObj.GetComponentsInChildren<DOTweenAnimation>())
            {
                doTween.DORewind();
            }
        }

        public void SetActive(bool active)
        {
            foreach (Collider2D collider in mObj.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = active;
            }

            mObj.SetActive(active);
        }
    }
}