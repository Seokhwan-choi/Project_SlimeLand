using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace MLand
{
    enum NavBarType
    {
        Slime,
        Building,
        Costume,
        Shop,

        Count,
    }

    class LobbyNavBarUI
    {
        LobbyUI mParent;
        public void Init(LobbyUI parent)
        {
            mParent = parent;

            GameObject navBar = parent.Find("NavBar");

            for (int i = 0; i < (int)NavBarType.Count; ++i)
            {
                NavBarType type = (NavBarType)i;

                string name = $"{type}";

                Button buttonNav = navBar.FindComponent<Button>(name);

                buttonNav.SetButtonAction(() => OnNavBarButton(name));
            }
        }

        void OnNavBarButton(string navBar)
        {
            if (Enum.TryParse(navBar, out NavBarType type))
            {
                switch (type)
                {
                    case NavBarType.Slime:
                        mParent.ShowPopup(PopupStatus.SlimeKing, (int)SlimeKingTab.Slime);
                        break;
                    case NavBarType.Building:
                        mParent.ShowPopup(PopupStatus.SlimeKing, (int)SlimeKingTab.Building);
                        break;
                    case NavBarType.Shop:
                        mParent.ShowPopup(PopupStatus.CheapShop);
                        break;
                    case NavBarType.Costume:
                        mParent.ShowPopup(PopupStatus.Costume);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}