using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace MLand
{
    class BuffItemUIManager
    {
        List<Lobby_BuffItemUI> mBuffItemList;
        public void Init(LobbyUI lobby)
        {
            mBuffItemList = new List<Lobby_BuffItemUI>();

            GameObject parent = lobby.Find("Buffs");

            int index = 0;

            foreach(var data in MLand.GameData.BuffData.Values)
            {
                GameObject buffItemObj = parent.FindGameObject($"Buff_{index + 1}");

                Lobby_BuffItemUI buffItem = buffItemObj.GetOrAddComponent<Lobby_BuffItemUI>();

                buffItem.Init(this, data.buffType);

                mBuffItemList.Add(buffItem);

                index++;
            }
        }

        public void RefreshDurationTime()
        {
            var popup = MLand.PopupManager.GetPopup<Popup_BuffUI>();

            foreach(var item in mBuffItemList)
            {
                item.ApplySavePoint();

                popup?.SetDuration(item.BuffType, item.Duration);
            }
        }

        public void OnUpdate(float dt)
        {
            foreach(Lobby_BuffItemUI buffItem in mBuffItemList)
            {
                buffItem.OnUpdate(dt);
            }
        }

        public void OnActiveBuff(BuffType type)
        {
            Lobby_BuffItemUI buffItemUI = mBuffItemList.Where(x => x.BuffType == type).FirstOrDefault();

            buffItemUI?.ActiveBuff();

            MLand.Lobby.RefreshDetail();
        }

        public int GetDuration(BuffType type)
        {
            Lobby_BuffItemUI buffItemUI = mBuffItemList.Where(x => x.BuffType == type).FirstOrDefault();

            return buffItemUI?.Duration ?? 0;
        }
    }
}