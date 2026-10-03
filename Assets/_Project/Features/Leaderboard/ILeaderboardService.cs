using System;
using System.Collections.Generic;

namespace Game.Features.Leaderboard
{
    /// <summary>
    /// Bản ghi điểm số trên Bảng xếp hạng toàn cầu.
    /// Hỗ trợ chuẩn hóa dữ liệu giữa Supabase REST API và Local Cache.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string player_name;
        public int score;
        public int level;
        public string created_at;

        public LeaderboardEntry() { }

        public LeaderboardEntry(int rank, string playerName, int score, int level, string createdAt)
        {
            this.rank = rank;
            this.player_name = playerName;
            this.score = score;
            this.level = level;
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
        void SubmitScore(string playerName, int score, int level, Action<bool> onComplete = null);
    }
}
