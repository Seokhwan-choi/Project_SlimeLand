using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    //class Detail_SlimeHelperUI : DetailUI
    //{
    //    public override void OnShow(string id)
    //    {
    //        base.OnShow(id);

    //        SetVisibleListButton(true);
    //        SetNameText("������ ���������");
    //    }

    //    public override void OnListButtonAction()
    //    {
    //        MLand.Lobby.ShowPopup(PopupStatusType.SlimeHelper);
    //    }

    //    public override void OnUpgrade()
    //    {
    //        int level = 0;

    //        CharacterUpgradeData upgradeData = DataUtil.GetCharacterUpgradeData(mCurId, level + 1);

    //        Debug.Assert(upgradeData != null, $"{mCurId}�� {level + 1}������ CharacterUpgradeData�� �������� �ʽ��ϴ�.");

    //        double requireMarso = upgradeData.requireMarso;

    //        // ���׷��̵�
    //        Upgrade(level, requireMarso);
    //    }

    //    void Upgrade(int level, double requireMarso)
    //    {
    //        if (MLand.Lobby.UseGold(requireMarso))
    //        {
    //            MLand.GameManager.UpgradeSlime(mCurId);

    //            SetUpgradePriceText(GetUpgradePriceText());
    //            SetLevelText(level + 1);
    //        }
    //    }

    //    public override string GetUpgradePriceText()
    //    {
    //        int level = 0;

    //        CharacterUpgradeData upgradeData = DataUtil.GetCharacterUpgradeData(mCurId, level + 1);
    //        if (upgradeData != null)
    //            return $"���� : {upgradeData.requireMarso}";
    //        else
    //            return $"�ְ� ���� (��ȭ �Ұ�)";
    //    }
    //}
}