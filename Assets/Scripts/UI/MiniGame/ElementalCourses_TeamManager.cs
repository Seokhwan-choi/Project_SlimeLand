using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;

namespace MLand
{
    class ElementalCourses_TeamManager
    {
        int mLevel;
        bool mInFever;
        Image mImage_SlimeTeacher;
        Image mImgLeftTeamButton;
        Image mImgRightTeamButton;
        Image mImgLeftTeamButtonEmblem;
        Image mImgRightTeamButtonEmblem;

        Image[] mImgLeftTeams;
        Image[] mImgRightTeams;
        List<string> mLeftTeamIdList;
        List<string> mRightTeamIdList;

        GameObject mNoneTeamParent;
        List<NoneTeam> mNoneTeamList;
        List<NoneTeamMoveInfo> mNoneTeamMoveInfoList;
        Popup_MiniGame_ElementalCoursesUI mParent;
        string CurrentSlimeId => mNoneTeamList.First()?.Id;
        int MaxLevel => MLand.GameData.MiniGameElementalCoursesData.maxLevel;
        int MaxNoneTeamCount => MaxTeamCount + MaxTeamCount;
        int MaxTeamCount => MLand.GameData.MiniGameElementalCoursesData.maxTeamCount;
        public void Init(Popup_MiniGame_ElementalCoursesUI parent)
        {
            mParent = parent;

            var gamePlayObj = parent.gameObject.FindGameObject("GamePlay");

            mImgLeftTeams = new Image[MaxTeamCount];
            mImgRightTeams = new Image[MaxTeamCount];
            for (int i = 0; i < MaxTeamCount; ++i)
            {
                mImgLeftTeams[i] = gamePlayObj.FindComponent<Image>($"Image_LeftTeamSlime_{i + 1}");
                mImgRightTeams[i] = gamePlayObj.FindComponent<Image>($"Image_RightTeamSlime_{i + 1}");
            }

            mNoneTeamParent = gamePlayObj.FindGameObject("NoneTeams");
            mNoneTeamMoveInfoList = new List<NoneTeamMoveInfo>();
            for (int i = 0; i < MaxNoneTeamCount; ++i)
            {
                RectTransform rectTm = mNoneTeamParent.FindComponent<RectTransform>($"NoneTeam_{i}");
                NoneTeamMoveInfo moveInfo = new NoneTeamMoveInfo()
                {
                    LocalPos = rectTm.localPosition,
                    SizeDelta = rectTm.sizeDelta,
                };

                mNoneTeamMoveInfoList.Add(moveInfo);

                rectTm.gameObject.SetActive(false);
            }

            Button leftButton = gamePlayObj.FindComponent<Button>("Button_Left");
            leftButton.SetButtonAction(() => mParent.OnElementalButtonAction(isLeft: true));
            mImgLeftTeamButton = gamePlayObj.FindComponent<Image>("ButtonFrame_Left");
            mImgLeftTeamButtonEmblem = gamePlayObj.FindComponent<Image>("Emblem_Left");

            Button rightButton = gamePlayObj.FindComponent<Button>("Button_Right");
            rightButton.SetButtonAction(() => mParent.OnElementalButtonAction(isLeft: false));
            mImgRightTeamButton = gamePlayObj.FindComponent<Image>("ButtonFrame_Right");
            mImgRightTeamButtonEmblem = gamePlayObj.FindComponent<Image>("Emblem_Right");

            mImage_SlimeTeacher = gamePlayObj.FindComponent<Image>("Image_SlimeTeacher");

            Reset();
        }

        public void Reset()
        {
            mLevel = 1;
            mInFever = false;

            RefreshTeamList();
            RefreshNoneTeamList();
            RefreshTeacherForward(playMotion:false);
        }

        public void OnLevelUp()
        {
            mLevel += 1;
            mLevel = Math.Min(mLevel, MaxLevel);

            SoundPlayer.PlayMiniGameLevelUp();

            RefreshTeamActiveList();
        }

        // 현재 슬라임 좌 or 우 팀 결정
        public bool ClassifyTeam(bool isLeft)
        {
            bool isSuccessClassify = mInFever ? true : (isLeft == IsLeftTeam(CurrentSlimeId));
            if (isSuccessClassify)
            {
                PullNoneTeamList();

                RefreshTeacherForward(playMotion:true);

                return true;
            }

            return false;
        }

        public void BroadCastFeverStart()
        {
            mInFever = true;

            foreach (var noneTeam in mNoneTeamList)
                noneTeam.ChangeEmotion(EmotionType.Excited);
        }

        public void BroadCastFeverEnd()
        {
            mInFever = false;

            foreach (var noneTeam in mNoneTeamList)
                noneTeam.ChangeEmotion(EmotionType.Idle);
        }

        public void BroadCastPenaltyStart()
        {
            var firstSlime = mNoneTeamList.First();

            firstSlime?.ChangeEmotion(EmotionType.Shock);
        }

        public void BroadCastPenaltyEnd()
        {
            var firstSlime = mNoneTeamList.First();

            firstSlime?.ChangeEmotion(EmotionType.Idle);
        }

        void RefreshTeacherForward(bool playMotion)
        {
            NoneTeam first = mNoneTeamList.First();

            bool isLeftTeam = IsLeftTeam(first?.Id);

            mImage_SlimeTeacher.rectTransform.localScale = new Vector3((isLeftTeam ? -1 : 1), 1, 1);
            if (playMotion)
            {
                mImage_SlimeTeacher.DORewind();
                mImage_SlimeTeacher.DOPlay();
            }
        }

        void RefreshTeamList()
        {
            mLeftTeamIdList = new List<string>();
            mRightTeamIdList = new List<string>();

            // 속성을 섞는다.
            var teamTypes = MLand.GameData.SlimeData.Values.Select(x => x.elementalType).Distinct();
            var shuffleTypes = DataUtil.GetShuffleDatas(teamTypes).ToArray();

            var shuffleDatas = DataUtil.GetShuffleDatas(MLand.GameData.SlimeData.Values);

            var leftTeamType = shuffleTypes.First();
            var rightTeamType = shuffleTypes.Last();

            mLeftTeamIdList = shuffleDatas
                .Where(x => x.elementalType == leftTeamType && DataUtil.CanAttendMiniGame(x.id))
                .Take(MaxTeamCount)
                .Select(x => x.id).ToList();

            mRightTeamIdList = shuffleDatas
                .Where(x => x.elementalType == rightTeamType && DataUtil.CanAttendMiniGame(x.id))
                .Take(MaxTeamCount)
                .Select(x => x.id).ToList();

            for (int i = 0; i < MaxTeamCount; ++i)
            {
                mImgLeftTeams[i].sprite = MonsterLandUtil.GetRandomSlimeEmotionImg(mLeftTeamIdList[i]);
                mImgRightTeams[i].sprite = MonsterLandUtil.GetRandomSlimeEmotionImg(mRightTeamIdList[i]); ;
            }

            mImgLeftTeamButton.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_ElementalCourses_{leftTeamType}");
            mImgLeftTeamButtonEmblem.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_ElementalCourses_{leftTeamType}_Emblem");
            mImgRightTeamButton.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_ElementalCourses_{rightTeamType}");
            mImgRightTeamButtonEmblem.sprite = MLand.Atlas.GetUISprite($"Btn_Minigame_ElementalCourses_{rightTeamType}_Emblem");

            RefreshTeamActiveList();
        }

        void RefreshTeamActiveList()
        {
            for (int i = 0; i < MaxTeamCount; ++i)
            {
                bool isActive = mLevel >= i + 1;

                InternalSetActive(mImgLeftTeams[i].gameObject, isLeft:true, isActive);
                InternalSetActive(mImgRightTeams[i].gameObject, isLeft:false, isActive);
            }

            void InternalSetActive(GameObject teamObj, bool isLeft, bool isActive)
            {
                if (teamObj.activeSelf == false)
                {
                    if (isActive)
                    {
                        RectTransform rectTm = teamObj.GetComponent<RectTransform>();

                        rectTm.DORewind();

                        float orgPosX = rectTm.anchoredPosition.x;
                        float orgPosY = rectTm.anchoredPosition.y;

                        rectTm.anchoredPosition = new Vector3(isLeft ? -300 : 300, orgPosY);
                        rectTm.DOAnchorPosX(orgPosX, 0.5f)
                            .SetEase(Ease.OutBack);
                    }
                }

                teamObj.SetActive(isActive);
            }
        }

        void RefreshNoneTeamList()
        {
            if (mNoneTeamList != null)
            {
                foreach (NoneTeam noneTeam in mNoneTeamList)
                    MLand.ObjectPool.ReleaseUI(noneTeam.gameObject);
            }

            mNoneTeamList = new List<NoneTeam>();

            for (int i = 0; i < MaxNoneTeamCount; ++i)
            {
                NoneTeam newNoneTeam = CreateNoneTeam();

                NoneTeamMoveInfo info = mNoneTeamMoveInfoList[i];

                newNoneTeam.SetRectTmInfo(info);

                mNoneTeamList.Add(newNoneTeam);
            }
        }

        NoneTeam CreateNoneTeam()
        {
            GameObject noneTeamObj = MLand.ObjectPool.AcquireUI("NoneTeam", mNoneTeamParent.transform);

            noneTeamObj.transform.SetAsFirstSibling();

            NoneTeam noneTeam = noneTeamObj.GetOrAddComponent<NoneTeam>();

            noneTeam.Init(GetAnyTeamSlime());

            return noneTeam;
        }

        string GetAnyTeamSlime()
        {
            var anyIds = Util.Dice() ? mLeftTeamIdList.Take(mLevel) : mRightTeamIdList.Take(mLevel);

            System.Random random = new System.Random();

            var shuffleSlimeIds = anyIds.OrderBy(x => random.Next());

            return shuffleSlimeIds.First();
        }

        void PullNoneTeamList()
        {
            // 이동 시키기
            for (int i = 1; i < MaxNoneTeamCount; ++i)
            {
                RectTransform moveTm = mNoneTeamList[i].RectTm;
                NoneTeamMoveInfo info = mNoneTeamMoveInfoList[i - 1];

                moveTm.DOLocalMove(info.LocalPos, 0.25f);
                moveTm.DOSizeDelta(info.SizeDelta, 0.25f);
            }

            // 맨 앞 날려버리기
            NoneTeam firstNoneTeam = mNoneTeamList.First();
            firstNoneTeam.ChangeEmotion(EmotionType.Happy);
            firstNoneTeam.Throw(IsLeftTeam(firstNoneTeam.Id));
            mNoneTeamList.RemoveAt(0);

            // 새로 생성 후 맨뒤로 보내기
            NoneTeam newNoneTeam = CreateNoneTeam();

            newNoneTeam.SetRectTmInfo(mNoneTeamMoveInfoList.Last());

            mNoneTeamList.Add(newNoneTeam);
        }

        bool IsLeftTeam(string id)
        {
            return mLeftTeamIdList.Contains(id);
        }
    }

    struct NoneTeamMoveInfo
    {
        public Vector3 LocalPos;
        public Vector2 SizeDelta;
    }

    class NoneTeam : MonoBehaviour
    {
        string mId;
        Image mImage;
        public string Id => mId;
        public RectTransform RectTm => mImage.rectTransform;
        public void Init(string id)
        {
            mId = id;
            mImage = gameObject.GetComponent<Image>();
            mImage.sprite = MonsterLandUtil.GetSlimeUIImg(id);
        }

        public void ChangeEmotion(EmotionType type)
        {
            mImage.sprite = MonsterLandUtil.GetSlimeEmotionImg(mId, type);
        }

        public void SetRectTmInfo(NoneTeamMoveInfo moveInfo)
        {
            SetRectTmInfo(moveInfo.LocalPos, moveInfo.SizeDelta);
        }

        public void Throw(bool isLeft)
        {
            SoundPlayer.PlaySlimeThrow();

            DOTween.Sequence()
                .Join(transform.DOLocalJump(isLeft ? Vector3.left * 1000f : Vector3.right * 1000f, UnityEngine.Random.Range(125f, 500f), 1, 1f))
                .Join(transform.DOScale(Vector3.one * 0.5f, 1f))
                .Join(transform.DOLocalRotate(new Vector3(0, 0, -360 * 3f), 1f))
                .OnComplete(
                () =>
                {
                    transform.localScale = Vector3.one;
                    transform.localRotation = Quaternion.Euler(Vector3.zero);

                    MLand.ObjectPool.ReleaseUI(gameObject);
                });
        }

        void SetRectTmInfo(Vector3 localPos, Vector2 sizeDelta)
        {
            RectTm.localPosition = localPos;
            RectTm.sizeDelta = sizeDelta;
        }
    }
}