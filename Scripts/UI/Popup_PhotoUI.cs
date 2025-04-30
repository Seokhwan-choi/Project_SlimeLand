using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace MLand
{
    class Popup_PhotoUI : PopupBase
    {
        Texture2D mPhoto;
        GameObject mBefore;
        GameObject mAfter;
        bool mIsAfterTakePhoto;
        public void Init()
        {
            mIsAfterTakePhoto = false;
            mBefore = gameObject.FindGameObject("Before");
            mAfter = gameObject.FindGameObject("After");

            RefreshBeforeAfter();

            InitButtonActions();

            MLand.Lobby.HideUI();
        }

        void RefreshBeforeAfter()
        {
            mBefore.SetActive(!mIsAfterTakePhoto);
            mAfter.SetActive(mIsAfterTakePhoto);

            var textDate = gameObject.FindComponent<TextMeshProUGUI>("Text_Date");

            DateTime now = TimeUtil.UtcNow.AddHours(9);

            textDate.text = $"{now.Year}/{now.Month}/{now.Day}";
        }

        void InitButtonActions()
        {
            var buttonBeforeClose = mBefore.FindComponent<Button>("Button_Close");
            buttonBeforeClose.SetButtonAction(() => OnClose());

            var buttonTakePhoto = gameObject.FindComponent<Button>("Button_TakePhoto");
            buttonTakePhoto.SetButtonAction(OnTakePhoto);

            var buttonSharePhoto = gameObject.FindComponent<Button>("Button_SharePhoto");
            buttonSharePhoto.SetButtonAction(OnSharePhoto);

            var buttonAfterModal = mAfter.FindComponent<Button>("Button_Modal");
            buttonAfterModal.SetButtonAction(OnCloseShareMode);

            var buttonAfterClose = mAfter.FindComponent<Button>("Button_Close");
            buttonAfterClose.SetButtonAction(OnCloseShareMode);
        }

        void OnTakePhoto()
        {
            StartCoroutine(StartPhoto());
        }

        IEnumerator StartPhoto()
        {
            yield return new WaitForEndOfFrame();

            SoundPlayer.PlayTakePhoto();

            CanvasScaler cs = MLand.Lobby.CanvasScaler;
            RectTransform tm = GetComponent<RectTransform>();

            mPhoto = Util.CaptureTextureInRectTmArea(cs, tm);

            Image imgSharePhoto = gameObject.FindComponent<Image>("Image_SharePhoto");

            imgSharePhoto.sprite = Sprite.Create(mPhoto, new Rect(0, 0, mPhoto.width, mPhoto.height), new Vector2(0f, 0f));

            Image flash = gameObject.FindComponent<Image>("Flash");
            flash.color = Color.white;
            flash.DORewind();
            flash.DOFade(0f, 0.125f)
                .SetAutoKill(false)
                .SetLoops(3, LoopType.Yoyo);

            yield return new WaitForSeconds(0.45f);

            mIsAfterTakePhoto = true;

            RefreshBeforeAfter();
        }

        void OnSharePhoto()
        {
            new NativeShare().AddFile(mPhoto).SetUrl(MLand.GameData.CommonData.playStoreUrl).SetCallback((result, target) =>
            {
                if (result == NativeShare.ShareResult.Shared)
                {
                    OnCloseShareMode();

                    MLand.SavePoint.CheckQuests(QuestType.PhotoShare);
                    MLand.SavePoint.CheckAchievements(AchievementsType.Influencer);
                }
            }).Share();
        }

        void OnCloseShareMode()
        {
            mIsAfterTakePhoto = false;

            RefreshBeforeAfter();
        }
    }

}