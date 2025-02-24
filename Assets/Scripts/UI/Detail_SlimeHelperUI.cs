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
    //        SetNameText("슬라임 도우미지롱");
    //    }

    //    public override void OnListButtonAction()
    //    {
    //        MLand.Lobby.ShowPopup(PopupStatusType.SlimeHelper);
    //    }

    //    public override void OnUpgrade()
    //    {
    //        int level = 0;

    //        CharacterUpgradeData upgradeData = DataUtil.GetCharacterUpgradeData(mCurId, level + 1);

    //        Debug.Assert(upgradeData != null, $"{mCurId}의 {level + 1}레벨의 CharacterUpgradeData가 존재하지 않습니다.");

    //        double requireMarso = upgradeData.requireMarso;

    //        // 업그레이드
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
    //            return $"가격 : {upgradeData.requireMarso}";
    //        else
    //            return $"최고 레벨 (강화 불가)";
    //    }
    //}
}