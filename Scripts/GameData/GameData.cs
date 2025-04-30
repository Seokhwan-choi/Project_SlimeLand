using System.Collections;
using System.Collections.Generic;
using ExcelToObject;
using System;

namespace MLand
{
    public class GameData
    {
        public CommonData CommonData;
        public CameraCommonData CameraCommonData;

        // 미니 게임
        public MiniGameCommonData MiniGameCommonData;
        public MiniGameElementalCoursesData MiniGameElementalCoursesData;
        public MiniGameTicTacToeData MiniGameTicTacToeData;
        public Dictionary<string, MiniGameRewardData> MiniGameRewardData;
        public Dictionary<string, MiniGameElementalCoursesAttendData> MiniGameElementalCoursesAttendData;

        // 슬라임
        public Dictionary<string, CharacterData> SlimeData;
        public SlimeCommonData SlimeCommonData;
        public GoldSlimeCommonData GoldSlimeCommonData;
        public List<SlimeFriendShipLevelUpData> SlimeFriendShipLevelUpData;
        public List<SlimeLevelUpRewardData> SlimeLevelUpRewardData;

        // 특수한 캐릭터 데이터
        public Dictionary<string, CharacterData> CharacterData;
        public List<SlimeFriendShipLevelUpData> CharacterFriendShipLevelUpData;

        // 건물
        public BuildingCommonData BuildingCommonData;
        public Dictionary<string, BuildingData> BuildingData;
        public Dictionary<string, BuildingUnlockData> BuildingUnlockData;
        public List<BuildingStatData> BuildingStatData;
        public List<BuildingUpgradeData> BuildingUpgradeData;

        // 상점 관련 데이터
        public ShopCommonData ShopCommonData;
        public Dictionary<string, GoldShopData> GoldShopData;
        public Dictionary<string, GemShopData> GemShopData;
        public Dictionary<string, BoxShopData> BoxShopData;
        public Dictionary<string, BoxData> BoxData;
        public ExpensiveShopItemProbData ExpensiveShopItemProbData;

        // 아이템 관련 데이터
        public Dictionary<string, FriendShipItemData> FriendShipItemData;

        // 버프
        public Dictionary<BuffType, BuffData> BuffData;

        // 출석부
        public AttendanceCommonData AttendanceCommonData;
        public Dictionary<int, AttendanceRewardData> AttendanceRewardData;

        // 퀘스트
        public DailyQuestCommonData DailyQuestCommonData;
        public Dictionary<string, DailyQuestData> DailyQuestData;
        public Dictionary<string, RewardData> RewardData;
        public List<StepQuestData> StepQuestData;

        // 업적
        public Dictionary<string, AchievementsData> AchievementsData;

        // 튜토리얼
        public List<TutorialData> TutorialData;

        // 오프라인 보상
        public OfflineRewardCommonData OfflineRewardCommonData;
        public List<OfflineRewardLevelUpData> OfflineRewardLevelUpData;

        // 코스튬
        public Dictionary<string, CostumeData> CostumeData;
        public List<CostumeUpgradeData> CostumeUpgradeData;
        public List<CostumeStatData> CostumeStatData;
        public List<CostumePosData> CostumePosData;

        // 스트링 테이블
        public Dictionary<string, StringTable> StringTable;
    }

    public class CharacterData
    {
        public string id;                       // 슬라임 Id
        public ElementalType elementalType;     // 슬라임 속성 타입
        public int level;
        public string spriteImg;                // 슬라임 기본 spriteImg;
        public string precendingSlime;          // 슬라임 해제 조건 # 1 ( 슬라임의 id, 비어있으면 조건 X )
        public string precendingBuilding;       // 슬라임 해제 조건 # 2 ( 본부 건물 id, 비어있으면 조건 X )
        public int precendingBuildingLevel;     // 슬라임 해제 조건 # 2-1 ( 본부 건물 id의 Level, 비어있으면 조건 X )
        public double unlockPrice;              // 슬라임 소환 비용
        public double slimeCoreDropAmount;      // 슬라임 똥 기본 드랍양
        public float slimeCoreDropCoolTime;     // 슬라임 똥 생산 시간
        public int friendshipMaxLevel;          // 호감도 Max Level
        public float moveSpeed;                 // 이동 속도
    }

    public class SlimeCommonData
    {
        public float idleTime;
        public float exitedTime;
        public float happyTime;
        public float shockTime;
        public float sleepyTime;
        public float spawnTime;
        public float communicationTime;
        public int communicationTouchCount;
        public float emojiDuration;
    }

    public class GoldSlimeCommonData
    {
        public string id;
        public string precendingBuilding;
        public int spawnIntervalMinute;
        public float durationMinute;
        public int maxDailyCount;
        public int rewardMinute;
    }

    public class SlimeFriendShipLevelUpData
    {
        public int level;
        public float requireStackedExp;
    }

    public class SlimeLevelUpRewardData
    {
        public int slimeLevel;
        public int level;
        public double rewardGem;
        public double rewardSlimeCoreForMinute;
        public double increaseSlimeCoreDropAmount;
        public float decreaseSlimeCoreDropCoolTime;
    }

    public class FriendShipItemData
    {
        public string id;
        public ItemType itemType;
        public ItemGrade grade;
        public double friendShipExp;
        public string spriteImg;
        public int goldPriceforMinute;
    }

    public class BuildingCommonData
    {
        public int centralBuildingMaxLevel;
    }

    public class BuildingData
    {
        public string id;                       // 건물 Id
        public ElementalType elementalType;     // 건물 속성 타입
        public bool isCentralBuilding;          // 중앙 건물인지 확인
    }

    public  class BuildingUnlockData
    {
        public string id;
        public ElementalType elementalType;     // 건물 속성 타입
        public string precendingBuilding;       // 선행 건물의 id ( 비어있으면 확인 안함 )
        public int precendingBuildingLevel;     // 선행 건물의 Level ( 선행 건물의 id가 없거나 비어있으면 확인 안함 )
        public double unlockPrice;              // 건물 건축 비용
        public string objName;                 // 건물 위치
    }

    public class BuildingStatData
    {
        public string id;
        public int level;
        public double slimeCoreDropAmount;      // 슬라임 똥 기본 드랍양
        public float slimeCoreDropCoolTime;     // 슬라임 똥 생산 시간
        public string spriteImg;                // 건물 img 이름
        public bool expandField;  
    }

    public class BuildingUpgradeData
    {
        public string id;
        public int level;
        public double upgradePrice;
        public string precendingBuilding;       // 선행 건물의 id ( 비어있으면 확인 안함 )
        public int precendingBuildingLevel;     // 선행 건물의 Level ( 선행 건물의 id가 없거나 비어있으면 확인 안함 )
        public string objName;
    }

    public class ShopCommonData
    {
        public int slimeCorePriceChangeHour;
        public float slimeCorePriceRangeValue;
        public float slimeCoreDefaultPrice;
        public int expensiveFirstOpenWaitMinute;
        public int expensiveShopOpenPeriodHour;
        public int expensiveShopItemCount;
        public int expensiveShopUpdateDailyCount;
        public double expensiveShopUpdateGemPrice;
        public string expensiveShopPrecendingBuilding;
        public int expensiveShopPrecendingBuildingLevel;
        public int gemShopSlotCount;
        public float costumeReturnValuePerPiece;
        public string removeAdProductId;
        public double removeAdPurchaseRewardGem;
    }

    public class ExpensiveShopItemProbData
    {
        public float[] itemTypeProb;
        public float[] gradeProb;
    }

    public class GoldShopData
    {
        public string id;
        public double gemPrice;
        public int goldAmountForMinute;
        public string spriteImg;
    }

    public class GemShopData
    {
        public string id;
        public double gemAmount;
        public double bonusGemAmount;
        public double fakePrice;
        public string spriteImg;
        public string productId;

        public int slot;
        public int priority;
        public bool onlyOne;
        public int watchAdDailyCount;
    }

    public class BoxShopData
    {
        public string id;
        public BoxType boxType;
        public string[] boxId;
        public double[] gemPrice;
        public int watchAdDailyCount;
        public string spriteImg;
    }

    public class BoxData
    {
        public string id;
        public BoxType boxType;
        public int openCount;
        public string costumeAmountRange;
        public float[] costumeTypeProb;
        public float[] itemTypeProb;
        public float[] gradeProb;
        public string spriteOpenImg;
        public string spriteCloseImg;
    }

    public class CommonData
    {
        public int friendShipMaxLevel;
        public string timeCheatingDetectUrl;
        public float toastMessageLiftTime;
        public string playStoreUrl;
        public string requestReviewBuilding;
        public int requestReviewBuildingLevel;
        public string luckySymbol;
    }

    public class CameraCommonData
    {
        public float slimeKingHeight;
        public float otherHeight;
        public float cameraMaxSize;
        public float cameraMinSize;
        public float cameraIntroSize;
        public float mapSizeX;
        public float mapSizeY;
        public float moveSpeed;
        public float zoomSpeed;
        public float originalCameraPosZ;
        public float originalCameraSize;
    }

    public class MiniGameCommonData
    {
        public float penaltyTime;
    }

    public class MiniGameElementalCoursesData
    {
        public float playTime;
        public int feverCount;
        public int feverTime;
        public int maxCombo;
        public int maxTeamCount;
        public int maxLevel;
        public float level2CheckTime;
        public float level3CheckTime;
        public int dailyFreePlayCount;
        public double gemPlayPrice;
        public int watchAdBonusValue;
        public float timeLimit;
    }

    public class MiniGameElementalCoursesAttendData
    {
        public string id;
        public bool attend;
    }

    public class MiniGameTicTacToeData
    {
        public int loseScore;
        public int drawScore;
        public int winScore;
        public int dailyFreePlayCount;
        public double gemPlayPrice;
    }

    public class MiniGameRewardData
    {
        public string score;
        public string rewardId;
    }

    public class BuffData
    {
        public BuffType buffType;
        public int duration;
        public int coolTime;
        public float buffValue;
        public int maxDailyCount;
    }

    public class AttendanceRewardData
    {
        public int day;
        public string rewardId;
    }

    public class AttendanceCommonData
    {
        public int maxDay;
    }

    public class DailyQuestCommonData
    {
        public int maxQuestCount;
    }

    public class DailyQuestData
    {
        public string id;
        public QuestType questType;
        public int requireCount;
        public string rewardId;
    }

    public class AchievementsData
    {
        public string id;
        public AchievementsType type;
        public int requireCount;
        public AchievementsRequireType requireType;
        public string rewardId;
        public string spriteImg;
    }

    public class StepQuestCommonData
    {
        public int maxStepCount;
    }

    public class StepQuestData
    {
        public string id;
        public QuestType type;
        public int step;
        public int requireStackCount;
        public string rewardId;
    }

    public class RewardData
    {
        public string id;
        public double gemReward;
        public double goldReward;
        public double[] slimeCores;
        public string friendShipReward;
        public int friendShipRewardCount;
        public string boxReward;
        public int boxRewardCount;
    }

    public class TutorialData
    {
        public string id;
        public int step;
        public TutorialActionType actionType;
        public string message;
        public string character;
        public float waitTime;
    }

    public class OfflineRewardCommonData
    {
        public int defaultMaxMinute;
        public int intervalTimeSeconds;
        public double adBonusValue;
        public double defaultValue;
    }

    public class OfflineRewardLevelUpData
    {
        public int level;
        public double price;
        public int addTimeForMinute;
    }

    public class CostumeData
    {
        public string id;
        public CostumeType costumeType;
        public string achievementsId;
        public double gemPrice;
        public float slimeCoreDropAmount;
        public float slimeCoreDropCoolTime;
        public string thumbnail;
        public string spriteImg;
        public string spriteImg2;
    }

    public class CostumeUpgradeData
    {
        public int level;
        public int requirePiece;
    }

    public class CostumeStatData
    {
        public string id;
        public int level;
        public double slimeCoreDropAmount;      // 슬라임 똥 기본 드랍양
        public float slimeCoreDropCoolTime;     // 슬라임 똥 생산 시간
    }

    public class CostumePosData
    {
        public string id;
        public string costumeId;
        public int orderInLayer;
        public string[] pos;
    }

    public class StringTable
    {
        public string key;
        public string[] text;
    }

    public struct Pos
    {
        public float X;
        public float Y;

        public static Pos Parse(string value)
        {
            string first = string.Empty;
            string second = string.Empty;

            bool haveRange = value.IsValid() && value.SplitWord(',', out first, out second);
            if (haveRange)
            {
                return new Pos() { X = first.ToFloatSafe(), Y = second.ToFloatSafe() };
            }
            else
            {
                float v = first.ToFloatSafe();
                return new Pos() { X = v, Y = v };
            }
        }
    }

    public struct MinMaxRange
    {
        public int min;
        public int max;

        public int SelectRandom()
        {
            return min == max ? min : UnityEngine.Random.Range(min, max + 1);
        }

        public bool IsInRange(int num)
        {
            return min <= num && num <= max;
        }

        public static MinMaxRange Parse(string value)
        {
            string first, second;

            bool haveRange = value.SplitWord('~', out first, out second);
            if (haveRange)
            {
                return new MinMaxRange() { min = first.ToIntSafe(), max = second.ToIntSafe() };
            }
            else
            {
                int v = first.ToIntSafe();
                return new MinMaxRange() { min = v, max = v };
            }
        }

        public override string ToString()
        {
            return $"{min}~{max}";
        }
    }

    // 포켓몬 상성 시스템 영문 위키피디아 참조
    public enum ElementalType
    {
        Water,  // 수 속성 ( 물 )
        Grass,  // 목 속성 ( 잔디 )
        Fire,   // 불 속성 ( 불 )
        Rock,   // 땅 속성 ( 바위 )

        Count
    }

    public enum ItemGrade
    {
        Normal,
        Rare,
        Unique,
        Count,
    }

    public enum ItemType
    {
        Toy,
        Food,
        SlimeCore,
        Gold,
        Gem,
        FriendShipExp,
        RandomBox,
        Costume,
    }

    public enum BuffType
    {
        None,
        DecreaseDropCoolTime,
        IncreaseDropAmount,

        Count,
    }

    public enum QuestType
    {
        None,
        Attendance,
        DailyquestClear,
        PlayAnyMiniGame,
        TouchSlime,
        SpawnSlime,
        BuildBuilding,
        FindGoldSlime,
        GiftSlime,
        BuyAdGem,
        WatchAd,
        OpenRandomBox,
        VisitExpensiveShop,
        PhotoShare,
        OpenCostumeBox,

        Count
    }

    public enum AchievementsType
    {
        None,
        LuckySymbol,        // 행운의 상징
        CottonCandy,        // 솜사탕
        BestFriend,         // 베스트 프랜드
        SpaceOut,           // 슬라임 멍
        PerfectAttendance,  // 개근
        HonorStudent,       // 우등생
        Influencer,         // 인플루언서
        GoldHunter,         // 황금 사냥꾼
        EyeShopping,        // 아이쇼핑
        SlimeCoreCollector, // 똥 수집가
    }

    public enum AchievementsRequireType
    {
        Stack,
        Compare,

        Count,
    }

    public enum CostumeType
    {
        Face,
        Body,
        Acc,    // 악세사리 (Accessories)
        Count,
    }

    public enum MiniGameType
    {
        ElementalCourses,
        TicTacToe,

        Count
    }

    public enum BoxType
    {
        FriendShip,
        Costume,

        Count
    }

    public enum BoxOpenType
    {
        One,
        Ten,
        Ad,
        Reward,

        Count
    }

    public enum LangCode
    {
        KR, // 한국어
        US, // 영어
        JP, // 일본어
        CN, // 중국어 간체
        TW, // 중국어 번체

        Count
    }

    public enum TutorialActionType
    {
        ShowMessage,                                // 킹 슬라임 대화
        JustWait,                                   // 단순 대기
        Close_CurrentDetail,                        // 현재 디테일 팝업 닫기
        Close_CurrentPopupStatus,                   // 현재 스테이터스 팝업 닫기
        Close_CurrentPopup,                         // 현재 제일 위 팝업을 닫기
        Close_AllPopup,                             // 현재 켜져있는 모든 팝업을 닫는다.
        Focus_SlimeKing,                            // 카메라 슬라임킹 보도록
        Focus_GoldSlime,                            // 카메라 황금 슬라임 보도록
        Focus_CheapShop,                            // 카메라 싸다 상점 보도록
        Focus_ExpensiveShop,                        // 카메라 활성화된 비싸다 상점 보도록
        Focus_MiniGame,                             // 카메라 미니게임장 보도록
        Highlight_SlimeButton,                      // NavBar SlimeButton 하이라이트
        Highlight_SlimeSpawnButton,                 // 지금 소환할 수 있는 슬라임 중 가장 첫번째 버튼 하이라이트
        Highlight_CurrentSlime,                     // 지금 선택 중인 슬라임 하이라이트 ( 만약 선택 중인 슬라임이 없다면 무시하자 )
        Highlight_CurrentSlime_GiftButton,          // 지금 선택 중인 슬라임 선물하기 버튼 하이라이트 ( 만약 선택 중인 슬라임이 없다면 무시하자 )
        Highlight_CurrentSlime_LevelupRewardButton, // 지금 선택 중인 슬라임 보상목록 버튼 하이라이트 ( 만약 선택 중인 슬라임이 없다면 무시하자 )
        Highlight_BuildButton,                      // NavBar의 BuildButton 하이라이트
        Highlight_BuildingBuildButton,              // 지금 건축할 수 있는 건물 중 가장 처음 버튼을 하이라이트
        Highlight_CheapShop,                        // 필드의 싸다 상점 하이라이트
        Highlight_CheapShop_Gold,                   // 싸다 상점 골드 탭 버튼 하이라이트
        Highlight_CheapShop_Gem,                    // 싸다 상점 잼 탭 버튼 하이라이트
        Highlight_CheapShop_RandomBox,              // 싸다 상점 상자 탭 버튼 하이라이트
        Highlight_ExpensiveShop,                    // 필드의 활성화된 비싸다 상점 하이라이트
        Highlight_MiniGame,                         // 필드의 미니게임장 하이라이트
        Highlight_GoldSlime,                        // 필드의 소환된 황금 슬라임 하이라이트

        Count
    }
}