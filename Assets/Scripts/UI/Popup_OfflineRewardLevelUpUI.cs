using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

namespace MLand
{
    class Popup_OfflineRewardLevelUpUI : PopupBase
    {
        public void Init()
        {
            PlayKingSlimeSpeechMotion(StringTableUtil.Get("OfflineReward_KingSlimeSpeechLevelUp"));

            TextMeshProUGUI textAddWorkTime = gameObject.FindComponent<TextMeshProUGUI>("Text_AddWorkTime");
            textAddWorkTime.text = StringTableUtil.Get("OfflineReward_AddWorkTime");

            TextMeshProUGUI textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = StringTableUtil.Get("OfflineReward_TitleDesc");

            TextMeshProUGUI textMaxTime = gameObject.FindComponent<TextMeshProUGUI>("Text_OfflineRewardMaxTime");
            int maxLevel = MLand.GameData.OfflineRewardLevelUpData.Max(x => x.level);
            (int hour, int minute) result = CalcHourAndMinute(DataUtil.GetOfflineMaxTimeForMinute(maxLevel));
            string maxTime = GetTimeStr(result.hour, result.minute);
            StringParam param = new StringParam("max", maxTime);
            textMaxTime.text = StringTableUtil.Get("OfflineReward_MaxTime", param);

            SetTitleText(StringTableUtil.Get("Title_OfflineRewardLevelUp"));
            SetUpCloseAction();

            InitButtons();

            Refresh();
        }

        void PlayKingSlimeSpeechMotion(string text)
        {
            TextMeshProUGUI textSpeech = gameObject.FindComponent<TextMeshProUGUI>("Text_Speech");
            textSpeech.text = string.Empty;
            textSpeech.DOText(text, 1f);
        }

        void Refresh()
        {
            RefreshPrice();
            RefreshMaxTime();
        }

        void RefreshMaxTime()
        {
            int currLevel = MLand.SavePoint.OfflineRewardManager.Level;
            int nextLevel = currLevel + 1;

            bool isMaxLevel = MLand.SavePoint.OfflineRewardManager.IsMaxLevel();

            var levelUp = gameObject.FindGameObject("LevelUp");
            var maxLevel = gameObject.FindGameObject("MaxLevel");

            levelUp.SetActive(!isMaxLevel);
            maxLevel.SetActive(isMaxLevel);

            InternalRefreshMaxTime("Text_BeforeMaxTime", currLevel);
            InternalRefreshMaxTime("Text_AfterMaxTime", nextLevel);

            void InternalRefreshMaxTime(string textName, int level)
            {
                TextMeshProUGUI textMaxTime = gameObject.FindComponent<TextMeshProUGUI>(textName);

                (int hour, int minute) = CalcHourAndMinute(DataUtil.GetOfflineMaxTimeForMinute(level));

                textMaxTime.text = GetTimeStr(hour, minute);
            }
        }

        string GetTimeStr(int hour, int minute)
        {
            string timeStr;

            if (hour > 0 && minute > 0)
            {
                StringParam param = new StringParam("hour", hour.ToString());
                param.AddParam("minute", minute.ToString());

                timeStr = StringTableUtil.Get("UIString_HourAndMinute", param);
            }
            else if (hour > 0)
            {
                StringParam param = new StringParam("hour", hour.ToString());

                timeStr = StringTableUtil.Get("UIString_Hour", param);
            }
            else
            {
                StringParam param = new StringParam("minute", minute.ToString());

                timeStr = StringTableUtil.Get("UIString_Minute", param);
            }

            return timeStr;
        }


        (int hour, int minute) CalcHourAndMinute(int totalMinute)
        {
            int hour = totalMinute / TimeUtil.MinutesInHour;
            totalMinute -= hour * TimeUtil.MinutesInHour;

            int minute = totalMinute;

            return (hour, minute);
        }

        void RefreshPrice()
        {
            int currLevel = MLand.SavePoint.OfflineRewardManager.Level;
            double price = DataUtil.GetOfflineLevelUpPrice(currLevel);
            bool isEnoughGem = price == 0 ? false : MLand.SavePoint.IsEnoughGem(price);

            TextMeshProUGUI textGemPrice = gameObject.FindComponent<TextMeshProUGUI>("Text_GemPrice");
            textGemPrice.text = isEnoughGem ? price.ToString() : $"<color=red>{price}</color>";

            Image buttonLevelUp = gameObject.FindComponent<Image>("Button_LevelUp");
            string btnName = isEnoughGem ? "Btn_Square_Yellow" : "Btn_Square_LightGray";
            buttonLevelUp.sprite = MLand.Atlas.GetUISprite(btnName);
        }

        void InitButtons()
        {
            var buttonLevelUp = gameObject.FindComponent<Button>("Button_LevelUp");
            buttonLevelUp.SetButtonAction(OnLevelUp);

            var buttonDesc = gameObject.FindComponent<Button>("Button_Desc");
            buttonDesc.SetButtonAction(ShowDesc);
        }

        void OnLevelUp()
        {
            if (MLand.SavePoint.OfflineRewardManager.IsMaxLevel())
            {
                MonsterLandUtil.ShowSystemErrorMessage("OfflineRewardMaxLevel");

                return;
            }

            int currLevel = MLand.SavePoint.OfflineRewardManager.Level;
            double price = DataUtil.GetOfflineLevelUpPrice(currLevel);
            if (MLand.SavePoint.IsEnoughGem(price) == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughGem");

                return;
            }

            string title = StringTableUtil.Get("Title_Confirm");

            StringParam param = new StringParam("gem", price.ToString());
            string desc = StringTableUtil.Get("Confirm_UseGem", param);

            MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);


            void OnConfirm()
            {
                if (MLand.GameManager.UseGem(price))
                {
                    MLand.SavePoint.OfflineRewardManager.LevelUp();

                    MLand.SavePoint.Save();

                    MonsterLandUtil.ShowSystemMessage(StringTableUtil.GetSystemMessage("OfflineRewardLevelUp"));

                    Refresh();

                    SoundPlayer.PlayOfflineLevelUp();
                }
            }
        }

        void ShowDesc()
        {
            PlayKingSlimeSpeechMotion(StringTableUtil.Get("OfflineReward_KingSlimeSpeechDesc"));
        }
    }
}