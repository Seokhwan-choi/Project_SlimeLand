using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace MLand
{
    class Popup_CostumeSlimeSelectUI : PopupBase
    {
        PopupStatus_CostumeUI mParent;

        public string CurSlimeId => mParent.CurSlimeId;
        public void Init(PopupStatus_CostumeUI parent)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("UIString_SlimeSelect"));

            mParent = parent;

            var content = gameObject.FindGameObject("Content");

            content.AllChildObjectOff();

            foreach (var slimeInfo in MLand.SavePoint.SlimeManager.SlimeInfoList.Where(x => x.Id != MLand.GameData.GoldSlimeCommonData.id))
            {
                var itemObj = Util.InstantiateUI("CostumeSlimeSelectItemUI", content.transform);

                itemObj.Localize();

                var itemUI = itemObj.GetOrAddComponent<CostumeSlimeSelectItemUI>();

                itemUI.Init(this, slimeInfo);
            }
        }

        public void OnChangeSlime(string slimeId)
        {
            int slimeIndex = MLand.SavePoint.SlimeManager.GetSlimeIndex(slimeId);

            mParent.OnChangeSlime(slimeId, slimeIndex);

            this.Close();
        }
    }

    class CostumeSlimeSelectItemUI : MonoBehaviour
    {
        CharacterData mData;
        public void Init(Popup_CostumeSlimeSelectUI parent, SavedSlimeInfo slimeInfo)
        {
            mData = MLand.GameData.SlimeData.TryGet(slimeInfo.Id);

            GameObject costumeList = gameObject.FindGameObject("EquippedCostumes");

            for (int i = 0; i < (int)CostumeType.Count; ++i)
            {
                CostumeType cosType = (CostumeType)i;

                var itemObj = costumeList.FindGameObject($"Costume_{cosType}");

                EquipCostumeItemUI itemUI = itemObj.GetOrAddComponent<EquipCostumeItemUI>();

                itemUI.Init(cosType);

                string costumeId = slimeInfo.Costumes[i];

                itemUI.RefreshCostume(costumeId);
            }

            SetSlimeImage(parent);
            SetSlimeName();
            SetButtonAction(parent);
        }

        void SetSlimeName()
        {
            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            textName.text = StringTableUtil.GetName(mData.id);
        }

        void SetSlimeImage(Popup_CostumeSlimeSelectUI parent)
        {
            Image imagePotrait = gameObject.FindComponent<Image>("Image_SlimeProfile");

            imagePotrait.sprite = MLand.Atlas.GetUISprite($"{mData.id}_{EmotionType.Happy}");

            Image imageSelected = gameObject.FindComponent<Image>("Selected");

            imageSelected.enabled = parent.CurSlimeId == mData.id;
        }

        void SetButtonAction(Popup_CostumeSlimeSelectUI parent)
        {
            var buttonChange = gameObject.FindComponent<Button>("Button_ChangeSlime");
            buttonChange.SetButtonAction(() =>
            {
                parent.OnChangeSlime(mData.id);
            });
        }
    }
}