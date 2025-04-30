using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

namespace MLand
{
    class PopupStatus_CostumeUI : PopupStatusUI
    {
        TextMeshProUGUI mTextSlimeName;
        SlimeCostumePreivew mSlimeCostumePreview;
        
        int mSlimeIndex;
        string mCurSlimeId;
        public string CurSlimeId => mCurSlimeId;

        CostumeTabUIManager mTabManager;
        public override void Init(PopupStatus type)
        {
            base.Init(type);

            mSlimeIndex = 0;
            mCurSlimeId = MLand.GameData.SlimeData.Keys.First();

            mTabManager = new CostumeTabUIManager();
            mTabManager.Init(this);

            var previewObj = Util.Instantiate("SlimeCostumePreview");

            mSlimeCostumePreview = previewObj.GetOrAddComponent<SlimeCostumePreivew>();
            mSlimeCostumePreview.Init();
            mSlimeCostumePreview.RefreshSlimeImg(mCurSlimeId);
            mSlimeCostumePreview.SetActive(false);

            mTextSlimeName = gameObject.FindComponent<TextMeshProUGUI>("Text_SlimeName");

            SetButtonActions();

            RefreshSlimeName();
            RefreshSlimeCostumes();
        }

        void RefreshSlimeImg()
        {
            mSlimeCostumePreview.RefreshSlimeImg(mCurSlimeId);
        }

        void SetButtonActions()
        {
            var buttonSlimeSelect = gameObject.FindComponent<Button>("Button_SlimeSelect");
            buttonSlimeSelect.SetButtonAction(() =>
            {
                var popup = MLand.PopupManager.CreatePopup<Popup_CostumeSlimeSelectUI>();

                popup.Init(this);
            });

            var buttonLeft = gameObject.FindComponent<Button>("Button_Left");
            buttonLeft.SetButtonAction(() => OnChangeSlime(true));

            var buttonRight = gameObject.FindComponent<Button>("Button_Right");
            buttonRight.SetButtonAction(() => OnChangeSlime(false));
        }

        void OnChangeSlime(bool isLeft)
        {
            int maxCount = MLand.SavePoint.SlimeManager.SlimeInfoList.Count(x => x.Id != MLand.GameData.GoldSlimeCommonData.id);

            if ( isLeft )
            {
                mSlimeIndex--;
                if ( mSlimeIndex < 0 )
                {
                    mSlimeIndex = maxCount - 1;
                }
            }
            else
            {
                mSlimeIndex++;
                if (mSlimeIndex >= maxCount)
                {
                    mSlimeIndex = 0;
                }
            }

            var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mSlimeIndex);
            if (slimeInfo != null)
            {
                if (slimeInfo.Id == MLand.GameData.GoldSlimeCommonData.id)
                {
                    OnChangeSlime(isLeft);
                }
                else
                {
                    OnChangeSlime(slimeInfo.Id);
                }
            }
        }

        public override void OnUpdate()
        {
            mTabManager.OnUpdate();
        }

        public override void OnEnter(int subIdx)
        {
            CostumeTab tab = (CostumeTab)subIdx;

            mTabManager.ChangeTab(tab);

            mSlimeCostumePreview.SetActive(true);

            MLand.GameManager.SetFollowSlime(mCurSlimeId);

            mSlimeIndex = MLand.SavePoint.SlimeManager.GetSlimeIndex(mCurSlimeId);
        }

        public override void OnCloseButtonAction()
        {
            MLand.Lobby.HidePopupStatus();

            MLand.CameraManager.ResetFollowInfo();
        }

        public override void Localize()
        {
            RefreshSlimeName();
        }

        public void OnChangeSlime(string slimeId)
        {
            mCurSlimeId = slimeId;

            MLand.GameManager.SetFollowSlime(slimeId);

            RefreshSlimeName();
            RefreshSlimeCostumes();

            mTabManager.GetCurTab()?.OnTabEnter();
        }

        public void OnChangeSlime(string slimeId, int slimeIndex)
        {
            OnChangeSlime(slimeId);

            mSlimeIndex = slimeIndex;
        }

        void RefreshSlimeName()
        {
            if (mCurSlimeId.IsValid())
            {
                mTextSlimeName.text = StringTableUtil.GetName(mCurSlimeId);
            }
        }

        public void RefreshSlimeCostumes()
        {
            // 코스튬 다시 그려주기
            var slime = MLand.GameManager.GetSlime(mCurSlimeId);

            slime?.RefreshCostumes();

            RefreshSlimeImg();
        }
    }

    class EquipCostumeItemUI : MonoBehaviour
    {
        Image mImgCostume;
        Image mImgCostumeType;
        public void Init(CostumeType type)
        {
            mImgCostumeType = gameObject.FindComponent<Image>("Image_CostumeType");
            mImgCostumeType.sprite = MLand.Atlas.GetUISprite($"Img_Silhouette_{type}");
            mImgCostume = gameObject.FindComponent<Image>("Image_EquipCostume");
        }

        public void RefreshCostume(string costumeId)
        {
            // 착용중인 코스튬 셋팅
            if (costumeId.IsValid())
            {
                var data = MLand.GameData.CostumeData.TryGet(costumeId);
                if (data == null)
                    return;

                // 이미지 보여주자
                mImgCostume.enabled = true;
                mImgCostume.sprite = MLand.Atlas.GetUISprite(data.thumbnail);

                // 이름은 가려주자
                mImgCostumeType.enabled = false;
            }
            else
            {
                mImgCostume.enabled = false;
                mImgCostumeType.enabled = true;
            }
        }
    }

    class SlimeCostumePreivew : MonoBehaviour
    {
        CharacterAnim mSlimeAnim;

        public void Init()
        {
            var spark = gameObject.FindGameObject("SparkleAreaYellow");
            spark.SetActive(false);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void RefreshSlimeImg(string id)
        {
            EmotionType randEmotion = CharacterUtil.GetRandEmotion();

            if (mSlimeAnim == null)
            {
                var slimeObj = gameObject.FindGameObject("Slime");
                mSlimeAnim = slimeObj.GetOrAddComponent<CharacterAnim>();
                mSlimeAnim.Init(id);

                mSlimeAnim.PlayIdle(randEmotion);
            }
            else
            {
                mSlimeAnim.ChangeSlime(id);

                mSlimeAnim.PlayIdle(randEmotion);
            }
        }
    }
}