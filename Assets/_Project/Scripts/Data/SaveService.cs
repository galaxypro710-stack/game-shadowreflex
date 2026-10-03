using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// Thuáº­t toÃ¡n chuyá»ƒn Ä‘á»•i JSON thuáº§n tÃºy (Pure Logic) cho há»‡ thá»‘ng lÆ°u trá»¯.
    /// Cho phÃ©p Unit Testing Ä‘á»™c láº­p khÃ´ng phá»¥ thuá»™c vÃ o PlayerPrefs cá»§a Unity.
    /// </summary>
    public static class SaveSerializer
    {
        public static string ToJson(GameSaveData data, bool prettyPrint = false)
        {
            if (data == null) return string.Empty;
            return JsonUtility.ToJson(data, prettyPrint);
        }

        public static GameSaveData FromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonUtility.FromJson<GameSaveData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSerializer] Lá»—i phÃ¢n tÃ­ch JSON SaveData: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Dá»‹ch vá»¥ quáº£n trá»‹ lÆ°u trá»¯ tráº¡ng thÃ¡i trÃ² chÆ¡i xuá»‘ng PlayerPrefs (hoáº·c Cloud Save).
    /// TÃ­ch há»£p cÆ¡ cháº¿ mÃ£ hÃ³a xÃ¡c thá»±c toÃ n váº¹n dá»¯ liá»‡u báº±ng chá»¯ kÃ½ sá»‘ HMAC-SHA256 chá»‘ng hack Ä‘iá»ƒm/F5.
    /// Chuáº©n Universal Core: KhÃ´ng phá»¥ thuá»™c vÃ o báº¥t ká»³ thá»ƒ loáº¡i gameplay nÃ o.
    /// </summary>
    public class SaveService : MonoBehaviour
    {
        public const string SaveKey = "GameCore_SavedData";
        public const string HighScoreKey = "GameCore_HighScore";
        public const string CoinsKey = "GameCore_Coins";
        public const string NoAdsKey = "GameCore_NoAds";

        // Salt máº·c Ä‘á»‹nh cá»§a Template Core (sáº½ Ä‘Æ°á»£c scaffold_new_game.ps1 sinh ngáº«u nhiÃªn cho tá»«ng game con)
        private static readonly byte[] HmacSalt = Encoding.UTF8.GetBytes("ShadowReflex_SecretSalt_a2abd944225b40c79ab61956154f42d9_v1");

        public static SaveService Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Táº¡o chá»¯ kÃ½ sá»‘ HMAC-SHA256 cho chuá»—i dá»¯ liá»‡u Ä‘áº§u vÃ o.
        /// </summary>
        public static string ComputeHmac(string payload)
        {
            if (string.IsNullOrEmpty(payload)) return string.Empty;
            using (var hmac = new HMACSHA256(HmacSalt))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Kiá»ƒm tra tÃ­nh há»£p lá»‡ cá»§a chá»¯ kÃ½ HMAC Ä‘á»‘i chiáº¿u vá»›i dá»¯ liá»‡u.
        /// </summary>
        public static bool VerifyHmac(string payload, string storedSignature)
        {
            if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(storedSignature)) return false;
            string computed = ComputeHmac(payload);
            return string.Equals(computed, storedSignature, StringComparison.Ordinal);
        }

        /// <summary>
        /// LÆ°u tráº¡ng thÃ¡i phiÃªn chÆ¡i hiá»‡n táº¡i kÃ¨m chá»¯ kÃ½ HMAC báº£o vá»‡.
        /// </summary>
        public void SaveGame(GameSaveData data)
        {
            if (data == null) return;

            string json = SaveSerializer.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.SetString(SaveKey + "_HMAC", ComputeHmac(json));

            // Cáº­p nháº­t HighScore náº¿u vÆ°á»£t ká»· lá»¥c
            int currentHigh = GetHighScore();
            if (data.score > currentHigh)
            {
                SetSecureInt(HighScoreKey, data.score);
            }

            // Äá»“ng bá»™ coins
            SetSecureInt(CoinsKey, data.coins);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Náº¡p láº¡i tráº¡ng thÃ¡i Ä‘Ã£ lÆ°u sau khi xÃ¡c thá»±c chá»¯ kÃ½ HMAC toÃ n váº¹n.
        /// </summary>
        public GameSaveData LoadGame()
        {
            if (!HasSavedGame()) return null;

            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            string sig = PlayerPrefs.GetString(SaveKey + "_HMAC", string.Empty);

            if (!VerifyHmac(json, sig))
            {
                Debug.LogWarning("[SaveService TAMPER DETECTED] Chá»¯ kÃ½ HMAC khÃ´ng khá»›p hoáº·c dá»¯ liá»‡u bá»‹ can thiá»‡p trÃ¡i phÃ©p! Tá»± Ä‘á»™ng há»§y save.");
                ClearSaveData();
                return null;
            }

            return SaveSerializer.FromJson(json);
        }

        /// <summary>
        /// Kiá»ƒm tra cÃ³ vÃ¡n Ä‘áº¥u nÃ o Ä‘ang lÆ°u dá»Ÿ dang hay khÃ´ng.
        /// </summary>
        public bool HasSavedGame()
        {
            return PlayerPrefs.HasKey(SaveKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(SaveKey));
        }

        /// <summary>
        /// XÃ³a tráº¡ng thÃ¡i vÃ¡n chÆ¡i khi vÃ¡n Ä‘áº¥u káº¿t thÃºc (Tháº¯ng hoáº·c Thua).
        /// </summary>
        public void ClearSaveData()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                PlayerPrefs.DeleteKey(SaveKey);
                PlayerPrefs.DeleteKey(SaveKey + "_HMAC");
                PlayerPrefs.Save();
            }
        }

        public int GetHighScore()
        {
            return GetSecureInt(HighScoreKey, 0);
        }

        public int GetCoins()
        {
            return GetSecureInt(CoinsKey, 0);
        }

        public void AddCoins(int amount)
        {
            SetSecureInt(CoinsKey, GetCoins() + amount);
        }

        public void SetSecureInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.SetString(key + "_HMAC", ComputeHmac(value.ToString()));
            PlayerPrefs.Save();
        }

        public int GetSecureInt(string key, int defaultValue = 0)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                SetSecureInt(key, defaultValue);
                return defaultValue;
            }

            int value = PlayerPrefs.GetInt(key, defaultValue);
            string sig = PlayerPrefs.GetString(key + "_HMAC", string.Empty);

            if (string.IsNullOrEmpty(sig) || !VerifyHmac(value.ToString(), sig))
            {
                Debug.LogWarning($"[SaveService TAMPER DETECTED] GiÃ¡ trá»‹ '{key}' bá»‹ can thiá»‡p trÃ¡i phÃ©p hoáº·c thiáº¿u chá»¯ kÃ½ HMAC! Reset vá» {defaultValue}.");
                SetSecureInt(key, defaultValue);
                return defaultValue;
            }

            return value;
        }

        public bool HasPurchasedNoAds()
        {
            return GetSecureInt(NoAdsKey, 0) == 1;
        }

        public void SetNoAdsPurchased()
        {
            SetSecureInt(NoAdsKey, 1);
        }
    }
}

