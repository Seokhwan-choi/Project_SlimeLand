using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Linq;
using CodeStage.AntiCheat.Detectors;

namespace MLand
{
    class GameManager : MonoBehaviour
    {
        // 물, 풀, 불, 땅 순
        readonly Vector2[] SlimeSpawnPos =
            new Vector2[4] {
                new Vector2(-6.5f, 10f), new Vector2(6f, 9.8f),
                new Vector2(6.85f, -7.65f), new Vector2(-5.55f, -7.9f) };

        int mLastUpdateTime;
        float mOfflineRewardIntervalTime;
        float mRandomizeIntervalTime;

        bool mIsPlay;

        TouchBlock mTouchBlock;
        SlimeKing mSlimeKing;
        CheapShopClicker mCheapShop;
        MiniGameClicker mMiniGame;
        Transform mUIMotionParent;
        Transform mUIMotionParent2;
        Transform mSlimeFieldTm;
        LuckySymbol mLuckySymbol;

        BuildingManager mBuildingManager;               // 건물 관리
        CharacterManager mCharacterManager;             // 슬라임 관리
        GoldSlimeSpawnManager mGoldSlimeSpawnManager;   // 골드 슬라임 소환 관리
        ExpensiveShopManager mExpensiveShopManager;
        ReviewManager mReviewManager;

        public bool IsPlay => mIsPlay;
        public Transform UIMotionParent => mUIMotionParent;
        public Transform UIMotionParent2 => mUIMotionParent2;
        public MiniGameClicker MiniGame => mMiniGame;
        public CheapShopClicker CheapShop => mCheapShop;

        public IEnumerator Init()
        {
            yield return null;

            mOfflineRewardIntervalTime = MLand.GameData.OfflineRewardCommonData.intervalTimeSeconds;

            InitGameObjects();

            yield return null;

            mBuildingManager = new BuildingManager();
            mBuildingManager.Init();

            yield return null;
            yield return null;
            yield return null;

            mCharacterManager = new CharacterManager();
            mCharacterManager.Init(mSlimeFieldTm);

            yield return null;

            mGoldSlimeSpawnManager = new GoldSlimeSpawnManager();
            mGoldSlimeSpawnManager.Init(mBuildingManager, mCharacterManager);

            yield return null;

            mExpensiveShopManager = new ExpensiveShopManager();
            mExpensiveShopManager.Init(mBuildingManager);

            mReviewManager = new ReviewManager();

            yield return null;

            mBuildingManager.RefreshExpandField();

            yield return null;

            mBuildingManager.BuildNavMesh();

            SyncServerTime();
        }

        public void SetPlay(bool play)
        {
            mIsPlay = play;
        }

        void InitGameObjects()
        {
            mUIMotionParent = GameObject.Find("UIMotions").transform;
            mUIMotionParent2 = GameObject.Find("UIMotions2").transform;
            GameObject touchBlockObj = GameObject.Find("TouchBlock");
            mTouchBlock = touchBlockObj.GetOrAddComponent<TouchBlock>();
            mTouchBlock.Init();
            GameObject slimeField = GameObject.Find("SlimeField");
            mSlimeFieldTm = slimeField.transform;

            var slimeKingObj = GameObject.Find("SlimeKing");
            mSlimeKing = slimeKingObj.GetOrAddComponent<SlimeKing>();
            mSlimeKing.Init();

            var cheapShopObj = GameObject.Find("CheapShop");
            mCheapShop = cheapShopObj.GetOrAddComponent<CheapShopClicker>();

            var miniGameObj = GameObject.Find("MiniGame");
            mMiniGame = miniGameObj.GetOrAddComponent<MiniGameClicker>();

            var luckySymbolObj = slimeField.FindGameObject("LuckySymbol");
            mLuckySymbol = luckySymbolObj.GetOrAddComponent<LuckySymbol>();
            mLuckySymbol.Init();
        }


        private void Update()
        {
            if (mIsPlay == false)
                return;

            // 뒤로가기 버튼을 눌렀을 때 동작
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (MLand.Lobby.InTutorial())
                    return;

                OnBackButton();
            }
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                Time.timeScale = 10f;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                SavePointBitFlags.ShowSlimeCoreGetAmount.Toggle();
            }

            if ( Input.GetKeyDown(KeyCode.B) )
            {
                CodelessIAPStoreListener.Instance.InitiatePurchase(MLand.GameData.ShopCommonData.removeAdProductId);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                MLand.Lobby.ShowUI();
                MLand.CameraManager.SetTestMode(false);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                int langCodeIdx = (int)MLand.SavePoint.LangCode;

                langCodeIdx = (langCodeIdx + 1) % (int)LangCode.Count;

                MLand.SavePoint.LangCode = (LangCode)langCodeIdx;
                MLand.SavePoint.Save();

                MLand.Lobby.Localize();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                MLand.CameraManager.PlayTestZoomOutRoutine(0.5f);

                MLand.Lobby.HideUI();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                MLand.Lobby.HideUI();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                MLand.GameManager.AddGem(500000);
                MLand.GameManager.AddGold(5000000000);
            }

            if (Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(KeyCode.DownArrow) )
            {
                MLand.CameraManager.TestMoveByKeyBoard();
            }
#else
            if (Time.timeScale > 1f)
            {
                Time.timeScale = 1f;
            }
#endif

            float dt = Time.deltaTime;

            mLastUpdateTime = TimeUtil.Now;

            mBuildingManager.OnUpdate(dt);
            mCharacterManager.OnUpdate(dt);
            mGoldSlimeSpawnManager.OnUpdate(dt);
            mExpensiveShopManager.OnUpdate();

            mRandomizeIntervalTime -= dt;
            if (mRandomizeIntervalTime <= 0)
            {
                mRandomizeIntervalTime = UnityEngine.Random.Range(1, 10);

                MLand.SavePoint.RandomizeKey();
            }

            mOfflineRewardIntervalTime -= dt;
            if (mOfflineRewardIntervalTime <= 0f)
            {
                mOfflineRewardIntervalTime = MLand.GameData.OfflineRewardCommonData.intervalTimeSeconds;

                var popup = MLand.PopupManager.GetPopup<Popup_OfflineRewardUI>();
                if (popup != null)
                    return;

                MLand.SavePoint.OfflineRewardManager.OnUpdate();
                MLand.SavePoint.Save();
            }
        }

        void OnBackButton()
        {
            if (MLand.PopupManager.Count > 0)
            {
                MLand.PopupManager.OnBackButton();
            }
            else
            {
                string title = StringTableUtil.Get("Title_Confirm");
                string desc = StringTableUtil.Get("Desc_ApplicationQuit");

                MonsterLandUtil.ShowConfirmPopup(title, desc, OnConfirm, null);

                void OnConfirm()
                {
                    Application.Quit();
                }
            }
        }

        private void OnEnable()
        {
            MLand.AdManager.OnEnableEvent();
        }

        void OnApplicationPause(bool pause)
        {
            MLand.AdManager.OnApplicationPauseEvent(pause);

            if (pause == false)
            {
                MLand.Lobby?.RefreshBuffDuration();
                // 복귀 했을 때 시간이 얼마 이상 지나있으면 서버에서 시간을 받아온다.
                CheckLongIdle();
            }
        }

        void CheckLongIdle()
        {
            // 5분정도 지났다면, 서버 시간을 갱신해주자
            int elapsed = TimeUtil.Now - mLastUpdateTime;
            int expire = TimeUtil.SecondsInMinute * 5;

            // 시간이 많이 어긋나있으면..
            // 일단 서버랑 시간만 맞추고 따로 조치를 취하지는 않는다.
            if (expire < elapsed && 
                TimeCheatingDetector.GettingOnlineTime == false)
            {
                SyncServerTime();
            }
        }

        void SyncServerTime()
        {
            string url = MLand.GameData.CommonData.timeCheatingDetectUrl;

            StartTouchBlock(10f);

            StartCoroutine(TimeCheatingDetector.GetOnlineTimeCoroutine(url, OnlineTimeCallBack));
        }

        void OnlineTimeCallBack(TimeCheatingDetector.OnlineTimeResult result)
        {
            EndTouchBlock();

            TimeUtil.SyncTo(result.OnlineDateTimeUtc);

            MLand.Lobby?.RefreshBuffDuration();
        }

        // =====================================
        // 몬스터 관련
        // =====================================
        public void SetFollowSlimeKing()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mSlimeKing.transform,
                FollowType = FollowType.SlimeKing,
            });
        }

        public void SetFollowSlime(string slimeId)
        {
            if (slimeId.IsValid())
            {
                var slime = GetSlime(slimeId);
                if (slime != null)
                {
                    MLand.CameraManager.SetFollowInfo(new FollowInfo()
                    {
                        FollowTm = slime.transform,
                        FollowType = FollowType.Slime,
                    });
                }
            }
        }

        public void SetFollowGoldSlime()
        {
            if (mCharacterManager.IsSpawnedGoldSlime == false)
                return;

            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mCharacterManager.GoldSlime.transform,
                FollowType = FollowType.Slime,
            });
        }

        public bool IsReadyForSpawnSlime(string id)
        {
            return mCharacterManager.IsReadyForSpawnSlime(id);
        }

        public void UpdateExpensiveShop(bool showMessage = false)
        {
            mExpensiveShopManager.RefreshShops(isShowMessage: showMessage);
        }

        public bool IsSatisfiedConditionSlime(string id)
        {
            return mCharacterManager.IsSatisfiedCondition(id);
        }

        public bool IsSpawnedSlime(string id)
        {
            return mCharacterManager.IsSpawnedSlime(id);
        }

        public void SpawnSlime(string id, double unlockPrice, Action onSpawnFinish)
        {
            if (MLand.SavePoint.UseGold(unlockPrice))
            {
                MLand.SavePoint.AddSlime(id);

                MLand.SavePoint.CheckQuests(QuestType.SpawnSlime);

                MLand.SavePoint.Save();

                MLand.Lobby.RefreshGoldText();

                StartCoroutine(PlaySpawnSlimeMotion(id, onSpawnFinish));
            }
        }

        public bool SpawnGoldSlime()
        {
            // 하루에 4번까지 소환이 가능하다.
            if (MLand.SavePoint.SlimeManager.StackGoldSlimeSpawnCount())
            {
                string goldSlimeId = MLand.GameData.GoldSlimeCommonData.id;

                MLand.SavePoint.AddSlime(goldSlimeId);

                MLand.SavePoint.Save();

                Character slime = mCharacterManager.SpawnSlime(goldSlimeId);
                if (slime != null)
                {
                    slime.StartPathFinder(MonsterLandUtil.GetCanMovePos());
                }

                return true;
            }

            return false;
        }

        public void RemoveGoldSlime()
        {
            string goldSlimeId = MLand.GameData.GoldSlimeCommonData.id;
            GoldSlime goldSlime = mCharacterManager.GetSlime(goldSlimeId) as GoldSlime;
            if (goldSlime != null)
            {
                if ( MLand.CameraManager.FollowTm == goldSlime.transform )
                {
                    MLand.CameraManager.ResetFollowInfo();
                }

                if (mCharacterManager.RemoveGoldSlime())
                {
                    MLand.SavePoint.RemoveSlime(goldSlimeId);

                    MLand.Lobby.SetActiveGoldSlimeButton(false);
                }
            }
        }

        public Slime GetSlime(string slimeId)
        {
            return mCharacterManager.GetSlime(slimeId);
        }

        public double CalcSlimeCoreDropAmountForMinute(int minute)
        {
            double slimeAmount = mCharacterManager.GetSlimeCoreDropAmountForMinute(minute);
            double buildingAmount = mBuildingManager.GetSlimeCoreDropAmountForMinute(minute);

            return slimeAmount + buildingAmount;
        }

        public double CalcSlimeCoreDropAmountForMinute(ElementalType type, int minute)
        {
            double slimeAmount = mCharacterManager.GetSlimeCoreDropAmountForMinute(type, minute);
            double buildingAmount = mBuildingManager.GetSlimeCoreDropAmountForMinute(type, minute);

            return slimeAmount + buildingAmount;
        }

        public bool IsSpwanedGoldSlime()
        {
            return mCharacterManager.IsSpawnedGoldSlime;
        }

        // =====================================
        // 건물 관련
        // =====================================
        public void UpgradeBuilding(string id, double upgradePrice, Action onBuildFinish)
        {
            // 약간의 코드 중복이 있지만 Save 두번 안하려고 이렇게 함
            if (MLand.SavePoint.UseGold(upgradePrice))
            {
                MLand.SavePoint.LevelUpBuilding(id);

                MLand.SavePoint.Save();

                MLand.Lobby.RefreshGoldText();

                StartCoroutine(PlayUpgradeBuildingMotion(id, onBuildFinish));
            }
        }

        public void UnlockBuilding(string id, double unlockPrice, Action onUnlockFinish)
        {
            // 약간의 코드 중복이 있지만 Save 두번 안하려고 이렇게 함
            if (MLand.SavePoint.UseGold(unlockPrice))
            {
                MLand.SavePoint.AddBuilding(id);

                MLand.SavePoint.Save();

                MLand.Lobby.RefreshGoldText();

                StartCoroutine(PlayBuildBuildingMotion(id, onUnlockFinish));
            }
            else
            {
                // 발생할 수 없는 일
                SoundPlayer.PlayErrorSound();
            }
        }
        
        public bool IsMaxLevelBuilding(string id)
        {
            return mBuildingManager.IsMaxLevelBuilding(id);
        }

        public bool IsUnlockedBuilding(string id)
        {
            return mBuildingManager.IsUnlockedBuilding(id);
        }

        public bool IsSatisfiedUnlockBuilding(string id)
        {
            return mBuildingManager.IsSatisfiedUnlockCondition(id);
        }

        public bool IsSatisfiedUpgradeBuilding(string id)
        {
            return mBuildingManager.IsSatisfiedUpgradeCondition(id);
        }

        public bool IsReadyForUnlockBuilding(string id)
        {
            return mBuildingManager.IsReadyForUnlockBuilding(id);
        }

        public bool IsReadyForUpgradeBuilding(string id)
        {
            return mBuildingManager.IsReadyForUpgradeBuilding(id);
        }

        public int GetBuildingLevel(string id)
        {
            return mBuildingManager.GetBuilding(id)?.Level ?? 0;
        }

        public Building GetBuilding(string buildingId)
        {
            return mBuildingManager.GetBuilding(buildingId);
        }

        public void SetFollowCheapShop()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mCheapShop.transform,
                FollowType = FollowType.Building,
            });
        }

        public void SetFollowMiniGame()
        {
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mMiniGame.transform,
                FollowType = FollowType.Building,
            });
        }

        public void SetFollwExpensiveShop()
        {
            // 아직 비싸다 상점이 활성화 되어 있지 않아서 못 가져올 수 있음
            ExpensiveShopClicker expensiveShop = mExpensiveShopManager.GetActiveExpensiveShop();
            if (expensiveShop == null)
                return;

            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = expensiveShop.transform,
                FollowType = FollowType.Building,
            });
        }

        public ExpensiveShopClicker GetActiveExpensiveShop()
        {
            return mExpensiveShopManager.GetActiveExpensiveShop();
        }

        // =====================================
        // 재화 관련
        // =====================================
        public void AddGold(double gold)
        {
            MLand.SavePoint.AddGold(gold);
            MLand.SavePoint.Save();
            MLand.Lobby.RefreshGoldText();

            // 보상 팝업
            var rewardData = new RewardData()
            {
                goldReward = gold
            };

            MonsterLandUtil.ShowRewardPopup(rewardData);
        }

        public bool UseGold(double gold)
        {
            if (MLand.SavePoint.UseGold(gold))
            {
                MLand.SavePoint.Save();

                MLand.Lobby.RefreshGoldText();

                return true;
            }

            return false;
        }

        public void AddGem(double gem)
        {
            MLand.SavePoint.AddGem(gem);
            MLand.SavePoint.Save();
            MLand.Lobby.RefreshGemText();

            // 보상 팝업
            var rewardData = new RewardData()
            {
                gemReward = gem
            };

            MonsterLandUtil.ShowRewardPopup(rewardData);
        }

        public bool UseGem(double gem)
        {
            if ( MLand.SavePoint.UseGem(gem) )
            {
                MLand.SavePoint.Save();

                MLand.Lobby.RefreshGemText();

                return true;
            }

            return false;
        }

        public void AddSlimeCore(ElementalType type, double amount)
        {
            MLand.SavePoint.AddSlimeCore(type, amount);
            MLand.SavePoint.Save();
            MLand.Lobby.RefreshSlimeCoreText(type);

            MLand.SavePoint.CheckAchievements(AchievementsType.SlimeCoreCollector, MLand.SavePoint.GetSlimeCoreAmount(type));
        }

        public void AddSlimeCores(double[] amounts)
        {
            MLand.SavePoint.AddSlimeCores(amounts);

            for (int i = 0; i < (int)ElementalType.Count; ++i)
            {
                ElementalType type = (ElementalType)i;

                MLand.SavePoint.Achievements.CheckAchievemets(AchievementsType.SlimeCoreCollector, MLand.SavePoint.GetSlimeCoreAmount(type));
            }

            MLand.SavePoint.Save();

            MLand.Lobby.RefreshSlimeCoresText();

            MLand.Lobby.RefreshNewDot();
        }

        public bool UseSlimeCore(ElementalType type, double amount)
        {
            if (MLand.SavePoint.UseSlimeCore(type, amount))
            {
                MLand.SavePoint.Save();

                MLand.Lobby.RefreshSlimeCoreText(type);

                return true;
            }

            return false;
        }

        public BoxOpenResult[] ReceiveReward(RewardData rewardData)
        {
            BoxOpenResult[] boxResults = MLand.SavePoint.ReceiveReward(rewardData);

            MLand.SavePoint.Save();

            MLand.Lobby.RefreshAllCurrencyText();

            return boxResults;
        }

        // ==============================
        // ETC
        // ==============================
        IEnumerator PlaySpawnSlimeMotion(string slimeId, Action onSpawnFinish)
        {
            // 다른데 터치 못하게 연출하는 동안 막아두장
            mTouchBlock.StartBlock(10f);

            MLand.CameraManager.ChangeMoveType(MoveType.Motion);
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mSlimeKing.transform,
                FollowType = FollowType.SlimeKing
            });

            var slimeData = MLand.GameData.SlimeData.TryGet(slimeId);

            // 킹 슬라임 마법진 생성!
            yield return mSlimeKing.PlaySpawnMotion(slimeData.elementalType);

            Character slime = mCharacterManager.SpawnSlime(slimeId);

            // 방구 생성
            MLand.ParticleManager.Aquire("SlimeSpawn", slime.transform, Vector3.up * 0.5f);

            SoundPlayer.PlaySpawnSlime();
            SoundPlayer.PlaySpawnBoom();

            // 각 속성 메인 건물 바로 앞에
            slime.StartPathFinder(SlimeSpawnPos[(int)slimeData.elementalType]);

            onSpawnFinish.Invoke();

            if (MLand.Lobby.InTutorial() == false)
                mTouchBlock.EndBlock();

            yield return new WaitForSeconds(1f);

            MLand.CameraManager.ChangeMoveType(MoveType.Original);

            MLand.Lobby.RefreshQuickGuide();
        }

        IEnumerator PlayUpgradeBuildingMotion(string id, Action onBuildFinish)
        {
            // 다른데 터치 못하게 연출하는 동안 막아두장
            mTouchBlock.StartBlock(10f);

            MLand.CameraManager.ChangeMoveType(MoveType.Motion);
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mSlimeKing.transform,
                FollowType = FollowType.SlimeKing
            });

            var buildingData = MLand.GameData.BuildingData.TryGet(id);
            var buildingStatData = DataUtil.GetBuildingStatData(id);

            // 킹 슬라임 마법진 생성!
            yield return mSlimeKing.PlaySpawnMotion(buildingData.elementalType, buildingStatData.expandField);

            CentralBuilding building = mBuildingManager.GetBuilding(id) as CentralBuilding;

            // 기모으는 연출 후
            yield return building.PlayUpgradeMotion();

            // 업그레이드 처리 후
            mBuildingManager.OnUpgradeBuilding(id);

            // 기모았던거 팡 터트리며 건물 등장
            yield return building.PlayLevelupNova(onBuildFinish);

            // 땅 확장 연출이 필요하면 구름 치워준다.
            if (buildingStatData.expandField)
            {
                yield return mBuildingManager.PlayFieldHideCloudFadeMotion(buildingData.elementalType);
            }

            if (MLand.Lobby.InTutorial() == false)
                mTouchBlock.EndBlock();

            MLand.Lobby.RefreshQuickGuide();

            MLand.CameraManager.ChangeMoveType(MoveType.Original);

            // 약 1초 후에 리뷰를 요청 조건을 만족했는지 확인하고
            // 조건을 만족했다면 리뷰를 요청하자.
            yield return new WaitForSeconds(1f);

            mReviewManager.RequestReview();
        }

        IEnumerator PlayBuildBuildingMotion(string id, Action onBuildFinish)
        {
            // 다른데 터치 못하게 연출하는 동안 막아두장
            mTouchBlock.StartBlock(10f);

            MLand.CameraManager.ChangeMoveType(MoveType.Motion);
            MLand.CameraManager.SetFollowInfo(new FollowInfo()
            {
                FollowTm = mSlimeKing.transform,
                FollowType = FollowType.SlimeKing
            });

            var buildingData = MLand.GameData.BuildingData.TryGet(id);

            // 킹 슬라임 마법진 생성!
            yield return mSlimeKing.PlaySpawnMotion(buildingData.elementalType, false);

            Building building = mBuildingManager.OnUnlockBuilding(id);

            // 방구 생성
            MLand.ParticleManager.Aquire("BuildBuilding", building.transform, Vector3.up * 0.5f);

            SoundPlayer.PlaySpawnSlime();
            SoundPlayer.PlaySpawnBoom();

            yield return new WaitForEndOfFrame();

            onBuildFinish.Invoke();

            if (MLand.Lobby.InTutorial() == false)
                mTouchBlock.EndBlock();

            yield return new WaitForSeconds(1f);

            MLand.CameraManager.ChangeMoveType(MoveType.Original);

            MLand.Lobby.RefreshQuickGuide();
        }

        public bool IsActiveTouchBlock()
        {
            return mTouchBlock.IsActive;
        }

        public void StartTouchBlock(float time, bool showLoadingImg = false)
        {
            mTouchBlock?.StartBlock(time, showLoadingImg);
        }

        public void EndTouchBlock()
        {
            mTouchBlock?.EndBlock();
        }
    }
}