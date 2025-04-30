using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class NewDotManager
    {
        GameObject mSlimeNewDot;
        GameObject mAttendanceNewDot;
        GameObject mQuestNewDot;
        GameObject mAchievementsNewDot;
        public void Init(LobbyUI lobby)
        {
            mSlimeNewDot = lobby.Find("Slime_NewDot");
            mAttendanceNewDot = lobby.Find("Attendance_NewDot");
            mQuestNewDot = lobby.Find("Quest_NewDot");
            mAchievementsNewDot = lobby.Find("Achievements_NewDot");

            Refresh();
        }

        public void Refresh()
        {
            RefreshSlimeNewDot();
            RefreshAttendanceNewDot();
            RefreshQuestNewDot();
            RefreshAchievementsNewDot();
        }

        void RefreshSlimeNewDot()
        {
            mSlimeNewDot.SetActive(MLand.SavePoint.AnyCanReceiveLevelReward());
        }

        void RefreshAttendanceNewDot()
        {
            mAttendanceNewDot.SetActive(MLand.SavePoint.Attendance.AnyCanReceiveReward());
        }

        void RefreshQuestNewDot()
        {
            bool anyDailyQuestCanReceive = MLand.SavePoint.DailyQuest.AnyQuestCanReceiveReward();
            bool anyStepQuestCanReceive = MLand.SavePoint.StepQuest.AnyQuestCanReceiveReward();

            mQuestNewDot.SetActive(anyDailyQuestCanReceive || anyStepQuestCanReceive);

            Popup_QuestUI popup_quest = MLand.PopupManager.GetPopup<Popup_QuestUI>();

            popup_quest?.RefreshNewDot();
        }

        

        void RefreshAchievementsNewDot()
        {
            bool show = MLand.SavePoint.Achievements.AnyAchievementsCanReceiveReward() ||
                SavePointBitFlags.AchievementsHelpShow.IsOff();

            mAchievementsNewDot.SetActive(show);
        }
    }
}