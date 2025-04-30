using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLand
{
    enum EmotionType
    {
        Idle,
        Excited,
        Happy,
        Shock,
        Sleepy,

        Count
    }

    enum AnimType
    {
        Idle,
        Excited,
        Happy,
        Shock,
        Sleepy,
        Move,

        Count
    }

    class CharacterAnim : MonoBehaviour
    {
        string mId;
        GameObject mCostumeObj;
        Animation mAnimation;
        SpriteRenderer[] mRenderers;
        Dictionary<CostumeType, CostumeObjManager> mCostumeManagers;
        public void Init(string id)
        {
            mId = id;
            mCostumeObj = gameObject.FindGameObject("Costume");
            mCostumeManagers = new Dictionary<CostumeType, CostumeObjManager>();
            for(int i = 0; i < (int)CostumeType.Count; ++i)
            {
                CostumeType type = (CostumeType)i;

                GameObject costumeObjParent = gameObject.FindGameObject($"{type}");

                CostumeObjManager manager = costumeObjParent.GetOrAddComponent<CostumeObjManager>();

                manager.Init(type, id);

                mCostumeManagers.Add(type, manager);
            }

            mAnimation = GetComponentInChildren<Animation>(true);

            var move = gameObject.FindGameObject("Move");
            var Idle = gameObject.FindGameObject("Idle");

            mRenderers = (move.GetComponentsInChildren<SpriteRenderer>(true)).Concat(Idle.GetComponentsInChildren<SpriteRenderer>(true)).ToArray();

            Refresh();
        }

        public void ChangeSlime(string id)
        {
            mId = id;

            foreach (var manager in mCostumeManagers.Values)
                manager.ChangeId(id);

            Refresh();
        }

        public void Refresh()
        {
            RefreshBody();
            RefreshCostumes();
        }

        public void RefreshBody()
        {
            if (mId.IsValid() && MLand.SavePoint.SlimeManager.SlimeInfoList.Count > 0)
            {
                var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mId);
                if (slimeInfo == null)
                    return;

                string bodyCostume = slimeInfo.Costumes[(int)CostumeType.Body];

                foreach (SpriteRenderer renderer in mRenderers)
                {
                    string name = renderer.name;
                    Sprite sprite = null;

                    if (mId == MLand.GameData.GoldSlimeCommonData.id || bodyCostume.IsValid() == false)
                    {
                        sprite = MLand.Atlas.GetCharacterSprite($"{mId}_{name}");

                        Debug.Assert(sprite != null, $"Id : {mId}, Name : {name}의 Sprite가 존재하지 않음");
                    }
                    else
                    {
                        sprite = MLand.Atlas.GetCostumeSprite($"{mId}_{name}_Costume");

                        Debug.Assert(sprite != null, $"Id : {mId}, Name : {name}_Costume 의 Sprite가 존재하지 않음");
                    }

                    renderer.sprite = sprite;
                }
            }
        }

        void RefreshCostumes(AnimType animType = AnimType.Idle)
        {
            foreach (var manager in mCostumeManagers.Values)
                manager.Refresh(animType);
        }

        public void ChangeSpriteFlipX(bool flipX)
        {
            foreach(var renderer in mRenderers)
            {
                if ( renderer.flipX != flipX )
                {
                    renderer.flipX = flipX;
                    mCostumeObj.transform.localScale = new Vector3(flipX ? -1 : 1, 1, 1);
                }
            }    
        }

        public void PlayMove()
        {
            PlayAnim("Slime_Move");

            RefreshCostumes(AnimType.Move);
        }

        public void PlayIdle(EmotionType emotionType)
        {
            PlayAnim($"Slime_{emotionType}");

            // 귀찮아서 순서를 맞춰놨음
            AnimType animType = (AnimType)emotionType;

            RefreshCostumes(animType);
        }

        void PlayAnim(string animName)
        {
            AnimationState anim = mAnimation[animName];

            anim.time = 0f;
            mAnimation.clip = anim.clip;
            mAnimation.Play();
        }
    }

    class CostumeObjManager : MonoBehaviour
    {
        enum CostumePos
        {
            Idle00,
            Idle01,
            Excited,
            Happy,
            Shock,
            Sleepy
        }

        string mSlimeId;
        CostumeType mType;
        public void Init(CostumeType type, string slimeId)
        {
            mSlimeId = slimeId;
            mType = type;
        }

        public void ChangeId(string slimeId)
        {
            mSlimeId = slimeId;
        }

        public void Refresh(AnimType animType)
        {
            SavedSlimeInfo slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mSlimeId);
            if (slimeInfo == null || mSlimeId == MLand.GameData.GoldSlimeCommonData.id)
            {
                gameObject.SetActive(false);

                return;
            }

            string costume = slimeInfo.Costumes[(int)mType];
            if (costume.IsValid())
            {
                gameObject.SetActive(true);

                CostumeData costumeData = MLand.GameData.CostumeData.TryGet(costume);
                if (costumeData != null)
                {
                    // 코스튬 장착
                    var renderer_idle_00 = gameObject.FindComponent<SpriteRenderer>($"{mType}_Idle_00");
                    var renderer_idle_01 = gameObject.FindComponent<SpriteRenderer>($"{mType}_Idle_01");

                    renderer_idle_00.sprite = MLand.Atlas.GetCostumeSprite(costumeData.spriteImg);
                    renderer_idle_01.sprite = MLand.Atlas.GetCostumeSprite(costumeData.spriteImg2);

                    var costumePosData = DataUtil.GetCostumePosData(mSlimeId, costume);
                    if (costumePosData != null)
                    {
                        string posStr00 = string.Empty;
                        string posStr01 = string.Empty;
                        // Move, Idle, Excited, Happy, Shock, Sleepy 중 하나 사용
                        if (animType == AnimType.Excited)
                        {
                            posStr00 = costumePosData.pos[(int)CostumePos.Excited];
                        }
                        else if (animType == AnimType.Happy)
                        {
                            posStr00 = costumePosData.pos[(int)CostumePos.Happy];
                        }
                        else if (animType == AnimType.Shock)
                        {
                            posStr00 = costumePosData.pos[(int)CostumePos.Shock];
                        }
                        else if (animType == AnimType.Sleepy)
                        {
                            posStr00 = costumePosData.pos[(int)CostumePos.Sleepy];
                        }
                        else // Move, Idle
                        {
                            // idle 00 과 idle 01 사용
                            posStr00 = costumePosData.pos[(int)CostumePos.Idle00];
                            posStr01 = costumePosData.pos[(int)CostumePos.Idle01];
                        }

                        Pos pos00 = Pos.Parse(posStr00);
                        Pos pos01 = Pos.Parse(posStr01);

                        // 코스튬 위치 조정
                        renderer_idle_00.transform.localPosition = new Vector3(pos00.X, pos00.Y);
                        renderer_idle_01.transform.localPosition = new Vector3(pos01.X, pos01.Y);

                        // 코스튬 댑스 조정
                        renderer_idle_00.sortingOrder = costumePosData.orderInLayer;
                        renderer_idle_01.sortingOrder = costumePosData.orderInLayer;
                    }
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}