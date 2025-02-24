using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace MLand
{
    class SoundManager : MonoBehaviour
    {
        const string DefaultLobbyBGM = "Lobby_Main_01";
        const int SECount = 15;
        const float DefaultmBgmDelay = 0.5f;

        Dictionary<string, AudioClip> mAudioList;
                
        public AudioSource mBGAudioPlayer;        
        public AudioSource[] mSFXAudioPlayer;
        public void Init()
        {
            mAudioList = new Dictionary<string, AudioClip>();

            Object[] audioSource = Resources.LoadAll("Sounds");

            for (int i = 0; i < audioSource.Length; ++i)
            {
                mAudioList.Add(audioSource[i].name, audioSource[i] as AudioClip);
            }

            //오디오소스 컴포넌트 호출
            mBGAudioPlayer = gameObject.AddComponent<AudioSource>();
            mSFXAudioPlayer = new AudioSource[SECount];
            for(int i = 0; i < SECount; ++i)
            {
                mSFXAudioPlayer[i] = gameObject.AddComponent<AudioSource>();
            }

            PlayBg(DefaultLobbyBGM);

            ApplySavePoint();
        }

        void ApplySavePoint()
        {
            SetBGSoundActive(SavePointBitFlags.OnBGMSound.IsOn());
            SetEffectSoundActive(SavePointBitFlags.OnEffectSound.IsOn());
        }

        public void PlayDefaultLobbyBg()
        {
            PlayBg(DefaultLobbyBGM);
        }

        public void PlayBg(string name, float volume = 0.5f, float delay = DefaultmBgmDelay)
        {
            if (mAudioList.TryGetValue(name, out var clip))
            {
                mBGAudioPlayer.clip = clip;
            }

            mBGAudioPlayer.loop = true;

            mBGAudioPlayer.DORewind();
            mBGAudioPlayer.DOFade(0f, delay * 0.5f)
                .OnComplete(() =>
                {
                    mBGAudioPlayer.DORewind();
                    mBGAudioPlayer.DOFade(volume, delay * 0.5f);
                    mBGAudioPlayer.Play();
                });
        }

        public void PauseBG()
        {
            if (mBGAudioPlayer.isPlaying)
            {
                mBGAudioPlayer.Pause();
            }
        }

        public void SetBGVolume(float volume)
        {
            mBGAudioPlayer.volume = volume;
        }

        public void UnPauseBG()
        {
            if (!mBGAudioPlayer.isPlaying)
            {
                mBGAudioPlayer.UnPause();
            }
        }

        public void SetBGSoundActive(bool on)
        {
            if (on)
                SetBGVolume(0.5f);
            else
                SetBGVolume(0f);
        }

        public void SetEffectSoundActive(bool on)
        {
            for (int i = 0; i < mSFXAudioPlayer.Length; ++i)
            {
                mSFXAudioPlayer[i].volume = on ? 1f : 0f;
            }
        }

        public AudioSource PlaySE(string name, float volume = 1f)
        {
            for (int i = 0; i < mSFXAudioPlayer.Length; ++i)
            {
                if (mSFXAudioPlayer[i].isPlaying) continue;
                if (mAudioList.TryGetValue(name, out var clip))
                {
                    mSFXAudioPlayer[i].clip = clip;
                    mSFXAudioPlayer[i].volume = SavePointBitFlags.OnEffectSound.IsOn() ? volume : 0f;
                    mSFXAudioPlayer[i].Play();
                    return mSFXAudioPlayer[i];
                }                
            }

            return null;
        }

        public void PauseSEAll()
        {
            for (int i = 0; i < mSFXAudioPlayer.Length; ++i)
            {
                if (mSFXAudioPlayer[i].isPlaying)
                {
                    mSFXAudioPlayer[i].Pause();
                }                
            }
        }
    }

    static class SoundPlayer
    {
        //======================================
        // Sound Effect
        //======================================
        public static void PlayHideCloud()
        {
            PlaySE("HideCloud");
        }

        public static void PlayOfflineLevelUp()
        {
            PlaySE("OfflineLevelUp");
        }
        public static void PlayAchievementsResult()
        {
            PlaySE("AchievementsResult");
        }

        public static void PlayTakePhoto()
        {
            PlaySE("TakePhoto");
        }

        public static void PlayAppearExpensiveShop()
        {
            PlaySE("AppearExpensiveShop");
        }

        public static void PlayUpdateExpensiveShop()
        {
            PlaySE("UpdateExpensiveShop");
        }

        public static void PlayMessageShowSound(float volume)
        {
            PlaySE("MessageSound", volume);
        }

        public static void PlayLoadingComplete()
        {
            PlaySE("LoadingComplete");
        }

        public static void PlayTitleSlimeAppear()
        {
            PlaySE("TitleSlimeAppear");
        }

        public static void PlayWind(float  volume)
        {
            PlaySE("Wind", volume);
        }
        
        public static void PlayShowRewardPopup()
        {
            PlaySE("ShowRewardPopup");
        }

        public static void PlayQuestClear()
        {
            PlaySE("QuestClear");
        }

        public static void PlayDropStamp()
        {
            PlaySE("DropStamp");
        }

        public static void PlayAttendanceCheck()
        {
            PlaySE("AttendanceCheck");
        }

        public static void PlaySlimeGift()
        {
            PlaySE($"SlimeGift");
        }

        public static void PlaySlimeEmotion(EmotionType type)
        {
            PlaySE($"Slime{type}", 0.5f);
        }

        public static void PlaySpawnBoom()
        {
            PlaySE("SlimeSpawnBoom");
        }

        public static void PlayCentralBuildingChargeStart()
        {
            PlaySE("CentralBuildingChargeStart", 0.5f);
        }

        public static void PlayCentralBuildingChargeFinish()
        {
            PlaySE("CentralBuildingChargeFinish", 0.5f);
        }

        public static void PlayAppearGoldSlime()
        {
            PlaySE("GoldSlimeAppear", 0.5f);
        }

        public static void PlayDisappearGoldSlime()
        {
            PlaySE("GoldSlimeDisappear", 0.5f);
        }

        public static void PlayFocus()
        {
            PlaySE("Swing3", 0.5f);
        }

        public static void PlayBoxAppear()
        {
            PlaySE("BoxAppear");
        }

        public static void PlayBeforeBoxOpen()
        {
            PlaySE("BeforeBoxOpen");
        }

        public static void PlayBoxShake()
        {
            PlaySE("BoxShake2");
        }

        public static void PlayShopBuyOrSell()
        {
            PlaySE("ShopBuyOrSell");
        }

        public static void PlaySlimeTouchSound()
        {
            int randIdx = Random.Range(0, 3);

            PlaySE($"SlimeTouchSound{randIdx + 1}");
        }

        public static void  PlayUIButtonTouchSound()
        {
            PlaySE("ButtonTouch", 0.5f);
        }

        public static void PlayBoxOpening()
        {
            PlaySE("BoxOpening");
        }

        public static void PlayBoxOpen()
        {
            PlaySE("BoxOpen");
        }

        public static void PlayItemDrop()
        {
            PlaySE("ItemDrop");
        }

        public static void PlayErrorSound()
        {
            PlaySE("ErrorSound");
        }

        public static void PlayBuffActive()
        {
            PlaySE("BuffActive");
        }

        public static void PlayShowPopup()
        {
            PlaySE("ShowPopup");
        }

        public static void PlayHidePopup()
        {
            PlaySE("HidePopup");
        }

        public static void PlayGetGold()
        {
            PlaySE("GetGold");
        }

        public static void PlayGetGem()
        {
            PlaySE("GetGem");
        }

        public static void PlayAirhorn()
        {
            PlaySE("Airhorn");
        }

        public static void PlayCountDown()
        {
            PlaySE("CountDown");
        }

        public static void PlaySchoolBell()
        {
            PlaySE("School_Bell");
        }

        public static void PlayMagicCircle()
        {
            PlaySE("MagicCircle");
        }

        public static void PlayMagicCircle2()
        {
            PlaySE("MagicCircle2");
        }

        public static void PlayElementalCoursesEnd()
        {
            PlaySE("MimiGame_ElementalCourses_EndGame");
        }

        public static void PlaySpawnSlime()
        {
            PlaySE("SpawnSlime");
        }

        public static void PlayMiniGame_TicTacToe_LineDraw()
        {
            PlaySE("LineDraw");
        }

        public static void PlayMiniGame_TicTacToe_Finish()
        {
            PlaySE("MiniGame_TicTacToe_Finish");
        }

        public static void PlayMiniGame_TicTacToe_ChangeTurn()
        {
            PlaySE("ChangeTurn");
        }

        public static void PlayMiniGame_TicTacToe_MakeMark(PlayerSymbol symbol)
        {
            PlaySE($"Make_Mark{symbol}");
        }

        public static void PlayMiniGame_TicTacToe_Win()
        {
            PlaySE("TicTacToe_Win");
        }

        public static void PlayMiniGame_TicTacToe_Lose()
        {
            PlaySE("TicTacToe_Lose");
        }

        public static void PlayMiniGame_TicTacToe_Draw()
        {
            PlaySE("TicTacToe_Draw");
        }

        public static void PlayMiniGameLevelUp()
        {
            PlaySE("MiniGame_LevelUp");
        }

        public static void PlaySlimeThrow()
        {
            PlaySE("Swing");
        }

        public static void PlayStartFever()
        {
            PlaySE("JJan");
        }

        public static void PlayElementalCoursesWrong()
        {
            PlaySE("Puppy");
        }

        public static void PlayElementalCoursesCollect(bool inFever)
        {
            string name = inFever ? 
                "MimiGame_ElementalCourses_Collect_InFever" :
                "MimiGame_ElementalCourses_Collect";

            PlaySE(name);
        }

        public static AudioSource PlayTimeLimit()
        {
            return PlaySE("TimeLimit");
        }

        public static void PlayStartMagicCircle()
        {
            PlaySE("StartMagicCircle");
        }

        public static void PlayShowPortraitMotion()
        {
            PlaySE("UI_Animate_Noise_Glide_Appear_stereo");
        }

        public static void PlayHidePortraitMotion()
        {
            PlaySE("UI_Animate_Noise_Glide_Disappear_stereo");
        }

        // ===========================================
        // BGM
        // ===========================================
        public static void PlayLobbyBGM()
        {
            MLand.SoundManager.PlayDefaultLobbyBg();
        }

        public static void PlayMiniGameHomeBGM()
        {
            PlayBG("MiniGame_Home");
        }

        public static void PlayMiniGameElementalCourses_ReadyBGM()
        {
            PlayBG("MiniGame_ElementalCourses_Ready");
        }

        public static void PlayMiniGameElementalCourses_PlayBGM()
        {
            PlayBG("MiniGame_ElementalCourses_Play");
        }

        public static void PlayMiniGameTicTacToeBGM()
        {
            PlayBG("MiniGame_TicTacToe");
        }

        static AudioSource PlaySE(string name, float volume = 1f)
        {
            return MLand.SoundManager.PlaySE(name, volume);
        }

        static void PlayBG(string name, float volume = 0.5f)
        {
            MLand.SoundManager.PlayBg(name, volume);
        }
    }
}

