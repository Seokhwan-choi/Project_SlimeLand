using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

namespace MLand
{
    class LuckySymbol : MonoBehaviour, IPointerClickHandler
    {
        bool mIsMaxLevel;

        public void Init()
        {
            mIsMaxLevel = MLand.SavePoint.LuckySymbol.IsMaxLevel;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundPlayer.PlayMiniGame_TicTacToe_ChangeTurn();

            transform.DORewind();
            transform.DOPunchScale(Vector3.one * 0.25f, 0.35f);

            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = transform,
                FollowType = FollowType.Slime,
            });

            // 호감도 증가, 터치하며 무조건 1 증가
            MLand.SavePoint.StackLuckySymbolFriendShipExp(1);

            MLand.Lobby.HidePopupStatus();

            MLand.Lobby.ShowDetail("LuckySymbol", DetailType.Slime);

            // 호감도 연출
            if (mIsMaxLevel == false)
            {
                if (MLand.SavePoint.LuckySymbol.IsMaxLevel)
                {
                    mIsMaxLevel = true;

                    StartMaxLevelMotion();
                }
            }
        }

        void StartMaxLevelMotion()
        {
            MLand.Lobby.EnqueueLobbyAction((lobbyAction) =>
            {
                StartCoroutine(MaxLevelMotion(lobbyAction.Done));
            });
        }

        IEnumerator MaxLevelMotion(Action onFinish)
        {
            MLand.GameManager.StartTouchBlock(4f);

            SoundPlayer.PlayLoadingComplete();

            bool isLeft = Util.Dice();

            float randX = UnityEngine.Random.Range(0.15f, 2f) * (isLeft ? -1 : 1);
            float randY = UnityEngine.Random.Range(-0.25f, 1f);

            GameObject particle = MLand.ParticleManager.Aquire("FriendShipUp", pos: transform.position);

            float orgPosX = particle.transform.position.x;
            float orgPosY = particle.transform.position.y;

            particle.transform.DORewind();
            particle.transform.DOLocalMove(new Vector3(orgPosX + randX, orgPosY + randY), 0.5f)
                .SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);

            MLand.GameManager.EndTouchBlock();

            onFinish?.Invoke();
        }
    }
}