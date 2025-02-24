using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MLand
{
    class Slime : Character
    {
        public override void Init(string id)
        {
            base.Init(id);

            mSlimeCoreDropCoolTime = mData.slimeCoreDropCoolTime;

            mTouchCoolTime = Const.TouchCoolTime;

            mActionManager.PlayAction(ActionType.Spawn);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            UpdateSlimeCoreDropCoolTime(dt);
        }

        public override void OnPointerClick()
        {
            OnTouchAction();
        }

        public void OnTouchAction()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = transform,
                FollowType = FollowType.Slime,
            });

            SoundPlayer.PlaySlimeTouchSound();

            float randPunchValue = Random.Range(0.1f, 0.35f);

            transform.DORewind();
            transform.DOPunchScale(new Vector3(randPunchValue, randPunchValue), 0.35f, elasticity: 0.2f)
                .SetAutoKill(false);

            if (mTouchCoolTime <= 0)
            {
                MLand.SavePoint.CheckQuests(QuestType.TouchSlime);

                mTouchCoolTime = Const.TouchCoolTime;
                mSlimeCoreDropCoolTime -= Const.DecreaseCoolTimeAmount;

                if (this.Action.IsCommunication)
                    this.Action.Communicate();
                else
                    this.Action.PlayAction(ActionType.Communication);

                // 호감도 증가, 터치하며 무조건 1 증가
                MLand.SavePoint.StackFriendShipExp(Id, 1);

                // 호감도 증가 연출
                bool isLeft = Util.Dice();

                float randX = Random.Range(0.15f, 2f) * (isLeft ? -1 : 1);
                float randY = Random.Range(-0.25f, 1f);

                GameObject particle = MLand.ParticleManager.Aquire("FriendShipUp", pos: transform.position);

                float orgPosX = particle.transform.position.x;
                float orgPosY = particle.transform.position.y;

                particle.transform.DORewind();
                particle.transform.DOLocalMove(new Vector3(orgPosX + randX, orgPosY + randY), 0.5f)
                    .SetEase(Ease.OutBack);
            }

            MLand.Lobby.HidePopupStatus();

            MLand.Lobby.ShowDetail(Id, DetailType.Slime);
        }

        void UpdateSlimeCoreDropCoolTime(float dt)
        {
            float buffValue = MLand.SavePoint.BuffManager.GetBuffValue(BuffType.DecreaseDropCoolTime);
            if (buffValue > 0)
                dt *= buffValue;

            mSlimeCoreDropCoolTime -= dt;
        }

        float GetSlimeCoreDropCoolTime()
        {
            return mData.slimeCoreDropCoolTime - MLand.SavePoint.SlimeManager.GetLevelUpRewardCoolTime(mData.id)
                - MLand.SavePoint.GetSlimeCoreDropCoolTimeByCostume(Id);
        }

        double GetSlimeCoreDropAmount()
        {
            return mData.slimeCoreDropAmount + MLand.SavePoint.SlimeManager.GetLevelUpRewardAmount(Id)
                + MLand.SavePoint.GetSlimeCoreDropAmountByCostume(Id);
        }

        public override (ElementalType, double) DropSlimeCore()
        {
            double amount = GetSlimeCoreDropAmount();
            if (amount <= 0)
                return (ElementalType.Water, 0);

            double buffValue = MLand.SavePoint.GetBuffValue(BuffType.IncreaseDropAmount);
            if (buffValue > 0)
                amount *= buffValue;

            ItemInfo itemInfo = new ItemInfo(ItemType.SlimeCore, amount)
                .SetElementalType(mData.elementalType);

            MonsterLandUtil.ShowItemGetOrUseMotion(transform, 1.5f, itemInfo);

            mSlimeCoreDropCoolTime = GetSlimeCoreDropCoolTime();

            return (mData.elementalType, amount);
        }
    }
}