using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MLand
{
    class GoldSlime : Slime
    {
        float mLifeTime;
        Popup_GoldSlimeUI mPopup;
        public override void Init(string id)
        {
            base.Init(id);

            mLifeTime = MLand.GameData.GoldSlimeCommonData.durationMinute * TimeUtil.SecondsInMinute;

            mActionManager.PlayAction(ActionType.Spawn);
        }

        float mUpdateInterval;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (mPopup != null)
                return;

            mLifeTime -= dt;
            mUpdateInterval -= dt;
            if (mLifeTime <= 0f)
            {
                MLand.GameManager.RemoveGoldSlime();

                string message = StringTableUtil.GetSystemMessage("DisappearGoldSlime");

                MonsterLandUtil.ShowSystemMessage(message);
            }

            if (mUpdateInterval <= 0f)
            {
                mUpdateInterval = 1f;

                MLand.Lobby.RefreshGoldSlimeLifeTime(mLifeTime);
            }
        }

        public override void OnPointerClick()
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

            MLand.Lobby.HidePopupStatus();
            MLand.Lobby.HideDetail();

            int minute = MLand.GameData.GoldSlimeCommonData.rewardMinute;
            double amount = MLand.GameManager.CalcSlimeCoreDropAmountForMinute(minute);

            mPopup = MLand.PopupManager.CreatePopup<Popup_GoldSlimeUI>();
            mPopup.SetOnCloseAction(() => mPopup = null);
            mPopup.Init(amount, () =>
            {
                MLand.SavePoint.CheckQuests(QuestType.FindGoldSlime);
                MLand.GameManager.RemoveGoldSlime();
            });
        }

        public override void OnRelease()
        {
            base.OnRelease();

            gameObject.Destroy();
        }
    }
}