using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class PopupStatus_CheapShopUI : PopupStatusUI
    {
        SpeechBalloonUI mSpeechBalloonUI;
        CheapShopTabUIManager mTabManager;
        Shop Shop => MLand.SavePoint.Shop;
        public override void Init(PopupStatus type)
        {
            base.Init(type);

            var speechBalloonObj = gameObject.FindGameObject("SpeechBalloon");

            mSpeechBalloonUI = speechBalloonObj.GetOrAddComponent<SpeechBalloonUI>();
            mSpeechBalloonUI.Init(StringTableUtil.GetName("Ssada"));

            mTabManager = new CheapShopTabUIManager();
            mTabManager.Init(this);
        }

        public override void OnUpdate()
        {
            if (Shop.UpdateSlimeCorePrice())
            {
                MLand.SavePoint.Save();
            }

            mTabManager.OnUpdate();
        }

        public override void OnEnter(int subIdx)
        {
            CheapShopTab tab = (CheapShopTab)subIdx;

            mTabManager.ChangeTab(tab);
        }

        public override void OnCloseButtonAction()
        {
            MLand.Lobby.HidePopupStatus();

            MLand.CameraManager.ResetFollowInfo();
        }

        public override void Localize()
        {
            mTabManager.Localize();

            string name = StringTableUtil.GetName("Ssada");
            string message = StringTableUtil.Get($"SsadaMessage_{mTabManager.CurTab}");

            mSpeechBalloonUI.Localize(name, message);
        }

        public void PlaySpeechBallonMotion(CheapShopTab tab, bool playSound)
        {
            if (playSound)
                SoundPlayer.PlayMessageShowSound(0.5f);

            var message = StringTableUtil.Get($"SsadaMessage_{tab}");

            mSpeechBalloonUI.PlayTextMotion(message);
        }

        public Button GetTabButton(CheapShopTab tab)
        {
            return mTabManager.GetTabButton(tab);
        }
    }
}