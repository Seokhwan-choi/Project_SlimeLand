using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MLand
{
    class Building : MonoBehaviour, IPointerClickHandler
    {
        protected int mLevel;
        protected float mSlimeCoreDropCoolTime;
        protected BuildingData mData;
        protected BuildingStatData mStatData;
        public int Level => mLevel;
        public string Id => mData.id;
        public bool CanSlimeCoreDrop => mSlimeCoreDropCoolTime <= 0f;
        public ElementalType Type => mData.elementalType;
        public BuildingStatData StatData => mStatData;
        public virtual void Init(BuildingData data) 
        {
            mData = data;

            Refresh();

            mSlimeCoreDropCoolTime = mStatData.slimeCoreDropCoolTime;
        }

        void Refresh()
        {
            RefreshLevel();
            RefreshStatData();
            RefreshObj();
        }

        void RefreshLevel()
        {
            int savedLevel = MLand.SavePoint.GetBuildingLevel(mData.id);

            mLevel = savedLevel == 0 ? 1 : savedLevel;
        }

        void RefreshStatData()
        {
            mStatData = DataUtil.GetBuildingStatData(mData.id, mLevel);

            Debug.Assert(mStatData != null, $"{mData.id}의 {mLevel}레벨 BuildingStatData가 없다.");
        }

        protected virtual void RefreshObj()
        {
            SetEnableObj(gameObject);
        }

        void SetEnableObj(GameObject go)
        {
            foreach (Collider2D collider in go.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = true;
            }

            go.SetActive(true);
        }

        public void OnLevelUp()
        {
            Refresh();

            PlayTweenScaleMotion();
        }

        public void OnUpdate(float dt)
        {
            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.DecreaseDropCoolTime);
            if (buffValue > 0)
                dt *= buffValue;

            mSlimeCoreDropCoolTime -= dt;
        }

        public (ElementalType, double) DropSlimeCore()
        {
            double amount = mStatData.slimeCoreDropAmount;
            if (amount <= 0)
                return (ElementalType.Water, 0);

            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.IncreaseDropAmount);
            if (buffValue > 0)
                amount *= buffValue;

            ItemInfo itemInfo = new ItemInfo(ItemType.SlimeCore, amount)
                .SetElementalType(mData.elementalType);

            MonsterLandUtil.ShowItemGetOrUseMotion(transform, 1.5f, itemInfo);

            mSlimeCoreDropCoolTime = mStatData.slimeCoreDropCoolTime;

            return (mData.elementalType, amount);
        }

        public void PlayTweenScaleMotion()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        public virtual void OnPointerClick()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = transform,
                FollowType = FollowType.Building,
            });

            MLand.Lobby.HidePopupStatus();

            MLand.Lobby.ShowDetail(Id, DetailType.Building);
        }
    }
}


