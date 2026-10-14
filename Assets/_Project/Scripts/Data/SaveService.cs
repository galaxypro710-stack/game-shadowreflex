using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// Thuật toán chuyển đổi JSON thuần túy (Pure Logic) cho hệ thống lưu trữ.
    /// Cho phép Unit Testing độc lập không phụ thuộc vào PlayerPrefs của Unity.
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
                Debug.LogError($"[SaveSerializer] Lỗi phân tích JSON SaveData: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Dịch vụ quản trị lưu trữ trạng thái trò chơi xuống PlayerPrefs (hoặc Cloud Save).
    /// Tích hợp cơ chế mã hóa xác thực toàn vẹn dữ liệu bằng chữ ký số HMAC-SHA256 chống hack điểm/F5.
    /// Chuẩn Universal Core: Không phụ thuộc vào bất kỳ thể loại gameplay nào.
    /// </summary>
    public class SaveService : MonoBehaviour
    {
        public const string SaveKey = "ShadowReflex_SavedData";
        public const string HighScoreKey = "ShadowReflex_HighScore";
        public const string CoinsKey = "ShadowReflex_Coins";
        public const string NoAdsKey = "ShadowReflex_NoAds";

        // Salt mặc định của Template Core (sẽ được scaffold_new_game.ps1 sinh ngẫu nhiên cho từng game con)
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
        /// Tạo chữ ký số HMAC-SHA256 cho chuỗi dữ liệu đầu vào.
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
        /// Kiểm tra tính hợp lệ của chữ ký HMAC đối chiếu với dữ liệu.
        /// </summary>
        public static bool VerifyHmac(string payload, string storedSignature)
        {
            if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(storedSignature)) return false;
            string computed = ComputeHmac(payload);
            return string.Equals(computed, storedSignature, StringComparison.Ordinal);
        }

        /// <summary>
        /// Lưu trạng thái phiên chơi hiện tại kèm chữ ký HMAC bảo vệ.
        /// </summary>
        public void SaveGame(GameSaveData data)
        {
            if (data == null) return;

            string json = SaveSerializer.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.SetString(SaveKey + "_HMAC", ComputeHmac(json));

            // Cập nhật HighScore nếu vượt kỷ lục
            int currentHigh = GetHighScore();
            if (data.score > currentHigh)
            {
                SetSecureInt(HighScoreKey, data.score);
            }

            // Đồng bộ coins
            SetSecureInt(CoinsKey, data.coins);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Nạp lại trạng thái đã lưu sau khi xác thực chữ ký HMAC toàn vẹn.
        /// </summary>
        public GameSaveData LoadGame()
        {
            if (!HasSavedGame()) return null;

            string json = PlayerPrefs.GetString(SaveKey, string.Empty);
            string sig = PlayerPrefs.GetString(SaveKey + "_HMAC", string.Empty);

            if (!VerifyHmac(json, sig))
            {
                Debug.LogWarning("[SaveService TAMPER DETECTED] Chữ ký HMAC không khớp hoặc dữ liệu bị can thiệp trái phép! Tự động hủy save.");
                ClearSaveData();
                return null;
            }

            return SaveSerializer.FromJson(json);
        }

        /// <summary>
        /// Kiểm tra có ván đấu nào đang lưu dở dang hay không.
        /// </summary>
        public bool HasSavedGame()
        {
            return PlayerPrefs.HasKey(SaveKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(SaveKey));
        }

        /// <summary>
        /// Xóa trạng thái ván chơi khi ván đấu kết thúc (Thắng hoặc Thua).
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
                Debug.LogWarning($"[SaveService TAMPER DETECTED] Giá trị '{key}' bị can thiệp trái phép hoặc thiếu chữ ký HMAC! Reset về {defaultValue}.");
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
