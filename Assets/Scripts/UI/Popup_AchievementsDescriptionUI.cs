using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MLand
{
    class Popup_AchievementsDescriptionUI : PopupBase
    {
        string mId;
        Action mOnTakeReward;
        public void Init(string id, Action onTakeReward)
        {
            mId = id;
            mOnTakeReward = onTakeReward;

            SetTitleText(StringTableUtil.Get("Title_AchievementsDesc"));
            SetUpCloseAction();

            var data = MLand.GameData.AchievementsData.TryGet(id);
            var rewardData = MLand.GameData.RewardData.TryGet(data.rewardId);

            // 업적 아이콘
            var imgIcon = gameObject.FindComponent<Image>("Image_Icon");
            imgIcon.sprite = MLand.Atlas.GetUISprite(data.spriteImg);

            // 업적 이름
            var textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");
            textName.text = StringTableUtil.Get($"Achievements_Name_{id}");

            // 업적 설명
            var textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = StringTableUtil.Get($"Achievements_Desc_{id}");

            // 업적 보상량 ( 무조건 잼 )
            var textGemReward = gameObject.FindComponent<TextMeshProUGUI>("Text_GemReward");
            textGemReward.text = $"{rewardData.gemReward}";

            InitButtons();
        }

        void InitButtons()
        {
            // 보상 받기 스트링 테이블 연결
            var textTakeReward = gameObject.FindComponent<TextMeshProUGUI>("Text_TakeReward");
            textTakeReward.text = StringTableUtil.Get("UIString_TakeReward");

            // 공유 스트링 테이블 연결
            var textShare = gameObject.FindComponent<TextMeshProUGUI>("Text_Share");
            textShare.text = StringTableUtil.Get("UIString_Share");

            RefreshButtons();
        }

        void RefreshButtons()
        {
            var buttonShare = gameObject.FindComponent<Button>("Button_Share");
            buttonShare.SetButtonAction(OnShareButton);

            var buttonTakeReward = gameObject.FindComponent<Button>("Button_TakeReward");
            buttonTakeReward.SetButtonAction(OnTakeRewardButton);

            var info = MLand.SavePoint.Achievements.GetAchievementsInfo(mId);
            bool activeTakeReward = info.CanReceiveReward && info.IsReceivedReward == false;

            var imgButton = buttonTakeReward.GetComponent<Image>();

            string buttonName = activeTakeReward ? "Btn_Square_Yellow" : "Btn_Square_LightGray";
            imgButton.sprite = MLand.Atlas.GetUISprite(buttonName);

            var newDotObj = buttonTakeReward.gameObject.FindGameObject("NewDot");
            newDotObj.SetActive(activeTakeReward);
        }

        void OnShareButton()
        {
            StartCoroutine(StartShare());
        }

        IEnumerator StartShare()
        {
            MLand.GameManager.StartTouchBlock(5f);

            yield return new WaitForEndOfFrame();

            CanvasScaler cs = MLand.Lobby.CanvasScaler;
            RectTransform tm = gameObject.FindComponent<RectTransform>("CaptureArea");

            var photo = Util.CaptureTextureInRectTmArea(cs, tm);

            new NativeShare().AddFile(photo).SetUrl(MLand.GameData.CommonData.playStoreUrl).Share();

            MLand.GameManager.EndTouchBlock();
        }

        void OnTakeRewardButton()
        {
            var info = MLand.SavePoint.Achievements.GetAchievementsInfo(mId);
            if (info == null)
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            if (info.CanReceiveReward == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("CantReceiveAchievementsReward");

                return;
            }

            if (info.IsReceivedReward)
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsAlreadyReceiveAchievementsReward");

                return;
            }

            MLand.SavePoint.ReceiveAchievementsReward(mId);

            MLand.Lobby.RefreshAllCurrencyText();

            MLand.Lobby.RefreshNewDot();

            RefreshButtons();

            mOnTakeReward?.Invoke();
        }
    }
}