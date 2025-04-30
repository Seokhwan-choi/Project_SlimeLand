using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class Popup_CostumeResultUI : PopupBase
    {
        public void Init(CostumeInfo info, bool upgrade)
        {
            Button modalButton = gameObject.FindComponent<Button>("Button_Modal");
            modalButton.SetButtonAction(() => Close());

            // 제목
            SetTitleText(upgrade ? 
                StringTableUtil.Get("UIString_UpgradeComplete") : 
                StringTableUtil.Get("UIString_MakeComplete"));

            var data = MLand.GameData.CostumeData.TryGet(info.Id);

            // 코스튬 아이콘
            var imgIcon = gameObject.FindComponent<Image>("Image_Icon");
            imgIcon.sprite = MLand.Atlas.GetUISprite(data.thumbnail);

            // 코스튬 레벨
            var textLevel = gameObject.FindComponent<TextMeshProUGUI>("Text_Level");
            StringParam param = new StringParam("level", info.Level.ToString());
            textLevel.text = StringTableUtil.Get("UIString_Level", param);

            // 코스튬 능력치
            GameObject statObj = gameObject.FindGameObject("SlimeCoreStat");
            int level = info == null ? 1 : Mathf.Max(info.Level, 1);
            CostumeStatData statData = DataUtil.GetCostumeStatData(info.Id, level);

            bool haveDropAmount = (statData?.slimeCoreDropAmount ?? 0) > 0;
            bool haveDropCoolTime = (statData?.slimeCoreDropCoolTime ?? 0) > 0;

            statObj.SetActive(haveDropAmount || haveDropCoolTime);

            TextMeshProUGUI textSlimeCoreDropAmount = statObj.FindComponent<TextMeshProUGUI>("Text_SlimeCoreDropAmount");
            textSlimeCoreDropAmount.gameObject.SetActive(haveDropAmount);

            TextMeshProUGUI textSlimeCoreDropCoolTime = statObj.FindComponent<TextMeshProUGUI>("Text_SlimeCoreDropCoolTime");
            textSlimeCoreDropCoolTime.gameObject.SetActive(haveDropCoolTime);

            if (statData != null)
            {
                StringParam param2 = new StringParam("amount", statData.slimeCoreDropAmount.ToString());
                textSlimeCoreDropAmount.text = StringTableUtil.Get("UIString_IncreaseSlimeCoreDropAmount", param2);

                StringParam param3 = new StringParam("time", $"{statData.slimeCoreDropCoolTime:F2}");
                textSlimeCoreDropCoolTime.text = StringTableUtil.Get("UIString_DecreaseSlimeCoreDropCoolTime", param3);
            }

            SoundPlayer.PlayCentralBuildingChargeFinish();
        }
    }
}