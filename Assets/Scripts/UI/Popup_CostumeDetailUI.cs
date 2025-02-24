using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace MLand
{
    class Popup_CostumeDetailUI : PopupBase
    {
        string mId;
        string mSlimeId;
        bool mInMotion;
        Action mOnChange;

        GameObject mEquipObj;
        GameObject mMakeObj;
        Button mButtonEquip;
        Button mButtonUnEquip;
        Button mButtonUpgrade;
        Button mButtonMake;

        TextMeshProUGUI mTextPiece;
        TextMeshProUGUI mTextLevel;
        Slider mSliderPiece;
        GameObject mCanUpgradeObj;
        public void Init(string id)
        {
            SetUpCloseAction();

            mId = id;

            var data = MLand.GameData.CostumeData.TryGet(id);

            var imgIcon = gameObject.FindComponent<Image>("Image_Icon");
            imgIcon.sprite = MLand.Atlas.GetUISprite(data.thumbnail);
            TextMeshProUGUI textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");
            textDesc.text = StringTableUtil.GetDesc(id);
            SetTitleText(StringTableUtil.GetName(id));

            GameObject statObj = gameObject.FindGameObject("SlimeCoreStat");
            CostumeInfo info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(id);
            int level = info == null ? 1 : Mathf.Max(info.Level, 1);
            CostumeStatData statData = DataUtil.GetCostumeStatData(id, level);

            bool haveDropAmount = (statData?.slimeCoreDropAmount ?? 0) > 0;
            bool haveDropCoolTime = (statData?.slimeCoreDropCoolTime ?? 0) > 0;

            statObj.SetActive(haveDropAmount || haveDropCoolTime);

            TextMeshProUGUI textSlimeCoreDropAmount = statObj.FindComponent<TextMeshProUGUI>("Text_SlimeCoreDropAmount");
            textSlimeCoreDropAmount.gameObject.SetActive(haveDropAmount);

            TextMeshProUGUI textSlimeCoreDropCoolTime = statObj.FindComponent<TextMeshProUGUI>("Text_SlimeCoreDropCoolTime");
            textSlimeCoreDropCoolTime.gameObject.SetActive(haveDropCoolTime);

            if (statData != null)
            {
                StringParam param = new StringParam("amount", statData.slimeCoreDropAmount.ToString());
                textSlimeCoreDropAmount.text = StringTableUtil.Get("UIString_IncreaseSlimeCoreDropAmount", param);

                StringParam param2 = new StringParam("time", $"{statData.slimeCoreDropCoolTime:F2}");
                textSlimeCoreDropCoolTime.text = StringTableUtil.Get("UIString_DecreaseSlimeCoreDropCoolTime", param2);
            }

            mInMotion = false;
        }

        public void Init(string id, string slimeId, Action onChange)
        {
            Init(id);

            mSlimeId = slimeId;
            mOnChange = onChange;

            var info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);

            mTextLevel = gameObject.FindComponent<TextMeshProUGUI>("Text_Level");

            mEquipObj = gameObject.FindGameObject("Equip");
            mMakeObj = gameObject.FindGameObject("Make");

            mButtonEquip = mEquipObj.FindComponent<Button>("Button_Equip");
            mButtonEquip.SetButtonAction(OnEquip);

            var textEquip = mEquipObj.FindComponent<TextMeshProUGUI>("Text_Equip");
            textEquip.text = StringTableUtil.Get("UIString_Equip");

            mButtonUnEquip = mEquipObj.FindComponent<Button>("Button_UnEquip");
            mButtonUnEquip.SetButtonAction(OnUnEquip);

            var textUnEquip = mEquipObj.FindComponent<TextMeshProUGUI>("Text_UnEquip");
            textUnEquip.text = StringTableUtil.Get("UIString_UnEquip");

            mButtonUpgrade = mEquipObj.FindComponent<Button>("Button_Upgrade");
            mButtonUpgrade.SetButtonAction(() => OnUpgrade(upgrade:true));

            var textUpgrade = mEquipObj.FindComponent<TextMeshProUGUI>("Text_Upgrade");
            textUpgrade.text = StringTableUtil.Get("UIString_Upgrade");

            mButtonMake = mMakeObj.FindComponent<Button>("Button_Make");
            mButtonMake.SetButtonAction(() => OnUpgrade(upgrade:false));

            var textMake = mMakeObj.FindComponent<TextMeshProUGUI>("Text_Make");
            textMake.text = StringTableUtil.Get("UIString_Make");

            mSliderPiece = gameObject.FindComponent<Slider>("Slider_Piece");
            mTextPiece = gameObject.FindComponent<TextMeshProUGUI>("Text_PieceCount");
            mCanUpgradeObj = gameObject.FindGameObject("CanUpgrade");

            Refresh();
        }

        public override void Close(bool immediate = false, bool hideMotion = true)
        {
            if (mInMotion)
                return;

            base.Close(immediate, hideMotion);
        }

        void OnEquip()
        {
            var costumeInfo = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);
            if ( costumeInfo == null )
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            SavedSlimeInfo slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mSlimeId);
            if ( slimeInfo == null )
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            // 착용하려는 코스튬을 장착하고 있는 슬라임이 있는지 확인한다.
            Slime equippedSlime = null;
            string equippedSlimeId = costumeInfo.EquipedSlimeId;
            if (equippedSlimeId.IsValid())
            {
                var equippedSlimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(equippedSlimeId);
                if (equippedSlimeInfo == null)
                    return ;

                equippedSlime = MLand.GameManager.GetSlime(equippedSlimeId);
                if (equippedSlime == null)
                    return;
            }

            MLand.SavePoint.EquipCostume(mId, mSlimeId);

            equippedSlime?.RefreshCostumes();

            mOnChange?.Invoke();

            Close();
        }

        void OnUnEquip()
        {
            var costumeInfo = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);
            if (costumeInfo == null)
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            var slimeInfo = MLand.SavePoint.SlimeManager.GetSlimeInfo(mSlimeId);
            if (slimeInfo == null)
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            MLand.SavePoint.UnEquipCostume(mId, mSlimeId);

            mOnChange?.Invoke();

            Close();
        }

        void OnUpgrade(bool upgrade)
        {
            var info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);
            if (info == null)
            {
                MonsterLandUtil.ShowSystemErrorMessage("HaveNotCosutme");

                return;
            }

            if (info.IsMaxLevel())
            {
                MonsterLandUtil.ShowSystemErrorMessage("IsMaxLevelCostume");

                return;
            }

            if (info.IsEnoughPieceCurrentUpgrade() == false)
            {
                if (upgrade)
                    MonsterLandUtil.ShowSystemErrorMessage("IsNotEnoughCostumePieceByUpgrade");
                else
                    MonsterLandUtil.ShowSystemErrorMessage("IsNotEnoughCostumePieceByMake");

                return;
            }

            if ( info.Upgrade() == false )
            {
                MonsterLandUtil.ShowSystemDefaultErrorMessage();

                return;
            }

            mOnChange?.Invoke();

            StartCoroutine(StartUpgradeMotion(info, upgrade));
        }

        void Refresh()
        {
            CostumeInfo info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);

            RefreshLevel(info);
            RefreshPiece(info);
            RefreshButtons(info);
        }

        void RefreshLevel(CostumeInfo info)
        {
            int level = info == null ? 0 : info.Level;

            StringParam param = new StringParam("level", level.ToString());

            mTextLevel.text = StringTableUtil.Get("UIString_Level", param);
        }

        void RefreshPiece(CostumeInfo info)
        {
            if (info != null)
            {
                if (info.IsMaxLevel() == false)
                {
                    int currentPiece = info.Piece;
                    int requirePiece = DataUtil.GetCostumeUpgradeRequirePiece(info.Level);
                    bool canUpgrade = info.CanUpgrade();

                    mSliderPiece.value = (float)currentPiece / (float)requirePiece;
                    mTextPiece.text = $"{currentPiece} / {requirePiece}";
                    mCanUpgradeObj.SetActive(canUpgrade);
                }
                else
                {
                    mTextPiece.text = $"{StringTableUtil.Get("UIString_MaxLevel")}";
                    mCanUpgradeObj.SetActive(true);
                }
            }
            else
            {
                int requirePiece = DataUtil.GetCostumeUpgradeRequirePiece(0);

                mSliderPiece.value = 0f;
                mTextPiece.text = $"{0} / {requirePiece}";
                mCanUpgradeObj.SetActive(false);
            }
        }

        void RefreshButtons(CostumeInfo info)
        {
            if (info != null)
            {
                mEquipObj.SetActive(true);
                mMakeObj.SetActive(false);

                if (info.Level != 0)
                {
                    // 현재 슬라임이 착용중이면 해제하기 버튼 보여주고
                    string equipSlimeId = info.EquipedSlimeId;

                    if (equipSlimeId == mSlimeId)
                    {
                        mButtonEquip.gameObject.SetActive(false);
                        mButtonUnEquip.gameObject.SetActive(true);
                    }
                    else
                    {
                        // 현재 슬라임이 안끼고 있으면 착용하기 버튼 보여주자
                        mButtonEquip.gameObject.SetActive(true);
                        mButtonUnEquip.gameObject.SetActive(false);

                        var equippedObj = gameObject.FindGameObject("Equipped");

                        // 누군가 끼고 있다면 누가 끼고 있는지 보여주자
                        if (equipSlimeId.IsValid())
                        {
                            equippedObj.SetActive(true);

                            Image imgSlime = equippedObj.FindComponent<Image>("Image_InMaskSlime");
                            imgSlime.sprite = MLand.Atlas.GetUISprite($"{equipSlimeId}_Profile");

                            string slimeName = StringTableUtil.GetName(equipSlimeId);
                            string equipped = StringTableUtil.Get("UIString_Equipped");

                            // 착용중 글자 보여주자
                            TextMeshProUGUI textEquipped = equippedObj.FindComponent<TextMeshProUGUI>("Text_Equipped");
                            textEquipped.text = $"{slimeName}\n{equipped}";
                        }
                        else
                        {
                            equippedObj.SetActive(false);
                        }
                    }

                    // 레벨업이 가능한지 확인
                    bool canUpgrade = info.CanUpgrade();

                    string btnName = canUpgrade ? "Btn_Square_Green" : "Btn_Square_LightGray";

                    var buttonImg = mButtonUpgrade.GetComponent<Image>();
                    buttonImg.sprite = MLand.Atlas.GetUISprite(btnName);
                }
                else
                {
                    RefreshMakeButton();
                }
            }
            else
            {
                RefreshMakeButton();
            }

            void RefreshMakeButton()
            {
                mEquipObj.SetActive(false);
                mMakeObj.SetActive(true);

                // 제작 가능한 상태인지 확인하자.
                bool canUpgrade = info?.CanUpgrade() ?? false;

                string btnName = canUpgrade ? "Btn_Square_Green" : "Btn_Square_LightGray";

                var buttonImg = mButtonMake.GetComponent<Image>();
                buttonImg.sprite = MLand.Atlas.GetUISprite(btnName);
            }
        }

        IEnumerator StartUpgradeMotion(CostumeInfo info, bool upgrade)
        {
            MLand.GameManager.StartTouchBlock(5f);

            mInMotion = true;

            CanvasGroup hide = gameObject.FindComponent<CanvasGroup>("Motion_Hide");
            CanvasGroup hide2 = gameObject.FindComponent<CanvasGroup>("Motion_Hide2");

            DOTween.To(() => hide.alpha, (f) => hide.alpha = f, 0f, 0.5f);
            DOTween.To(() => hide2.alpha, (f) => hide2.alpha = f, 0f, 0.5f);

            // 화면 어둡게
            Image modal = gameObject.FindComponent<Image>("Motion_Modal");
            modal.enabled = true;
            modal.DOFade(167f / 255f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            SoundPlayer.PlayCentralBuildingChargeStart();

            // 아이콘 하얗게 화이트아웃
            Image whiteOut = gameObject.FindComponent<Image>("Motion_WhiteOut");
            whiteOut.enabled = true;
            whiteOut.DOFade(1f, 1f);

            var starVortexFX = gameObject.FindComponent<ParticleSystem>("StarVortex");
            starVortexFX.gameObject.SetActive(true);
            starVortexFX.Play();

            yield return new WaitForSeconds(2f);

            mInMotion = false;

            Close(immediate:true);

            MLand.GameManager.EndTouchBlock();

            Popup_CostumeResultUI popup = MLand.PopupManager.CreatePopup<Popup_CostumeResultUI>();

            popup.Init(info, upgrade);
        }
    }
}


