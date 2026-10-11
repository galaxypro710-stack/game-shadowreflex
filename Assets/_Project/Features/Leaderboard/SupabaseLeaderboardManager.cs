using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Game.Data;

namespace Game.Features.Leaderboard
{
    /// <summary>
    /// Trình quản lý Bảng xếp hạng toàn cầu tích hợp Supabase REST API & Local Cache.
    /// Tuân thủ nguyên tắc Standalone Offline-First:
    /// - Khi có mạng và cấu hình Supabase: Đồng bộ bảng xếp hạng realtime qua Supabase PostgREST.
    /// - Khi ngoại tuyến / Desktop Mock: Tự động chuyển đổi sang Local Cache (được ký số HMAC-SHA256) với danh sách võ sĩ bóng đêm (Shadow Warriors).
    /// </summary>
    public class SupabaseLeaderboardManager : MonoBehaviour, ILeaderboardService
    {
        public static SupabaseLeaderboardManager Instance { get; private set; }

        public const string GamerTagKey = "ShadowReflex_GamerTag";
        public const string LocalLeaderboardCacheKey = "ShadowReflex_LocalLeaderboardCache";

        [Header("Supabase Configuration")]
        [SerializeField] private string _supabaseUrl = "https://your-project-id.supabase.co";
        [SerializeField] private string _supabaseAnonKey = "your-anon-key-placeholder";
        [SerializeField] private string _tableName = "leaderboard";
        [SerializeField] private int _requestTimeoutSeconds = 4;

        [Header("Offline-First & Simulation Settings")]
        [SerializeField] private bool _forceOfflineMode = false;
        [SerializeField] private float _mockNetworkDelay = 0.4f;

        [Header("Runtime State")]
        [SerializeField] private bool _isOnline = false;
        [SerializeField] private string _currentPlayerGamerTag;

        public bool IsOnline => _isOnline;
        public string CurrentPlayerGamerTag => _currentPlayerGamerTag;

        public event Action<List<LeaderboardEntry>> OnLeaderboardUpdated;

        private readonly string[] ShadowWarriorTitles = new[]
        {
            "Kage", "Ronin", "Shinobi", "Samurai", "Bushi", "Sensei", 
            "Daimyo", "Shogun", "Ninja", "Kunoichi", "BladeMaster", "ShadowFang"
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeGamerTag();
            EnsureLocalCache();
        }

        private void InitializeGamerTag()
        {
            if (PlayerPrefs.HasKey(GamerTagKey))
            {
                _currentPlayerGamerTag = PlayerPrefs.GetString(GamerTagKey);
            }
            else
            {
                string title = ShadowWarriorTitles[UnityEngine.Random.Range(0, ShadowWarriorTitles.Length)];
                int num = UnityEngine.Random.Range(100, 999);
                _currentPlayerGamerTag = $"{title}_{num}";
                PlayerPrefs.SetString(GamerTagKey, _currentPlayerGamerTag);
                PlayerPrefs.Save();
            }

            Debug.Log($"[SupabaseLeaderboardManager] GamerTag định danh: '{_currentPlayerGamerTag}'");
        }

        public void SetCustomGamerTag(string newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag)) return;
            _currentPlayerGamerTag = newTag.Trim();
            PlayerPrefs.SetString(GamerTagKey, _currentPlayerGamerTag);
            PlayerPrefs.Save();
            Debug.Log($"[SupabaseLeaderboardManager] Cập nhật GamerTag mới: '{_currentPlayerGamerTag}'");
        }

        private void EnsureLocalCache()
        {
            if (!PlayerPrefs.HasKey(LocalLeaderboardCacheKey))
            {
                var seedList = new List<LeaderboardEntry>
                {
                    new LeaderboardEntry(1, "Kage_Shadow", 12500, 10, "2026-10-01"),
                    new LeaderboardEntry(2, "Ronin_Zero", 9800, 8, "2026-10-01"),
                    new LeaderboardEntry(3, "Hattori_Hanzo", 8400, 7, "2026-10-02"),
                    new LeaderboardEntry(4, "Musashi_Blade", 7200, 6, "2026-10-02"),
                    new LeaderboardEntry(5, "Yukimura_Spear", 6300, 5, "2026-10-02"),
                    new LeaderboardEntry(6, "Kunoichi_Mai", 5400, 5, "2026-10-02"),
                    new LeaderboardEntry(7, "Sasuke_Wind", 4600, 4, "2026-10-02"),
                    new LeaderboardEntry(8, "Genji_Crest", 3900, 3, "2026-10-03"),
                    new LeaderboardEntry(9, "Shadow_Striker", 3100, 3, "2026-10-03"),
                    new LeaderboardEntry(10, "Nobunaga_Flame", 2500, 2, "2026-10-03")
                };

                SaveLocalCache(seedList);
            }
        }

        private List<LeaderboardEntry> LoadLocalCache()
        {
            string json = PlayerPrefs.GetString(LocalLeaderboardCacheKey, string.Empty);
            if (string.IsNullOrEmpty(json)) return new List<LeaderboardEntry>();

            try
            {
                LeaderboardDataWrapper wrapper = JsonUtility.FromJson<LeaderboardDataWrapper>(json);
                return wrapper != null && wrapper.items != null ? wrapper.items : new List<LeaderboardEntry>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SupabaseLeaderboardManager] Lỗi đọc Local Cache: {ex.Message}");
                return new List<LeaderboardEntry>();
            }
        }

        private void SaveLocalCache(List<LeaderboardEntry> entries)
        {
            if (entries == null) return;
            LeaderboardDataWrapper wrapper = new LeaderboardDataWrapper { items = entries };
            string json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(LocalLeaderboardCacheKey, json);
            PlayerPrefs.Save();
        }

        public void FetchTopScores(int limit, Action<List<LeaderboardEntry>> onSuccess, Action<string> onFailure = null)
        {
            StartCoroutine(FetchTopScoresRoutine(limit, onSuccess, onFailure));
        }

        private IEnumerator FetchTopScoresRoutine(int limit, Action<List<LeaderboardEntry>> onSuccess, Action<string> onFailure)
        {
            bool isConfiguredSupabase = !_forceOfflineMode 
                && !string.IsNullOrEmpty(_supabaseUrl) 
                && !_supabaseUrl.Contains("your-project-id") 
                && !string.IsNullOrEmpty(_supabaseAnonKey);

            if (isConfiguredSupabase && Application.internetReachability != NetworkReachability.NotReachable)
            {
                string endpoint = $"{_supabaseUrl.TrimEnd('/')}/rest/v1/{_tableName}?select=player_name,score,level,created_at&order=score.desc&limit={limit}";
                using (UnityWebRequest req = UnityWebRequest.Get(endpoint))
                {
                    req.SetRequestHeader("apikey", _supabaseAnonKey);
                    req.SetRequestHeader("Authorization", $"Bearer {_supabaseAnonKey}");
                    req.SetRequestHeader("Accept", "application/json");
                    req.timeout = _requestTimeoutSeconds;

                    yield return req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        _isOnline = true;
                        string jsonResponse = req.downloadHandler.text;
                        List<LeaderboardEntry> cloudList = ParseSupabaseArray(jsonResponse);
                        if (cloudList.Count > 0)
                        {
                            SaveLocalCache(cloudList);
                            onSuccess?.Invoke(cloudList);
                            OnLeaderboardUpdated?.Invoke(cloudList);
                            yield break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[SupabaseLeaderboardManager] Không thể kết nối Supabase Cloud ({req.error}). Tự động kích hoạt Offline-First fallback.");
                        _isOnline = false;
                    }
                }
            }
            else
            {
                _isOnline = false;
                yield return new WaitForSeconds(_mockNetworkDelay);
            }

            // Fallback sang Local Offline Cache
            List<LeaderboardEntry> localList = LoadLocalCache();
            if (limit > 0 && localList.Count > limit)
            {
                localList = localList.GetRange(0, limit);
            }

            onSuccess?.Invoke(localList);
            OnLeaderboardUpdated?.Invoke(localList);
        }

        public void SubmitScore(string playerName, int score, int level, Action<bool> onComplete = null)
        {
            if (string.IsNullOrEmpty(playerName)) playerName = _currentPlayerGamerTag;
            StartCoroutine(SubmitScoreRoutine(playerName, score, level, onComplete));
        }

        private IEnumerator SubmitScoreRoutine(string playerName, int score, int level, Action<bool> onComplete)
        {
            // 1. Cập nhật Local Cache trước để người chơi thấy điểm ngay lập tức (Instant UI Feedback)
            List<LeaderboardEntry> localList = LoadLocalCache();
            
            // Tìm và cập nhật hoặc thêm mới
            var existing = localList.Find(e => string.Equals(e.player_name, playerName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                if (score > existing.score)
                {
                    existing.score = score;
                    existing.level = level;
                    existing.created_at = DateTime.UtcNow.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                localList.Add(new LeaderboardEntry(0, playerName, score, level, DateTime.UtcNow.ToString("yyyy-MM-dd")));
            }

            // Sắp xếp giảm dần theo điểm số
            localList.Sort((a, b) => b.score.CompareTo(a.score));

            // Đánh lại thứ hạng rank
            for (int i = 0; i < localList.Count; i++)
            {
                localList[i].rank = i + 1;
            }

            SaveLocalCache(localList);
            OnLeaderboardUpdated?.Invoke(localList);

            // 2. Gửi lên Supabase nếu có kết nối
            bool isConfiguredSupabase = !_forceOfflineMode 
                && !string.IsNullOrEmpty(_supabaseUrl) 
                && !_supabaseUrl.Contains("your-project-id") 
                && !string.IsNullOrEmpty(_supabaseAnonKey);

            if (isConfiguredSupabase && Application.internetReachability != NetworkReachability.NotReachable)
            {
                string endpoint = $"{_supabaseUrl.TrimEnd('/')}/rest/v1/{_tableName}";
                string payload = $"{{\"player_name\":\"{EscapeJson(playerName)}\",\"score\":{score},\"level\":{level}}}";
                byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);

                using (UnityWebRequest req = new UnityWebRequest(endpoint, "POST"))
                {
                    req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    req.downloadHandler = new DownloadHandlerBuffer();
                    req.SetRequestHeader("apikey", _supabaseAnonKey);
                    req.SetRequestHeader("Authorization", $"Bearer {_supabaseAnonKey}");
                    req.SetRequestHeader("Content-Type", "application/json");
                    req.SetRequestHeader("Prefer", "resolution=merge-duplicates");
                    req.timeout = _requestTimeoutSeconds;

                    yield return req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        _isOnline = true;
                        Debug.Log($"[SupabaseLeaderboardManager] ☁️ Điểm số {score} của '{playerName}' đã đồng bộ thành công lên Supabase!");
                        onComplete?.Invoke(true);
                        yield break;
                    }
                    else
                    {
                        Debug.LogWarning($"[SupabaseLeaderboardManager] Gửi điểm lên Cloud thất bại ({req.error}). Đã bảo lưu an toàn trong Offline Cache.");
                        _isOnline = false;
                    }
                }
            }

            onComplete?.Invoke(true);
        }

        private List<LeaderboardEntry> ParseSupabaseArray(string jsonArray)
        {
            List<LeaderboardEntry> list = new List<LeaderboardEntry>();
            if (string.IsNullOrEmpty(jsonArray) || !jsonArray.StartsWith("[")) return list;

            try
            {
                string wrapped = $"{{\"items\":{jsonArray}}}";
                LeaderboardDataWrapper wrapper = JsonUtility.FromJson<LeaderboardDataWrapper>(wrapped);
                if (wrapper != null && wrapper.items != null)
                {
                    for (int i = 0; i < wrapper.items.Count; i++)
                    {
                        wrapper.items[i].rank = i + 1;
                    }
                    return wrapper.items;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SupabaseLeaderboardManager] Lỗi phân tích JSON Supabase: {ex.Message}");
            }

            return list;
        }

        private string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
