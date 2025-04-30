using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    //항상 실행될 클래스
    public class Context : MonoBehaviour
    {        
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Normal;
            QualitySettings.vSyncCount = 0; //수직동기화 Off...비디오카드 불라불라

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            MLand.Init(gameObject);
        }
    }


    class MLand
    {
        static public AdManager AdManager;
        static public GPGSBinder GPGSBinder;
        static public FirebaseSetup Firebase;
        static public ObjectPool ObjectPool;
        static public AtlasManager Atlas;
        static public FontManager Font;
        static public IAPManager IAPManager;
        static public GameData GameData;
        static public SavePoint SavePoint;
        static public PopupManager PopupManager;
        static public CameraManager CameraManager;
        static public SoundManager SoundManager;
        static public ParticleManager ParticleManager;
        static public GameManager GameManager;
        static public LobbyUI Lobby;
        
        static public void Init(GameObject go)
        {
            AdManager = new AdManager();
            AdManager.Init();

            Firebase = new FirebaseSetup();

            Atlas = new AtlasManager();

            Font = new FontManager();
            Font.Init();

            ObjectPool = new ObjectPool();

            GameData = GameDataReader.ReadGameData("DataSheet");

            IAPManager = new IAPManager();
            
            SavePoint = SavePointUtil.Load();
            SavePoint.Normalize();
            SavePoint.Save();

            SoundManager = go.AddComponent<SoundManager>();
            SoundManager.Init();

            PopupManager = new PopupManager();

            ParticleManager = go.AddComponent<ParticleManager>();
            ParticleManager.Init();

            CameraManager = go.AddComponent<CameraManager>();
            CameraManager.Init();

            GameManager = go.AddComponent<GameManager>();
            Lobby = go.GetComponentInParent<LobbyUI>();
        }
    }
}