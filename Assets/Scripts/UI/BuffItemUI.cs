using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Linq;

namespace MLand
{
    class Lobby_BuffItemUI : MonoBehaviour
    {
        float mInterval;
        int mDuration;

        BuffData mData;
        GameObject mActiveObj;
        GameObject mApplyObj;
        GameObject mInActiveObj;
        TextMeshProUGUI mDurationTimeText;
        public BuffType BuffType => mData.buffType;
        bool IsActive => mDuration > 0;
        public int Duration => mDuration;
        public void Init(BuffItemUIManager parent, BuffType type)
        {
            mData = MLand.GameData.BuffData.TryGet(type);

            Button button = gameObject.FindComponent<Button>("Button_Buff");
            button.SetButtonAction(() =>
            {
                var popup = MLand.PopupManager.CreatePopup<Popup_BuffUI>();

                popup.Init(parent);
            });

            mActiveObj = gameObject.FindGameObject("Active");
            mInActiveObj = gameObject.FindGameObject("InActive");
            mApplyObj = mActiveObj.FindGameObject("Apply");
            mDurationTimeText = mActiveObj.FindComponent<TextMeshProUGUI>("Text_Buff_DurationTime");

            // 버프 이미지 초기화
            Sprite buffSprite = MLand.Atlas.GetUISprite($"Buff_{mData.buffType}");
            Image activeBuffImg = mActiveObj.FindComponent<Image>("Image_Buff");
            Image inActiveBuffImg = mInActiveObj.FindComponent<Image>("Image_Buff2");
            Image activeLightBuffImg = mActiveObj.FindComponent<Image>("Image_ActiveLight");
            activeBuffImg.sprite = buffSprite;
            inActiveBuffImg.sprite = buffSprite;
            activeLightBuffImg.sprite = MLand.Atlas.GetUISprite($"Buff_{mData.buffType}_Activate");

            ApplySavePoint();
        }

        public void ApplySavePoint()
        {
            BuffInfo buff = MLand.SavePoint.BuffManager.GetBuffInfo(BuffType);
            if (buff.BuffDurationEndTime > 0)
            {
                // 혹시라도 버프 지속시간이 남아 있으면 다시 시작
                int remainDurationTime = buff.BuffDurationEndTime - TimeUtil.Now;
                if (remainDurationTime > 0)
                {
                    mDuration = remainDurationTime;
                }
            }

            RefreshActive();
            mApplyObj?.SetActive(IsActive);
        }

        void RefreshActive()
        {
            BuffInfo buff = MLand.SavePoint.BuffManager.GetBuffInfo(BuffType);

            // 오늘 사용가능한 최대 횟수를 모두 사용했으면 처리
            bool isMaxCount = buff.DailyCounter?.IsMaxCount(mData.maxDailyCount) ?? false;

            mActiveObj?.SetActive(!isMaxCount || IsActive);
            mInActiveObj?.SetActive(isMaxCount && IsActive == false);
        }

        public void OnUpdate(float dt)
        {
            mInterval -= dt;
            if (mInterval <= 0)
            {
                mInterval = 1f;

                if (IsActive)
                {
                    mDuration -= 1;
                    if (mDuration <= 0)
                    {
                        EndBuff();
                    }

                    Popup_BuffUI popup = MLand.PopupManager.GetPopup<Popup_BuffUI>();

                    popup?.SetDuration(BuffType, mDuration);

                    SetDurationTimeText();
                }
            }
        }

        public void ActiveBuff()
        {
            mDuration = mData.duration;

            mApplyObj.SetActive(IsActive);

            SetDurationTimeText();
        }

        void EndBuff()
        {
            mDuration = 0;

            RefreshActive();

            mApplyObj.SetActive(IsActive);
        }

        void SetDurationTimeText()
        {
            int duration = Mathf.RoundToInt(mDuration);

            var result = CalcMiunuteAndSeconds(duration);

            StringParam param = new StringParam("minute", result.minute.ToString());
            param.AddParam("seconds", result.seconds.ToString());

            mDurationTimeText.text = StringTableUtil.Get("UIString_MinuteAndSeconds", param);

            (int minute, int seconds) CalcMiunuteAndSeconds(int totalSeconds)
            {
                int minute = totalSeconds / TimeUtil.SecondsInMinute;
                totalSeconds -= minute * TimeUtil.SecondsInMinute;

                int seconds = totalSeconds;

                return (minute, seconds);
            }
        }
    }
}