using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    public class AdVideoTest_Context : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Normal;
            QualitySettings.vSyncCount = 0; //수직동기화 Off...비디오카드 불라불라

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            MLand2.Init(gameObject);
        }
    }


    class MLand2
    {
        static public void Init(GameObject go)
        {
            MLand.Atlas = new AtlasManager();
            MLand.Atlas.Init();

            MLand.Font = new FontManager();
            MLand.Font.Init();

            MLand.GameData = GameDataReader.ReadGameData("DataSheet");

            var slime = GameObject.Find("Slime");
            var slimeAnim = slime?.GetOrAddComponent<AdVideoTest_SlimeAnim>();
            slimeAnim?.Init();
        }
    }
}
