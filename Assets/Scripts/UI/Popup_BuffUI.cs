using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using UnityEngine.UI;

namespace MLand
{
    class Popup_BuffUI : PopupBase
    {
        Dictionary<BuffType, Popup_BuffItemUI> mItemDics;
        public void Init(BuffItemUIManager parent)
        {
            SetUpCloseAction();
            SetTitleText(StringTableUtil.Get("Title_Buff"));

            var textWarning = gameObject.FindComponent<TextMeshProUGUI>("Text_Warning");
            textWarning.text = StringTableUtil.GetDesc("BuffWarning");

            mItemDics = new Dictionary<BuffType, Popup_BuffItemUI>();

            int index = 0;

            foreach (var data in MLand.GameData.BuffData.Values)
            {
                GameObject buffItemObj = gameObject.FindGameObject($"Buff_{index + 1}");

                Popup_BuffItemUI buffItem = buffItemObj.GetOrAddComponent<Popup_BuffItemUI>();

                buffItem.Init(parent, data.buffType);

                mItemDics.Add(data.buffType, buffItem);

                index++;
            }
        }

        public void SetDuration(BuffType buffType, int duration)
        {
            var item = mItemDics.TryGet(buffType);

            item?.SetDuration(duration);
        }
    }

    class Popup_BuffItemUI : MonoBehaviour
    {
        float mInterval;
        int mDuration;

        BuffData mData;
        GameObject mActiveObj;
        GameObject mApplyObj;
        GameObject mInActiveObj;
        TextMeshProUGUI mTextApplyTime;
        TextMeshProUGUI mTextActiveUpdate;
        TextMeshProUGUI mTextInActiveUpdate;
        BuffItemUIManager mParent;
        bool IsActive => mDuration > 0;
        public void Init(BuffItemUIManager parent, BuffType type)
        {
            mParent = parent;
            mData = MLand.GameData.BuffData.TryGet(type);

            string buffName = $"Buff_{mData.buffType}";

            Image imgBuffIcon = gameObject.FindComponent<Image>("Image_BuffIcon");
            imgBuffIcon.sprite = MLand.Atlas.GetUISprite(buffName);

            TextMeshProUGUI textName = gameObject.FindComponent<TextMeshProUGUI>("Text_Name");
            textName.text = StringTableUtil.GetName(buffName);

            TextMeshProUGUI textDesc = gameObject.FindComponent<TextMeshProUGUI>("Text_Desc");

            StringParam descParam = new StringParam("time", mData.duration.ToString());
            string desc = StringTableUtil.GetDesc($"Popup_{buffName}", descParam);

            textDesc.text = desc;

            var buttonActive = gameObject.FindComponent<Button>("Button_Active");
            var buttonInActive = gameObject.FindComponent<Button>("Button_InActive");
            var buttonApplying = gameObject.FindComponent<Button>("Button_Applying");

            buttonActive.SetButtonAction(OnActiveBuff);
            buttonInActive.SetButtonAction(OnActiveBuff);
            buttonApplying.SetButtonAction(OnActiveBuff);

            mActiveObj = buttonActive.gameObject;
            mInActiveObj = buttonInActive.gameObject;
            mApplyObj = buttonApplying.gameObject;

            mTextActiveUpdate = mActiveObj.FindComponent<TextMeshProUGUI>("Text_Update");
            mTextInActiveUpdate = mInActiveObj.FindComponent<TextMeshProUGUI>("Text_Update");
            mTextApplyTime = mApplyObj.FindComponent<TextMeshProUGUI>("Text_ApplyingTime");

            var textApply = mApplyObj.FindComponent<TextMeshProUGUI>("Text_Applying");
            textApply.text = StringTableUtil.Get("UIString_Applying");

            SetUpdateText();
            SetDuration(parent.GetDuration(type));
        }

        void SetUpdateText()
        {
            var buffInfo = MLand.SavePoint.BuffManager.GetBuffInfo(mData.buffType);

            int maxCount = mData.maxDailyCount;
            int remainCount = maxCount - (buffInfo.DailyCounter?.StackedCount ?? maxCount);

            string watchAdAndApplyStr = StringTableUtil.Get("UIString_WatchAdAndApply");
            string countStr = $"({remainCount}/{maxCount})";
            string text = $"{watchAdAndApplyStr} {countStr}";

            mTextActiveUpdate.text = text;
            mTextInActiveUpdate.text = text;
        }

        void SetApplyTimeText()
        {
            int duration = Mathf.RoundToInt(mDuration);

            var result = CalcMiunuteAndSeconds(duration);

            StringParam param = new StringParam("minute", result.minute.ToString());
            param.AddParam("seconds", result.seconds.ToString());

            mTextApplyTime.text = StringTableUtil.Get("UIString_MinuteAndSeconds", param);

            (int minute, int seconds) CalcMiunuteAndSeconds(int totalSeconds)
            {
                int minute = totalSeconds / TimeUtil.SecondsInMinute;
                totalSeconds -= minute * TimeUtil.SecondsInMinute;

                int seconds = totalSeconds;

                return (minute, seconds);
            }
        }


        void OnActiveBuff()
        {
            // 버프가 적용중인지 확인
            if (IsActive)
            {
                MonsterLandUtil.ShowSystemErrorMessage("BuffAlreadyActive");

                return;
            }

            // 버프 하루 사용 횟수 남았는지 확인
            BuffInfo buffInfo = MLand.SavePoint.BuffManager.GetBuffInfo(mData.buffType);
            if (buffInfo.IsEnoughDailyCounter() == false)
            {
                MonsterLandUtil.ShowSystemErrorMessage("NotEnoughBuffCount");

                return;
            }

            var removeAdProduct = CodelessIAPStoreListener.Instance.GetProduct(MLand.GameData.ShopCommonData.removeAdProductId);
            if (removeAdProduct != null)
            {
                if (removeAdProduct.definition.type == ProductType.NonConsumable && removeAdProduct.hasReceipt)
                {
                    ActiveBuff();
                }
                else
                {
                    ConfirmWatchAd();
                }
            }
            else
            {
                ConfirmWatchAd();
            }

            void ConfirmWatchAd()
            {
                string buffName = $"Buff_{mData.buffType}";

                var name = StringTableUtil.GetName(buffName);
                var descParam = new StringParam("time", mData.duration.ToString());
                var descMessage = StringTableUtil.GetDesc(buffName, descParam);

                MonsterLandUtil.ShowAdConfirmPopup(name, descMessage, ActiveBuff);
            }
        }

        void ActiveBuff()
        {
            SoundPlayer.PlayBuffActive();

            MLand.SavePoint.ActiveBuff(mData.buffType, mData.duration);

            MLand.SavePoint.Save();

            string message = StringTableUtil.GetSystemMessage($"BuffActive_{mData.buffType}");

            MonsterLandUtil.ShowSystemMessage(message);

            mParent.OnActiveBuff(mData.buffType);

            SetUpdateText();
            SetDuration(mData.duration);
        }

        void RefreshActive()
        {
            BuffInfo buff = MLand.SavePoint.BuffManager.GetBuffInfo(mData.buffType);

            // 오늘 사용가능한 최대 횟수를 모두 사용했으면 처리
            bool isMaxCount = buff.DailyCounter.IsMaxCount(mData.maxDailyCount);

            mActiveObj.SetActive(!isMaxCount&&!IsActive);
            mInActiveObj.SetActive(isMaxCount);
            mApplyObj.SetActive(IsActive);
        }

        public void SetDuration(int duration)
        {
            mDuration = duration;

            SetApplyTimeText();

            RefreshActive();
        }
    }
}