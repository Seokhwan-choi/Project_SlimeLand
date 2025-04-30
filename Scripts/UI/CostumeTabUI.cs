using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

namespace MLand
{
    class CostumeTabUI : MonoBehaviour
    {
        public virtual void Init(CostumeTabUIManager parent) {}
        public virtual void OnUpdate() { }
        public virtual void OnTabEnter() { }
        public virtual void OnTabLeave() { }
        public virtual void Localize() { }
    }

    class CostumeItemUI : MonoBehaviour
    {
        CostumeTabUIManager mParent;
        GameObject mEquippedObj;
        GameObject mEquippedMySelf;
        Image mEquippedSlime;
        Image mImageModal;

        TextMeshProUGUI mTextPiece;
        TextMeshProUGUI mTextName;
        Slider mSliderPiece;
        GameObject mCanUpgradeObj;

        string mId;

        public void Init(CostumeTabUIManager parent, string id)
        {
            mParent = parent;
            mId = id;

            mEquippedObj = gameObject.FindGameObject("Equipped");
            mEquippedMySelf = mEquippedObj.FindGameObject("MySelf");
            mEquippedSlime = mEquippedObj.FindComponent<Image>("Image_InMaskSlime");
            mImageModal = gameObject.FindComponent<Image>("Image_Modal");
            mTextName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");

            var data = MLand.GameData.CostumeData.TryGet(id);

            var imgIcon = gameObject.FindComponent<Image>("Image_Costume");
            imgIcon.sprite = MLand.Atlas.GetUISprite(data.thumbnail);

            var buttonDetail = gameObject.FindComponent<Button>("Button_ShowDetail");
            buttonDetail.SetButtonAction(() =>
            {
                var popup = MLand.PopupManager.CreatePopup<Popup_CostumeDetailUI>();

                popup.Init(mId, parent.CurSlimeId, parent.OnChangeCostume);
            });

            mSliderPiece = gameObject.FindComponent<Slider>("Slider_Piece");
            mTextPiece = gameObject.FindComponent<TextMeshProUGUI>("Text_PieceCount");
            mCanUpgradeObj = gameObject.FindGameObject("CanUpgrade");

            Refresh();
        }

        public void Refresh()
        {
            CostumeInfo info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);

            RefreshEquipped(info);
            RefreshModal(info);
            RefreshPiece(info);
            RefreshName();
        }

        public void Localize()
        {
            CostumeInfo info = MLand.SavePoint.CostumeInventory.GetCostumeInfo(mId);

            RefreshName();
            RefreshPiece(info);
        }

        void RefreshName()
        {
            mTextName.text = StringTableUtil.GetName(mId);
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

        void RefreshEquipped(CostumeInfo info)
        {
            if (info != null)
            {
                string equippedSlimeId = info.EquipedSlimeId;
                bool isEquipped = equippedSlimeId.IsValid();
                bool mySelf = mParent.CurSlimeId == equippedSlimeId;

                mEquippedObj.SetActive(isEquipped);
                mEquippedMySelf.SetActive(mySelf);
                if (isEquipped)
                    mEquippedSlime.sprite = MLand.Atlas.GetUISprite($"{equippedSlimeId}_Profile");
            }
            else
            {
                mEquippedObj.SetActive(false);
                mEquippedMySelf.SetActive(false);
            }
        }

        void RefreshModal(CostumeInfo info)
        {
            mImageModal.enabled = info != null;
        }
    }
}