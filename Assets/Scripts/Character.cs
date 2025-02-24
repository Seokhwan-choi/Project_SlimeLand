using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

namespace MLand
{
    class Character : MonoBehaviour, IPointerClickHandler
    {
        protected float mSlimeCoreDropCoolTime;     // 슬라임 똥 생성 쿨 타임
        protected float mTouchCoolTime;             // 터치 쿨 타임
        protected CharacterAI mAI;
        protected CharacterData mData;
        protected CharacterAnim mAnim;
        protected CharacterPathFinder mPathFinder;
        protected ActionManager mActionManager;
        public string Id => mData.id;
        public bool CanSlimeCoreDrop => mSlimeCoreDropCoolTime <= 0f;
        public CharacterPathFinder PathFinder => mPathFinder;
        public CharacterData Data => mData;
        public CharacterAnim Anim => mAnim;
        public ActionManager Action => mActionManager;
        public virtual void Init(string id)
        {
            mData = MLand.GameData.SlimeData.TryGet(id);

            Debug.Assert(mData != null, $"{id}의 CharacterData가 없습니다.");

            mAnim = gameObject.GetOrAddComponent<CharacterAnim>();
            mAnim.Init(id);

            mActionManager = new ActionManager();
            mActionManager.Init(this);

            mAI = new CharacterAI();
            mAI.Init(mActionManager);

            mPathFinder = new CharacterPathFinder();
            mPathFinder.Init(this);

            GameObject goldSlimeAura = gameObject.FindGameObject("SparkleAreaYellow");
            goldSlimeAura.SetActive(this.IsGoldSlime());
        }

        public virtual void OnUpdate(float dt) 
        {
            mTouchCoolTime -= dt;

            mActionManager.OnUpdate(dt);
            mAI.OnUpdate(dt);
            mPathFinder.OnUpdate(dt);
        }
        public virtual void OnRelease() { }
        public virtual void OnPointerClick() { }
        public virtual (ElementalType, double) DropSlimeCore() { return (ElementalType.Water, 0); }
        public virtual void PlaySpawnMotion()
        {
            transform.localScale = Vector3.zero;

            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        public void ChangeSpriteFlipX(bool flipX)
        {
            mAnim.ChangeSpriteFlipX(flipX);
        }

        public void StartPathFinder(Vector3 startPos)
        {
            mPathFinder.StartActive(startPos);
        }

        public void RefreshCostumes()
        {
            mAnim.RefreshBody();

            mActionManager.PlayAction(ActionType.Idle);
        }
    }

    static class CharacterUtil
    {
        static public void MoveToTarget(this Character character, Character target, float arriveOffset = Const.ArriveOffset, Action onArrive = null)
        {
            character.PathFinder.SetPathFindTarget(target.transform, arriveOffset, onArrive);
        }

        static public void MoveToTarget(this Character character, Vector3 targetPos, float arriveOffset = Const.ArriveOffset, Action onArrive = null)
        {
            character.PathFinder.SetPathFindTarget(targetPos, arriveOffset, onArrive);
        }

        static public void MoveToRandomPos(this Character character, float arriveOffset = Const.ArriveOffset, Action onArrive = null)
        {
            character.MoveToTarget(MonsterLandUtil.GetCanMovePos(), arriveOffset, onArrive);
        }

        static public EmotionType GetRandEmotion()
        {
            return (EmotionType)UnityEngine.Random.Range(0, (int)EmotionType.Count);
        }

        static public bool IsGoldSlime(this Character character)
        {
            return character.Id == MLand.GameData.GoldSlimeCommonData.id;
        }
    }
}