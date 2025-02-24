using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MLand
{
    class Popup_AchievementsResultUI : PopupBase
    {
        bool mCanClose;
        float mShowTime;
        public void Init(string id)
        {
            mCanClose = false;

            mShowTime = 2f;

            var data = MLand.GameData.AchievementsData.TryGet(id);

            Image imgAchievementsSymbol = gameObject.FindComponent<Image>("Image_AchievementsSymbol");
            imgAchievementsSymbol.sprite = MLand.Atlas.GetUISprite(data.spriteImg);

            TextMeshProUGUI textAchievementsClear = gameObject.FindComponent<TextMeshProUGUI>("Text_AchievementsClear");
            textAchievementsClear.text = StringTableUtil.Get("UIString_AchievementsClear");

            TextMeshProUGUI textAchievementsName = gameObject.FindComponent<TextMeshProUGUI>("Text_AchievementsName");
            textAchievementsName.text = StringTableUtil.Get($"Achievements_Name_{id}");

            TextMeshProUGUI textAnyTouchClose = gameObject.FindComponent<TextMeshProUGUI>("Text_AnyTouchClose");
            textAnyTouchClose.text = StringTableUtil.Get("UIString_AnyTouchScreen");

            Button buttonClose = gameObject.FindComponent<Button>("Button_Close");
            buttonClose.SetButtonAction(() => OnClose());

            SoundPlayer.PlayAchievementsResult();
        }

        public override void OnBackButton(bool immediate = false, bool hideMotion = true)
        {
            if (mCanClose == false)
                return;

            base.OnBackButton(immediate, hideMotion);
        }

        public override void OnClose()
        {
            if (mCanClose == false)
                return;

            base.OnClose();
        }

        
        private void Update()
        {
            mShowTime -= Time.deltaTime;
            if (mShowTime <= 0f && mCanClose == false)
            {
                TextMeshProUGUI textAnyClose = gameObject.FindComponent<TextMeshProUGUI>("Text_AnyTouchClose");
                textAnyClose.DOFade(1f, 0.25f);

                mCanClose = true;
            }
        }
    }
}