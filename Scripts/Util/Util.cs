using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.AI;
using DG.Tweening;
using TMPro;

namespace MLand
{
    static class Util
    {
        public static char SelectAny(string str)
        {
            if (str == string.Empty)
                return (char)0;

            float seed = Time.time * 100f;

            UnityEngine.Random.InitState((int)seed);

            return str[UnityEngine.Random.Range(0, str.Length)];
        }

        public static bool Dice(float prob = 0.5f)
        {
            var probs = new float[2] { prob, 1 - prob };

            int idx = RandomChoose(probs);

            return idx == 0;
        }

        public static int RandomChoose(float[] probs)
        {
            float total = 0;

            foreach (float elem in probs)
            {
                total += elem;
            }

            float randomPoint = UnityEngine.Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probs[i];
                }
            }
            return probs.Length - 1;
        }

        public static T RandomEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        public static GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            return GameObject.Instantiate<GameObject>(prefab, parent);
        }

        public static GameObject Instantiate(string prefabPath, Transform parent = null)
        {
            var obj = Resources.Load<GameObject>(prefabPath);

            if (obj == null)
            {
                return default(GameObject);
            }

            GameObject go = GameObject.Instantiate(obj, parent);

            return go;
        }

        public static GameObject InstantiateUI(string prefabPath, Transform parent = null)
        {
            if (parent == null)
            {
                parent = GameObject.Find("Canvas")?.transform ?? null;
            }

            var go = Instantiate($"UI/{prefabPath}", parent);

            //var rectTm = go.GetComponent<RectTransform>();
            //if (rectTm != null)
            //{
            //    rectTm.offsetMin = new Vector2(0, 0);
            //    rectTm.offsetMax = new Vector2(0, 0);
            //}

            return go;
        }

        public static GameObject Find(this GameObject go, string name, bool includeinactive = false)
        {
            if (go != null)
            {
                var tmList = go.GetComponentsInChildren<Transform>(includeinactive);
                foreach (var tm in tmList)
                {
                    if (tm.name == name)
                        return tm.gameObject;
                }
            }

            return null;
        }

        public static T Find<T>(this GameObject go, string name, bool includeinactive = false)
        {
            if (go != null)
            {
                var tm = go.Find(name, includeinactive);
                return tm.GetComponent<T>();
            }

            return default(T);
        }

        public static bool IsOverLapPoint2D(this GameObject go, Vector2 point)
        {
            return go.GetComponent<BoxCollider2D>()?.OverlapPoint(point) ?? false;
        }

        public static void Destroy(this GameObject go)
        {
            if (go != null)
            {
                GameObject.Destroy(go);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        public static T Get<T>(this GameObject go)
        {
            T comp = go.GetComponent<T>();

            return comp;
        }

        static public void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static bool Intersects(this GameObject go, GameObject other)
        {
            BoxCollider colliderA = go.GetComponent<BoxCollider>();
            BoxCollider colliderB = other.GetComponent<BoxCollider>();
            if (colliderA != null && colliderB != null)
            {
                Bounds boundA = colliderA.bounds;
                Bounds boundB = colliderB.bounds;

                return boundA.Intersects(boundB);
            }

            return false;
        }

        public static bool Intersects2D(this GameObject go, GameObject other)
        {
            BoxCollider2D colliderA = go.GetComponent<BoxCollider2D>();
            BoxCollider2D colliderB = other.GetComponent<BoxCollider2D>();
            if (colliderA != null && colliderB != null)
            {
                Bounds boundA = colliderA.bounds;
                Bounds boundB = colliderB.bounds;

                return boundA.Intersects(boundB);
            }

            return false;
        }

        public static void ResetAnchor(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
        }

        public static TValue TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            dic.TryGetValue(key, out value);

            return value;
        }

        public static float GetDistanceBy2D(Vector2 a, Vector2 b)
        {
            return (a - b).magnitude;
        }

        public static float GetDistance(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude;
        }

        public static Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return Camera.main.WorldToScreenPoint(worldPos);
        }

        public static GameObject FindGameObject(this GameObject go, string name)
        {
            GameObject find = go.Find(name, true);

            Debug.Assert(find != null, $"{go.name}의 {name} GameObject가 존재하지 않음");

            return find;
        }

        public static T FindComponent<T>(this GameObject go, string name)
        {
            GameObject componentObj = go.FindGameObject(name);

            T component = componentObj.GetComponent<T>();

            Debug.Assert(component != null, $"{go.name}의 {typeof(T).Name} {name}가 존재하지 않음");

            return component;
        }

        public static void AllChildObjectOff(this GameObject parent)
        {
            // 불필요한 오브젝트 Off
            foreach (Transform child in parent.GetComponentsInChildren<Transform>())
            {
                if (parent.transform == child)
                    continue;

                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 이 코드를 실행하기 전에 한 프레임 기다려줘야함
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D CaptureTextureInRectTmArea(CanvasScaler cs, RectTransform rectTm)
        {
            // Canvas의 크기 가져오기
            Vector2 canvasSize = cs.referenceResolution;

            // UI 요소의 크기 가져오기
            Vector2 uiSize = rectTm.rect.size;

            // 화면의 크기 가져오기
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            // Canvas의 비율과 화면의 비율 중 작은 비율을 사용하여 Pixel Size 계산
            float scale = Mathf.Min(screenSize.x / canvasSize.x, screenSize.y / canvasSize.y);
            Vector2 pixelSize = uiSize * scale;

            int pixelWidth = (int)pixelSize.x;
            int pixelHeight = (int)pixelSize.y;

            Rect captureRect = new Rect(rectTm.position.x - (pixelWidth * 0.5f), rectTm.position.y - (pixelHeight * 0.5f), pixelWidth, pixelHeight);

            Texture2D captureTexture = new Texture2D(pixelWidth, pixelHeight, TextureFormat.RGB24, false);
            captureTexture.ReadPixels(captureRect, 0, 0);
            captureTexture.Apply();

            return captureTexture;
        }
    }
    
    public static class ButtonExt
    {
        public static void SetButtonAction(this Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                SoundPlayer.PlayUIButtonTouchSound();

                action?.Invoke();
            });
        }
    }

    public static class StringExt
    {
        public static bool IsValid(this string s)
        {
            return string.IsNullOrEmpty(s) == false;
        }

        // delimited의 줄임말, delim ( 구분 문자 )
        public static string[] SplitByDelim(this string s, char delim = ',')
        {
            IEnumerable<string> array = s.Split(delim).Select(x => x.Trim());

            return array.ToArray();
        }

        public static bool SplitWord(this string s, char word, out string first, out string second)
        {
            string[] splitResult = s.Split(word).Select(x => x.Trim()).ToArray();

            if (splitResult.Length <= 0 || splitResult.Length > 2)
            {
                first = string.Empty;
                second = string.Empty;

                return false;
            }
            else
            {
                first = splitResult[0];
                second = splitResult[1];

                return true;
            }
        }

        public static float ToFloatSafe(this string s, float defaultValue = 0)
        {
            if (s.IsValid())
            {
                float f;
                if (float.TryParse(s, out f))
                    return f;
            }

            return defaultValue;
        }

        public static int ToIntSafe(this string s, int defaultValue = 0)
        {
            if (s.IsValid())
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
            }

            return defaultValue;
        }
    }

    public static class TimeUtil
    {
        static DateTime mPivot = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public const int SecondsInMinute = 60;
        public const int SecondsInHour = 60 * 60;
        public const int SecondsInDay = 24 * SecondsInHour;
        public const int SecondsInWeek = 7 * SecondsInDay;
        public const int MinutesInHour = 60;
        public const int MinutesInDay = 24 * 60;

        static int mSyncOffset;

        public static int Now
        {
            get
            {
                return LocalNow + mSyncOffset;
            }
        }

        public static int LocalNow
        {
            get
            {
                return (int)((DateTime.UtcNow - mPivot).TotalSeconds);
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow.AddSeconds(mSyncOffset);
            }
        }

        public static void SyncTo(DateTime serverUtcNow)
        {
            int serverNow = (int)(serverUtcNow - mPivot).TotalSeconds;

            SyncTo(serverNow);
        }

        public static void SyncTo(int serverNow)
        {
            mSyncOffset = serverNow - LocalNow;
        }

        public static int NowDateNum(int timeZone = 9)
        {
            DateTime now = UtcNow.AddHours(timeZone);

            int year = now.Year * 10000;
            int month = now.Month * 100;
            int day = now.Day;

            // 2068 01 19
            // YYYY/MM/DD
            return year + month + day;
        }

        public static string GetTimeStr(int second, bool ignoreHour = false)
        {
            if (second < 0)
                second = 0;

            int hour = second / SecondsInHour;
            second -= hour * SecondsInHour;

            int min = second / SecondsInMinute;
            second -= min * SecondsInMinute;

            int sec = second;
            if (ignoreHour == false)
            {
                return $"{hour:00}:" + $"{min:00}:" + $"{sec:00}";
            }
            else
            {
                return $"{min:00}:" + $"{sec:00}";
            }
        }

        public static int RemainSecondsToNextDay(int hourOffset = 9)
        {
            DateTime now = UtcNow.AddHours(hourOffset);
            DateTime tomorrow = now.AddDays(1).Date;

            return (int)((tomorrow - now).TotalSeconds);
        }
    }

    static class MonsterLandUtil
    {
        public static Sprite GetSlimeCoreImg(ElementalType type)
        {
            return MLand.Atlas.GetCurrencySprite($"SlimeCore_{type}");
        }

        public static Sprite GetEmblemImg(ElementalType type)
        {
            return MLand.Atlas.GetUISprite($"Emblem_{type}");
        }

        public static Sprite GetSlimeUIImg(string slimeId)
        {
            CharacterData slimeData = MLand.GameData.SlimeData.TryGet(slimeId);

            return MLand.Atlas.GetUISprite(slimeData?.spriteImg ?? string.Empty);
        }

        public static Sprite GetSlimeImg(string slimeId)
        {
            CharacterData slimeData = MLand.GameData.SlimeData.TryGet(slimeId);

            return MLand.Atlas.GetCharacterSprite(slimeData?.spriteImg ?? string.Empty);
        }

        public static Sprite GetRandomSlimeEmotionImg(string slimeId)
        {
            EmotionType randType = CharacterUtil.GetRandEmotion();

            return GetSlimeEmotionImg(slimeId, randType);
        }

        public static Sprite GetSlimeEmotionImg(string slimeId, EmotionType type)
        {
            if (type == EmotionType.Idle)
            {
                return GetSlimeImg(slimeId);
            }
            else
            {
                return MLand.Atlas.GetCharacterSprite($"{slimeId}_{type}");
            }
        }

        public static void ShowRewardPopup(RewardData data)
        {
            Popup_RewardUI rewardPopup = MLand.PopupManager.CreatePopup<Popup_RewardUI>();

            rewardPopup.Init(data);
        }

        public static void ShowRewardPopup(ItemInfo[] itemInfos)
        {
            Popup_RewardUI rewardPopup = MLand.PopupManager.CreatePopup<Popup_RewardUI>();

            rewardPopup.Init(itemInfos);
        }

        public static void ShowDescPopup(ItemInfo itemInfo)
        {
            if (itemInfo.Type != ItemType.Costume)
            {
                if (itemInfo.Type == ItemType.Food || itemInfo.Type == ItemType.Toy)
                {
                    Popup_DescriptionUI descPopup = MLand.PopupManager.CreatePopup<Popup_DescriptionUI>();

                    descPopup.Init(itemInfo);
                }
                else
                {
                    Popup_DescriptionUI descPopup = MLand.PopupManager.CreatePopup<Popup_DescriptionUI>("Popup_DescriptionUI2");

                    descPopup.Init(itemInfo);
                }
            }
            else
            {
                Popup_CostumeDetailUI detailPopup = MLand.PopupManager.CreatePopup<Popup_CostumeDetailUI>("Popup_CostumeDetailUI2");

                detailPopup.Init(itemInfo.Id);
            }
        }

        public static Popup_ConfirmUI ShowConfirmPopup(string title, string desc, Action onConfirm, Action onCancel, bool ignoreModalTouch = false)
        {
            Popup_ConfirmUI popup = MLand.PopupManager.CreatePopup<Popup_ConfirmUI>();

            popup.Init(title, desc, onConfirm, onCancel, ignoreModalTouch);

            return popup;
        }

        public static Popup_ConfirmUI ShowAdConfirmPopup(string title, string desc, Action onConfirm, Action onCancel = null)
        {
            Popup_ConfirmUI popup = MLand.PopupManager.CreatePopup<Popup_ConfirmUI>("Popup_AdConfirmUI");

            popup.Init(title, desc, () => ShowRewardedAd(onConfirm), onCancel);

            return popup;
        }

        public static void ShowRewardedAd(Action onConfirm)
        {
            MLand.AdManager.ShowRewardedVideo(onConfirm, adFailedShow: OnAdFailedShow);

            void OnAdFailedShow()
            {
                Popup_ConfirmUI popup = MLand.PopupManager.CreatePopup<Popup_ConfirmUI>();

                string title = StringTableUtil.Get("Title_AdFailedShow");
                string desc = StringTableUtil.GetDesc("AdFailedShowGiveReward");

                popup.Init(title, desc, onConfirm, null);
                popup.SetHideCancelButton();
            }
        }

        public static void ShowSystemMessage(string systemMessage)
        {
            Popup_SystemMessageUI systemPopup = MLand.PopupManager.CreatePopup<Popup_SystemMessageUI>(showMotion: false);

            systemPopup.Init(systemMessage);
        }

        public static void ShowSystemDefaultErrorMessage()
        {
            ShowSystemErrorMessage("DefaultErrorMessage");
        }

        public static void ShowSystemErrorMessage(string stringIdx, StringParam param = null)
        {
            var message = StringTableUtil.GetSystemMessage(stringIdx, param);

            ShowSystemMessage(message);

            SoundPlayer.PlayErrorSound();
        }

        public static void ShowItemGetOrUseMotion(Transform target, float offsetY, ItemInfo itemInfo)
        {
            Transform parent = MLand.GameManager.UIMotionParent;

            GameObject motionObj = MLand.ObjectPool.AcquireUI("ItemGetOrUseMotionUI", parent);

            motionObj.Localize();

            ItemGetOrUseMotionUI motionUI = motionObj.GetOrAddComponent<ItemGetOrUseMotionUI>();

            motionUI.Init(target, offsetY, itemInfo);
        }

        public static Vector3 GetCanMovePos()
        {
            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * (Mathf.Max(MLand.GameData.CameraCommonData.mapSizeX, MLand.GameData.CameraCommonData.mapSizeY));

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 25f, NavMesh.GetAreaFromName("Workable")))
            {
                return hit.position;
            }

            return new Vector3(
                UnityEngine.Random.Range(-5f, 5f),
                UnityEngine.Random.Range(-5f, 5f));
        }

        public static void PlayUpAppearMotion(RectTransform rectTm, float startPosY)
        {
            float orgPosX = rectTm.anchoredPosition.x;
            float orgPosY = rectTm.anchoredPosition.y;

            rectTm.anchoredPosition = new Vector3(orgPosX, startPosY);

            DOTween.Sequence()
                .Append(rectTm.DOAnchorPosY(orgPosY, 0.25f))
                .Append(rectTm.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.75f, elasticity: 0.25f));
        }

        public static void ShowBoxProbabilityTable(string boxId)
        {
            var boxData = MLand.GameData.BoxData.TryGet(boxId);

            Debug.Assert(boxData != null, $"{boxId} 없는 BoxData Id");

            ShowBoxProbabilityTable(boxData);
        }

        public static void ShowBoxProbabilityTable(BoxData boxData)
        {
            Popup_ProbabilityTableUI popup = MLand.PopupManager.CreatePopup<Popup_ProbabilityTableUI>();

            popup.Init(boxData);
        }

        public static void ShowMiniSizeSpeechBubble(Vector2 pos, string desc)
        {
            var popup = MLand.PopupManager.CreatePopup<Popup_SpeechBubbleUI>("Popup_MiniSizeSpeechBubble");

            popup.Init(pos, desc);
        }

        public static void ShowKingSlimePortraitMotion()
        {
            GameObject motionObj = Util.InstantiateUI("PortraitMotionUI", MLand.GameManager.UIMotionParent2);

            PortraitMotionUI motionUI = motionObj.GetOrAddComponent<PortraitMotionUI>();

            motionUI.Init();
        }

        public static void Localize(this GameObject go)
        {
            TMP_FontAsset fontAsset = MLand.Font.GetFont(MLand.SavePoint.LangCode);

            TextMeshProUGUI[] textMeshProArray = go.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true).ToArray();
            foreach (TextMeshProUGUI textMeshPro in textMeshProArray)
            {
                try
                {
                    if (textMeshPro.GetComponent<IgnoreLocalize>() != null)
                        continue;

                    if (textMeshPro.font == null)
                        continue;

                    if (textMeshPro.fontMaterial.name.IsValid() == false)
                        continue;

                    bool isNoOutline = textMeshPro.fontMaterial.name.Contains("NoOutline");

                    textMeshPro.font = fontAsset;
                    textMeshPro.fontMaterial = MLand.Font.GetMaterial(MLand.SavePoint.LangCode, !isNoOutline);
                }
                catch
                {
                    Debug.LogError(textMeshPro.name);
                }
            }
        }
    }

    public static class JsonUtil
    {
        // ======== Newton Json ========

        public static string ToJsonNewton(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static object FromJsonNewton(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static T Base64FromJsonNewton<T>(string base64)
        {
            string json = Base64StringToJsonNewton(base64);

            return (T)FromJsonNewton(json, typeof(T));
        }

        public static string JsonToBase64StringNewton(string json)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(json);

            string base64 = Convert.ToBase64String(utf8);

            return base64;
        }

        public static string Base64StringToJsonNewton(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            string json = Encoding.UTF8.GetString(bytes);

            return json;
        }

        // ======== Unity Json ========

        public static string ToJsonUnity(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public static T FromJsonUnity<T>(string json)
        {
            return (T)JsonUtility.FromJson(json, typeof(T));
        }

        public static T Base64FromJsonUnity<T>(string base64)
        {
            string json = Base64StringToJson(base64);

            return FromJsonUnity<T>(json);
        }

        public static string JsonToBase64String(string json)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(json);

            string base64 = Convert.ToBase64String(utf8);

            return base64;
        }

        public static string Base64StringToJson(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            string json = Encoding.UTF8.GetString(bytes);

            return json;
        }
    }
}