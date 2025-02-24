using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLand
{
    class AdVideoTest_SlimeAnim : MonoBehaviour
    {
        string mId;
        int mIndex;
        string[] mCostumes;
        Animation mAnimation;
        SpriteRenderer[] mRenderers;
        Dictionary<CostumeType, AdVideoTest_CostumeObjManager> mCostumeManagers;
        public string[] Costumes => mCostumes; 
        public void Init()
        {
            mId = MLand.GameData.SlimeData.Keys.First();
            mIndex = 0;
            mCostumeManagers = new Dictionary<CostumeType, AdVideoTest_CostumeObjManager>();
            for (int i = 0; i < (int)CostumeType.Count; ++i)
            {
                CostumeType type = (CostumeType)i;

                GameObject costumeObjParent = gameObject.FindGameObject($"{type}");

                AdVideoTest_CostumeObjManager manager = costumeObjParent.GetOrAddComponent<AdVideoTest_CostumeObjManager>();

                manager.Init(this, type, mId);

                mCostumeManagers.Add(type, manager);
            }

            mAnimation = GetComponentInChildren<Animation>(true);

            mCostumes = new string[(int)CostumeType.Count];

            var move = gameObject.FindGameObject("Move");
            var Idle = gameObject.FindGameObject("Idle");

            mRenderers = (move.GetComponentsInChildren<SpriteRenderer>(true)).Concat(Idle.GetComponentsInChildren<SpriteRenderer>(true)).ToArray();

            Refresh();

            PlayIdle(EmotionType.Idle);
        }

        private void Update()
        {
            if ( Input.GetKeyDown(KeyCode.C) )
            {
                ChangeSlime();
            }

            if ( Input.GetKeyDown(KeyCode.V) )
            {
                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);
            }
            
            if ( Input.GetKeyDown(KeyCode.Q) )
            {
                mCostumes[(int)CostumeType.Face] = DataUtil.GetCostumeRandomData(CostumeType.Face).id;

                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);

                RefreshBody();
            }

            if ( Input.GetKeyDown(KeyCode.W) )
            {
                mCostumes[(int)CostumeType.Body] = DataUtil.GetCostumeRandomData(CostumeType.Body).id;

                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);

                RefreshBody();
            }

            if ( Input.GetKeyDown(KeyCode.E) )
            {
                mCostumes[(int)CostumeType.Acc] = DataUtil.GetCostumeRandomData(CostumeType.Acc).id;

                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);

                RefreshBody();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                mCostumes[(int)CostumeType.Face] = DataUtil.GetCostumeRandomData(CostumeType.Face).id;
                mCostumes[(int)CostumeType.Body] = DataUtil.GetCostumeRandomData(CostumeType.Body).id;
                mCostumes[(int)CostumeType.Acc] = DataUtil.GetCostumeRandomData(CostumeType.Acc).id;

                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);

                RefreshBody();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                mCostumes[(int)CostumeType.Face] = string.Empty;
                mCostumes[(int)CostumeType.Body] = string.Empty;
                mCostumes[(int)CostumeType.Acc] = string.Empty;

                EmotionType randEmotion = CharacterUtil.GetRandEmotion();

                PlayIdle(randEmotion);

                RefreshBody();
            }
        }

        public void ChangeSlime()
        {
            var slimeDatas = MLand.GameData.SlimeData.Values.Where(x => x.id != MLand.GameData.GoldSlimeCommonData.id);

            mIndex = (mIndex + 1) % slimeDatas.Count();
            mId = slimeDatas.ToList()[mIndex].id;

            foreach (var manager in mCostumeManagers.Values)
                manager.ChangeId(mId);

            Refresh();
        }

        public void Refresh()
        {
            RefreshBody();
            RefreshCostumes();
        }

        public void RefreshBody()
        {
            if (mId.IsValid())
            {
                string bodyCostume = mCostumes[(int)CostumeType.Body];

                foreach (SpriteRenderer renderer in mRenderers)
                {
                    string name = renderer.name;
                    Sprite sprite = null;

                    if (bodyCostume.IsValid() == false)
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

    class AdVideoTest_CostumeObjManager : MonoBehaviour
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

        AdVideoTest_SlimeAnim mParent;
        string mSlimeId;
        CostumeType mType;
        public void Init(AdVideoTest_SlimeAnim parent, CostumeType type, string slimeId)
        {
            mParent = parent;
            mSlimeId = slimeId;
            mType = type;
        }

        public void ChangeId(string slimeId)
        {
            mSlimeId = slimeId;
        }

        public void Refresh(AnimType animType)
        {
            string costume = mParent.Costumes[(int)mType];
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