using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.Leaderboard
{
    /// <summary>
    /// Bản ghi điểm số trên Bảng xếp hạng toàn cầu.
    /// Khớp chính xác với cấu trúc bảng Supabase: id, gamertag, score, wave, created_at.
    /// Hỗ trợ chuẩn hóa dữ liệu giữa Supabase REST API và Local Cache.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public long id;
        public int rank;
        public string gamertag;
        public int score;
        public int wave;
        public string created_at;

        [SerializeField] private string player_name_legacy;
        [SerializeField] private int level_legacy;

        // Thuộc tính tương thích ngược cho UI
        public string player_name => !string.IsNullOrEmpty(gamertag) ? gamertag : player_name_legacy;
        public int level => wave > 0 ? wave : level_legacy;

        public LeaderboardEntry() { }

        public LeaderboardEntry(int rank, string gamertag, int score, int wave, string createdAt, long id = 0)
        {
            this.id = id;
            this.rank = rank;
            this.gamertag = gamertag;
            this.player_name_legacy = gamertag;
            this.score = score;
            this.wave = wave;
            this.level_legacy = wave;
            this.created_at = createdAt;
        }
    }

    [Serializable]
    public class LeaderboardDataWrapper
    {
        public List<LeaderboardEntry> items = new List<LeaderboardEntry>();
    }

    /// <summary>
    /// Giao diện dịch vụ Bảng xếp hạng toàn cầu (Global Leaderboard Interface).
    /// Tuân thủ Dependency Inversion Principle: Hỗ trợ linh hoạt cả Supabase REST API và Local Offline-First Cache.
    /// </summary>
    public interface ILeaderboardService
    {
        bool IsOnline { get; }
        string CurrentPlayerGamerTag { get; }
        void FetchTopScores(int limit, Action<List<LeaderboardEntry>> onSuccess, Action<string> onFailure = null);
        void SubmitScore(string gamertag, int score, int wave, Action<bool> onComplete = null);
    }
}
