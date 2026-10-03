using System;
using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// Cấu trúc dữ liệu lưu trữ trạng thái tổng quát chuẩn Universal Core (GameSaveData).
    /// Độc lập 100% với thể loại gameplay, hỗ trợ customPayload dạng JSON cho subgame.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public const int SaveVersion = 1;

        public int version = SaveVersion;
        public int currentLevel = 1;
        public int score = 0;
        public int highScore = 0;
        public int coins = 0;
        public bool hasPurchasedNoAds = false;
        
        /// <summary>
        /// Chuỗi JSON tùy biến dành riêng cho từng subgame con (Decoupled Payload).
        /// </summary>
        public string customPayload = "";

        public GameSaveData() { }

        public GameSaveData(int level, int score, int highScore, int coins)
        {
            this.currentLevel = level;
            this.score = score;
            this.highScore = highScore;
            this.coins = coins;
            this.hasPurchasedNoAds = false;
            this.customPayload = "";
        }
    }
}
