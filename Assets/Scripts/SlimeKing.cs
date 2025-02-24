using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MLand
{
    class SlimeKing : MonoBehaviour, IPointerClickHandler
    {
        GameObject mMagicCircleParent;
        ParticleSystem[] mMagicCircles;
        ParticleSystem[] mMagicCircle2s;

        Animation mAnim;
        public void Init()
        {
            mMagicCircleParent = gameObject.FindGameObject("MagicCircle");

            mAnim = gameObject.GetComponent<Animation>();

            PlayAnim("SlimeKing_Idle");

            int count = (int)ElementalType.Count;

            mMagicCircles = new ParticleSystem[count];
            mMagicCircle2s = new ParticleSystem[count];

            for (int i = 0; i < count; ++i)
            {
                ElementalType type = (ElementalType)i;

                mMagicCircles[i] = mMagicCircleParent.FindComponent<ParticleSystem>($"MagicCircle_{type}");
                mMagicCircles[i].gameObject.SetActive(false);

                mMagicCircle2s[i] = mMagicCircleParent.FindComponent<ParticleSystem>($"MagicCircle_{type}_2");
                mMagicCircle2s[i].gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.DORewind();
            transform.DOPunchScale(Vector3.one * 0.25f, 0.35f);

            MLand.Lobby.ShowPopup(PopupStatus.SlimeKing);

            SoundPlayer.PlaySlimeTouchSound();
        }

        public IEnumerator PlaySpawnMotion(ElementalType type, bool expandField = false)
        {
            PlayAnim("SlimeKing_SpawnReady");

            SoundPlayer.PlayStartMagicCircle();
            
            // 초상화 연출 같이 보여줌
            MonsterLandUtil.ShowKingSlimePortraitMotion();

            yield return new WaitForSeconds(0.5f);

            // 마법진 실행
            PlayMagicCircle(type);

            SoundPlayer.PlayMagicCircle();

            if (expandField)
            {
                yield return new WaitForSeconds(1.5f);

                // 마법진 2 실행
                PlayMagicCircle2(type);

                SoundPlayer.PlayMagicCircle2();
            }

            // 마법진 실행하는 동안 잠깐 대기
            yield return new WaitForSeconds(1.5f);

            // 다시 애니메이션 시작
            PlayAnim("SlimeKing_Idle");
        }

        void PlayMagicCircle(ElementalType type)
        {
            int index = (int)type;

            mMagicCircleParent.transform.DORewind();
            mMagicCircleParent.transform.DOScale(1f, 0.5f).SetAutoKill(false);

            for (int i = 0; i < mMagicCircles.Length; ++i)
            {
                if (index == i)
                {
                    mMagicCircles[i].gameObject.SetActive(true);
                    mMagicCircles[i].Play();
                }
                else
                {
                    mMagicCircles[i].gameObject.SetActive(false);
                }
            }
        }

        void PlayMagicCircle2(ElementalType type)
        {
            int index = (int)type;

            for (int i = 0; i < mMagicCircle2s.Length; ++i)
            {
                if (index == i)
                {
                    mMagicCircle2s[i].gameObject.SetActive(true);
                    mMagicCircle2s[i].Play();
                }
                else
                {
                    mMagicCircle2s[i].gameObject.SetActive(false);
                }
            }
        }

        void PlayAnim(string name)
        {
            AnimationState state = mAnim[name];

            state.time = 0f;
            mAnim.clip = state.clip;
            mAnim.Play();
        }
    }
}