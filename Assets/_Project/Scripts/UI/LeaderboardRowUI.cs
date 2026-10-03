using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Thành phần hiển thị 1 dòng trên Bảng xếp hạng.
    /// Tách riêng file theo chuẩn Unity MonoBehaviour để đảm bảo có GUID .meta hợp lệ,
    /// tránh lỗi Missing Script và corrupt scene level0 trên Android IL2CPP Release.
    /// </summary>
    public class LeaderboardRowUI : MonoBehaviour
    {
        public Image rankTrophyImage;
        public Text rankText;
        public Text nameText;
        public Text scoreText;
        public Text levelText;
        public Image backgroundImage;

        public void Setup(int rank, string playerName, int score, int level, Sprite trophySprite, bool isCurrentPlayer)
        {
            if (rankTrophyImage != null)
            {
                if (trophySprite != null)
                {
                    rankTrophyImage.gameObject.SetActive(true);
                    rankTrophyImage.sprite = trophySprite;
                    if (rankText != null) rankText.gameObject.SetActive(false);
                }
                else
                {
                    rankTrophyImage.gameObject.SetActive(false);
                    if (rankText != null)
                    {
                        rankText.gameObject.SetActive(true);
                        rankText.text = $"#{rank}";
                    }
                }
            }
            else if (rankText != null)
            {
                rankText.gameObject.SetActive(true);
                rankText.text = $"#{rank}";
            }

            if (nameText != null)
            {
                nameText.text = isCurrentPlayer ? $"{playerName} (Bạn)" : playerName;
                nameText.color = isCurrentPlayer ? new Color(1f, 0.84f, 0f) : Color.white; // Gold highlight
            }

            if (scoreText != null)
            {
                scoreText.text = $"{score:N0}";
                scoreText.color = isCurrentPlayer ? new Color(1f, 0.84f, 0f) : new Color(0.9f, 0.9f, 0.9f);
            }

            if (levelText != null)
            {
                levelText.text = $"Màn {level}";
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = isCurrentPlayer 
                    ? new Color(0.8f, 0.6f, 0.1f, 0.35f) 
                    : (rank % 2 == 0 ? new Color(0.12f, 0.14f, 0.18f, 0.7f) : new Color(0.08f, 0.1f, 0.14f, 0.7f));
            }
        }
    }
}
