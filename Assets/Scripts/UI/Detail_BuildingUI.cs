using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MLand
{
    class Detail_BuildingUI : DetailUI
    {

        void SetSlimeCoreDropAmount(BuildingStatData data)
        {
            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.IncreaseDropAmount);

            double orgValue = data.slimeCoreDropAmount;
            string bonusStr = buffValue > 1 ? ((orgValue * buffValue) - orgValue).ToAlphaString() : string.Empty;

            SetSlimeCoreAmountText(orgValue.ToAlphaString(), bonusStr);
        }

        void SetSlimeCoreDropCoolTime(BuildingStatData data)
        {
            float buffValue = MLand.SavePoint.GetBuffValue(BuffType.DecreaseDropCoolTime);

            float orgValue = data.slimeCoreDropCoolTime;
            string bonusStr = buffValue > 1 ? $"{orgValue / buffValue:F}" : string.Empty;

            SetSlimeCoreCoolTimeText($"{orgValue.ToString()}s", bonusStr);
        }

        public override void Refresh()
        {
            if (mCurId.IsValid() == false)
                return;

            BuildingStatData statData = DataUtil.GetBuildingStatData(mCurId);
            BuildingData buildingData = MLand.GameData.BuildingData.TryGet(mCurId);

            // �̸� ����
            SetNameText(StringTableUtil.GetBuildingName(mCurId, statData.level));

            // �ʻ�ȭ �̹���
            SetPotraitImg(MLand.Atlas.GetBuildingUISprite(statData.spriteImg));

            // �Ӽ�
            SetSlimeCoreElementalTypeText(buildingData.elementalType);
            SetEmblemImg(buildingData.elementalType);

            // ������ �� ���귮 / ����ӵ�
            SetSlimeCore1Img(buildingData.elementalType);
            SetSlimeCore2Img(buildingData.elementalType);

            SetSlimeCoreDropAmount(statData);
            SetSlimeCoreDropCoolTime(statData);
        }

        public override void OnListButtonAction()
        {
            string id = mCurId;

            base.OnListButtonAction();

            MLand.Lobby.ShowPopup(PopupStatus.SlimeKing, subIdx: (int)SlimeKingTab.Building);

            //  ��ũ�� �� �ڿ������� �̵� ���� ����
            MLand.Lobby.PopupStatusManager.GetPopup<PopupStatus_SlimeKingUI>().MoveToTab(id);
        }
    }
}