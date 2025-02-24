using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MLand
{
    class Popup_AttendanceUI : PopupBase
    {
        Dictionary<int, AttendanceItem> mAttendanceItems;
        public void Init()
        {
            CheckAttendance();

            SetUpCloseAction();

            SetTitleText(StringTableUtil.Get("Title_Attendance"));

            mAttendanceItems = new Dictionary<int, AttendanceItem>();

            for(int i = 0; i < MLand.GameData.AttendanceCommonData.maxDay; ++i)
            {
                int day = i + 1;

                bool isLastDay = day == MLand.GameData.AttendanceCommonData.maxDay;

                GameObject attendanceItemObj = gameObject.FindGameObject($"AttendanceItem_{day}");

                AttendanceItem attendanceItem = attendanceItemObj.GetOrAddComponent<AttendanceItem>();

                attendanceItem.Init(day, isLastDay);

                mAttendanceItems.Add(day, attendanceItem);
            }
        }

        void CheckAttendance()
        {
            if (MLand.SavePoint.Attendance.Check())
            {
                MLand.SavePoint.Save();
            }

            string test = MLand.SavePoint.Attendance.LastCheckDateNum.ToString();

            //var textTest = gameObject.FindComponent<TextMeshProUGUI>("Text_Test");

            //textTest.text = test;

            //Debug.LogError(test);
        }
    }
}

