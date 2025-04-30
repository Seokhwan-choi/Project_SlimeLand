using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

namespace MLand
{
    enum ToggleButtonType
    {
        BGM,
        Effect,
        UISet,

        Count,
    }

    class Popup_SettingUI : PopupBase
    {
        List<LanguageButton> mLanguageButtons;
        public void Init()
        {
            mLanguageButtons = new List<LanguageButton>();

            var langbuttonsParent = gameObject.FindGameObject("LangButtons");

            for(int i = 0; i < (int)LangCode.Count; ++i)
            {
                LangCode langCode = (LangCode)i;

                if (langCode == LangCode.CN || langCode == LangCode.TW)
                    continue;

                var languageButtonObj = langbuttonsParent.FindGameObject($"{langCode}");

                LanguageButton button = languageButtonObj.GetOrAddComponent<LanguageButton>();

                button.Init(this, langCode);

                mLanguageButtons.Add(button);
            }

            SetUpCloseAction();

            SetTitleText(StringTableUtil.Get("Title_Setting"));

            for(int i = 0; i < (int)ToggleButtonType.Count; ++i)
            {
                ToggleButtonType type = (ToggleButtonType)i;

                GameObject soundButtonObj = gameObject.FindGameObject($"{type}");

                ToggleButtonItem soundButtonItem = soundButtonObj.GetOrAddComponent<ToggleButtonItem>();

                soundButtonItem.Init(type.ChangeToSavePointFlag());
            }

            Button buttonSave = gameObject.FindComponent<Button>("Button_Save");
            buttonSave.SetButtonAction(OnSaveButtonAction);

            Button buttonLoad = gameObject.FindComponent<Button>("Button_Load");
            buttonLoad.SetButtonAction(OnLoadButtonAction);

            TextMeshProUGUI textVersion = gameObject.FindComponent<TextMeshProUGUI>("Text_Version");
            textVersion.text = $"Ver {Application.version}";
        }

        public void OnLanguageButtonAction()
        {
            Localize();

            SetTitleText(StringTableUtil.Get("Title_Setting"));

            foreach (var langButton in mLanguageButtons)
            {
                langButton.RefreshButtonImg();
            }
        }

        void OnSaveButtonAction()
        {
            string savedStr = MLand.SavePoint.ChangeToSavedStr();

            MLand.GameManager.StartTouchBlock(10f, showLoadingImg:true);

            MLand.GPGSBinder.SaveCloud("SavePoint", savedStr, OnCloudSaved);

            void OnCloudSaved(bool success)
            {
                if(success)
                {
                    // 저장 성공!
                    string message = StringTableUtil.GetSystemMessage("SuccessCloudSave");
                    MonsterLandUtil.ShowSystemMessage(message);
                }
                else
                {
                    // 저장 실패
                    string message = StringTableUtil.GetSystemMessage("FailedCloudSave");
                    MonsterLandUtil.ShowSystemMessage(message);

                    SoundPlayer.PlayErrorSound();
                }

                MLand.GameManager.EndTouchBlock();
            }
        }

        void OnLoadButtonAction()
        {
            string title = StringTableUtil.Get("Title_CloudLoad");
            string desc = StringTableUtil.GetDesc("ConfirmCloudLoad");

            MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);

            void OnConfirm()
            {
                MLand.GameManager.StartTouchBlock(10f, showLoadingImg: true);

                MLand.GPGSBinder.LoadCloud("SavePoint", OnCloudLoaded);

                void OnCloudLoaded(bool success, string loadData)
                {
                    if (success)
                    {
                        var message = StringTableUtil.GetSystemMessage("SuccessCloudLoad");

                        MonsterLandUtil.ShowSystemMessage(message);

                        MLand.SavePoint = JsonUtil.Base64FromJsonUnity<SavePoint>(loadData);
                        MLand.SavePoint.Normalize();
                        MLand.SavePoint.Save();

                        StartCoroutine(ReloadingScene());
                    }
                    else
                    {
                        var message = StringTableUtil.GetSystemMessage("FailedCloudLoad");

                        MonsterLandUtil.ShowSystemMessage(message);

                        SoundPlayer.PlayErrorSound();

                        MLand.GameManager.EndTouchBlock();
                    }

                    
                }
            }
        }

        IEnumerator ReloadingScene()
        {
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene("Main");

            MLand.GameManager.EndTouchBlock();
        }
    }

    class ToggleButtonItem : MonoBehaviour
    {
        public void Init(SavePointBitFlags flag)
        {
            var buttonToggle = gameObject.FindComponent<Button>("Button_Toggle");

            Image imgCheck = gameObject.FindComponent<Image>("Image_Check");

            imgCheck.gameObject.SetActive(flag.IsOn());

            buttonToggle.SetButtonAction(() =>
            {
                flag.Toggle();

                imgCheck.gameObject.SetActive(flag.IsOn());

                if (flag == SavePointBitFlags.OnBGMSound)
                {
                    MLand.SoundManager.SetBGSoundActive(flag.IsOn());
                }
                else if(flag == SavePointBitFlags.OnEffectSound)
                {
                    MLand.SoundManager.SetEffectSoundActive(flag.IsOn());
                }
            });
        }
    }

    static class SoundButtonTypeUtil
    {
        public static SavePointBitFlags ChangeToSavePointFlag(this ToggleButtonType type)
        {
            if (type == ToggleButtonType.BGM)
                return SavePointBitFlags.OnBGMSound;
            else if (type == ToggleButtonType.Effect)
                return SavePointBitFlags.OnEffectSound;
            else
                return SavePointBitFlags.ShowSlimeCoreGetAmount;
        }
    }

    class LanguageButton : MonoBehaviour
    {
        LangCode mLangCode;
        Image mImgButton;
        public void Init(Popup_SettingUI parent, LangCode langCode)
        {
            mLangCode = langCode;

            var button = gameObject.FindComponent<Button>("Button_Toggle");
            button.SetButtonAction(() => OnButtonAction(parent));

            mImgButton = button.GetComponent<Image>();

            RefreshButtonImg();
        }

        public void RefreshButtonImg()
        {
            bool active = MLand.SavePoint.LangCode == mLangCode;

            string btnName = active ? "Btn_Square_Blue" : "Btn_Square_Yellow";

            mImgButton.sprite = MLand.Atlas.GetUISprite(btnName);
        }

        void OnButtonAction(Popup_SettingUI parent)
        {
            MLand.SavePoint.LangCode = mLangCode;
            MLand.SavePoint.Save();

            parent.OnLanguageButtonAction();

            MLand.Lobby.Localize();
        }
    }
}