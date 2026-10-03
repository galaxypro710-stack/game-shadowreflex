using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Features.Leaderboard;
using Game.Data;

namespace Game.UI
{
    /// <summary>
    /// Popup Bảng Xếp Hạng Toàn Cầu (Global Leaderboard Popup).
    /// Hiển thị Top 10 kỷ lục điểm số toàn cầu với phong cách Hoàng Gia Ai Cập:
    /// - Cúp Vàng (Top 1), Cúp Bạc (Top 2), Cúp Đồng (Top 3).
    /// - Hiển thị GamerTag, Điểm số, Màn chơi đạt được.
    /// - Banner riêng cho thứ hạng của người chơi hiện tại kèm Điểm Kỷ Lục HMAC-SHA256.
    /// - Nút Làm Mới (Refresh) và Trạng thái kết nối Cloud / Offline-First.
    /// </summary>
    public class LeaderboardPopup : MonoBehaviour
    {
        [Header("Header Elements")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _statusText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _refreshButton;

        [Header("Player Summary Banner")]
        [SerializeField] private Text _myGamerTagText;
        [SerializeField] private Text _myRankText;
        [SerializeField] private Text _myHighScoreText;

        [Header("Entries Container (10 Rows)")]
        [SerializeField] private Transform _rowsContainer;
        [SerializeField] private LeaderboardRowUI[] _fixedRows;

        [Header("Trophy Sprites")]
        [SerializeField] private Sprite _goldTrophySprite;
        [SerializeField] private Sprite _silverTrophySprite;
        [SerializeField] private Sprite _bronzeTrophySprite;

        private void Awake()
        {
            if (_closeButton != null) _closeButton.onClick.AddListener(Close);
            if (_refreshButton != null) _refreshButton.onClick.AddListener(RefreshLeaderboard);
        }

        private void OnEnable()
        {
            RefreshLeaderboard();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            RefreshLeaderboard();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void RefreshLeaderboard()
        {
            if (_statusText != null) _statusText.text = "Đang đồng bộ dữ liệu...";

            var mgr = SupabaseLeaderboardManager.Instance;
            if (mgr == null)
            {
                if (_statusText != null) _statusText.text = "🟡 Ngoại tuyến (Offline-First)";
                return;
            }

            mgr.FetchTopScores(10, entries =>
            {
                DisplayEntries(entries);
                if (_statusText != null)
                {
                    _statusText.text = mgr.IsOnline ? "🟢 Supabase Cloud (Trực tuyến)" : "🟡 Offline-First Cache (Ngoại tuyến)";
                }
            }, error =>
            {
                if (_statusText != null) _statusText.text = "🟡 Chế độ Ngoại tuyến (Đã lưu)";
            });

            UpdatePlayerSummary();
        }

        private void DisplayEntries(List<LeaderboardEntry> entries)
        {
            if (entries == null) return;

            string myTag = SupabaseLeaderboardManager.Instance != null 
                ? SupabaseLeaderboardManager.Instance.CurrentPlayerGamerTag 
                : string.Empty;

            if (_fixedRows != null && _fixedRows.Length > 0)
            {
                for (int i = 0; i < _fixedRows.Length; i++)
                {
                    if (i < entries.Count)
                    {
                        var entry = entries[i];
                        bool isMe = string.Equals(entry.player_name, myTag, StringComparison.OrdinalIgnoreCase);
                        Sprite trophy = null;
                        if (i == 0) trophy = _goldTrophySprite;
                        else if (i == 1) trophy = _silverTrophySprite;
                        else if (i == 2) trophy = _bronzeTrophySprite;

                        _fixedRows[i].Setup(entry.rank, entry.player_name, entry.score, entry.level, trophy, isMe);
                        _fixedRows[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        _fixedRows[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void UpdatePlayerSummary()
        {
            var mgr = SupabaseLeaderboardManager.Instance;
            string gamerTag = mgr != null ? mgr.CurrentPlayerGamerTag : "Pharaoh_Player";
            int highScore = SaveService.Instance != null ? SaveService.Instance.GetHighScore() : PlayerPrefs.GetInt(SaveService.HighScoreKey, 0);

            if (_myGamerTagText != null) _myGamerTagText.text = $"👑 {gamerTag}";
            if (_myHighScoreText != null) _myHighScoreText.text = $"Kỷ lục: {highScore:N0}";
            if (_myRankText != null) _myRankText.text = "Hạng: TOP";
        }
    }
}
