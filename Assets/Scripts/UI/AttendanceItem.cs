using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class AttendanceItem : MonoBehaviour
    {
        Button mButtonActiveReceive;
        Button mButtonInactiveReceive;

        Image mImgModal;
        Image mImgReceived;
        GameObject mReceivedObj;
        GameObject mNewDotObj;
        public void Init(int day, bool isLastDay)
        {
            TextMeshProUGUI textDay = gameObject.FindComponent<TextMeshProUGUI>("Text_Day");
            textDay.text = $"Day.{day}";

            Image imgTag = gameObject.FindComponent<Image>("Image_Tag");
            imgTag.enabled = isLastDay;

            mReceivedObj = gameObject.FindGameObject("Received");
            mNewDotObj = gameObject.FindGameObject("NewDot");

            bool isReceivedReward = MLand.SavePoint.Attendance.IsAlreadyReceivedReward(day);
            bool isReadyForReceive = MLand.SavePoint.Attendance.IsReadyForReceiveReward(day);
            mImgReceived = mReceivedObj.FindComponent<Image>("Image_Received");
            mImgModal = mReceivedObj.FindComponent<Image>("Image_Modal");
            mReceivedObj.SetActive(isReceivedReward);

            InitRewardInfo(day);
            InitReceiveButton(day);
            SetActiveReceiveButton(isReadyForReceive);
            mNewDotObj.SetActive(isReadyForReceive);
        }

        void InitReceiveButton(int day)
        {
            mButtonActiveReceive = gameObject.FindComponent<Button>("Button_ActiveReceive");
            mButtonActiveReceive.SetButtonAction(() => OnReceiveButton(day));

            mButtonInactiveReceive = gameObject.FindComponent<Button>("Button_InactiveReceive");
        }

        void SetActiveReceiveButton(bool isActive)
        {
            mButtonActiveReceive.gameObject.SetActive(isActive);
            mButtonInactiveReceive.gameObject.SetActive(!isActive);
        }

        void OnReceiveButton(int day)
        {
            if ( MLand.SavePoint.IsAlreadyReceiveAttendanceReward(day) )
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsAlreadyReceiveAttendanceReward");

                return;
            }

            if ( MLand.SavePoint.IsReadyForReceiveAttendanceReward(day) == false )
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsNotReadyForReceiveAttendanceReward");

                return;
            }

            PlayMotion();

            MLand.SavePoint.ReceiveAttendanceReward(day);

            MLand.Lobby.RefreshAllCurrencyText();

            MLand.Lobby.RefreshNewDot();

            MLand.SavePoint.CheckQuests(QuestType.Attendance);

            SoundPlayer.PlayAttendanceCheck();
        }

        void PlayMotion()
        {
            // 버튼 비활성화
            SetActiveReceiveButton(false);
            // 레드 닷 비활성화
            mNewDotObj.SetActive(false);
            // 보상 받았다 활성화
            mReceivedObj.SetActive(true);

            // 도장 떨어지는 연출
            mImgReceived.transform.localScale = Vector3.one * 5f;
            mImgReceived.transform.DOScale(Vector3.one, 0.25f)
                .OnComplete(() => SoundPlayer.PlayDropStamp());

            // 점점 어두워지는 연출
            Color startColor = Color.white;
            startColor.a = 0f;

            mImgModal.color = startColor;
            mImgModal.DOFade(1f, 0.25f);
        }

        void InitRewardInfo(int day)
        {
            AttendanceRewardData attendanceRewardData = MLand.GameData.AttendanceRewardData.TryGet(day);

            var rewardData = MLand.GameData.RewardData.TryGet(attendanceRewardData.rewardId);

            ItemInfo rewardInfo = ItemInfo.CreateRewardInfo(rewardData);

            Image imgReward = gameObject.FindComponent<Image>("Image_Reward");
            imgReward.sprite = rewardInfo.GetIconImg();

            Image imgGradeInCircle = gameObject.FindComponent<Image>("Image_InCircle");
            imgGradeInCircle.sprite = rewardInfo.GetCircleImg();

            TextMeshProUGUI textRewardAmount = gameObject.FindComponent<TextMeshProUGUI>("Text_RewardAmount");
            textRewardAmount.text = $"X {rewardInfo.GetAmountString()}";

            Button buttonShowRewardInfo = gameObject.FindComponent<Button>("Button_ShowRewardInfo");
            buttonShowRewardInfo.SetButtonAction(() => MonsterLandUtil.ShowDescPopup(rewardInfo));
        }
    }
}