using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Game.Events;
using Game.Data;

namespace Game.UI
{
    /// <summary>
    /// Popup thông báo kết quả Chiến Thắng (Victory) hoặc Thất Bại (GameOver).
    /// Chuẩn Universal Core: Lắng nghe GameStateEventChannelSO, xử lý xem Ad hồi sinh,
    /// nộp điểm Leaderboard và chuyển màn. Hoàn toàn không phụ thuộc logic gameplay cụ thể.
    /// </summary>
    public class WinLosePopup : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _victoryPanel;
        [SerializeField] private GameObject _gameOverPanel;

        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _watchAdReviveButton;
        [SerializeField] private Button _buyNoAdsButton;
        [SerializeField] private Button _victoryLeaderboardButton;
        [SerializeField] private Button _gameOverLeaderboardButton;

        [Header("Popups")]
        [SerializeField] private LeaderboardPopup _leaderboardPopup;

        [Header("Event Channels")]
        [SerializeField] private GameStateEventChannelSO _onGameStateChangedChannel;
        [SerializeField] private VoidEventChannelSO _onRestartRequestedChannel;
        [SerializeField] private VoidEventChannelSO _onReviveTriggeredChannel;

        private void Awake()
        {
            if (_victoryPanel != null) _victoryPanel.SetActive(false);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
        }

        private void OnEnable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised += HandleGameStateChanged;
            }

            if (_nextLevelButton != null) _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            if (_retryButton != null) _retryButton.onClick.AddListener(OnRetryClicked);
            if (_watchAdReviveButton != null) _watchAdReviveButton.onClick.AddListener(OnWatchAdReviveClicked);
            if (_buyNoAdsButton != null) _buyNoAdsButton.onClick.AddListener(OnBuyNoAdsClicked);
            if (_victoryLeaderboardButton != null) _victoryLeaderboardButton.onClick.AddListener(OnLeaderboardClicked);
            if (_gameOverLeaderboardButton != null) _gameOverLeaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        }

        private void OnDisable()
        {
            if (_onGameStateChangedChannel != null)
            {
                _onGameStateChangedChannel.OnEventRaised -= HandleGameStateChanged;
            }

            if (_nextLevelButton != null) _nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
            if (_retryButton != null) _retryButton.onClick.RemoveListener(OnRetryClicked);
            if (_watchAdReviveButton != null) _watchAdReviveButton.onClick.RemoveListener(OnWatchAdReviveClicked);
            if (_buyNoAdsButton != null) _buyNoAdsButton.onClick.RemoveListener(OnBuyNoAdsClicked);
            if (_victoryLeaderboardButton != null) _victoryLeaderboardButton.onClick.RemoveListener(OnLeaderboardClicked);
            if (_gameOverLeaderboardButton != null) _gameOverLeaderboardButton.onClick.RemoveListener(OnLeaderboardClicked);
        }

        private void HandleGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Victory:
                    if (_victoryPanel != null) _victoryPanel.SetActive(true);
                    if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
                    SubmitScoreToLeaderboard();
                    break;

                case GameState.GameOver:
                    if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
                    if (_victoryPanel != null) _victoryPanel.SetActive(false);
                    SubmitScoreToLeaderboard();
                    break;

                case GameState.Playing:
                case GameState.Boot:
                case GameState.MainMenu:
                    if (_victoryPanel != null) _victoryPanel.SetActive(false);
                    if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
                    break;

                default:
                    break;
            }
        }

        private void OnWatchAdReviveClicked()
        {
            if (Game.Features.Monetization.AdMobManager.Instance != null)
            {
                Game.Features.Monetization.AdMobManager.Instance.ShowRewardedAd(
                    onRewardEarned: () =>
                    {
                        TriggerReviveSuccess();
                        Debug.Log("[WinLosePopup] Thưởng thành công: Kích hoạt hồi sinh ván đấu!");
                    },
                    onAdFailed: (error) =>
                    {
                        Debug.LogWarning($"[WinLosePopup] Ad không hiển thị được ({error}), kích hoạt fallback hồi sinh!");
                        TriggerReviveSuccess();
                    }
                );
            }
            else
            {
                TriggerReviveSuccess();
            }
        }

        private void TriggerReviveSuccess()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_onReviveTriggeredChannel != null) _onReviveTriggeredChannel.RaiseEvent();
            if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Playing);
        }

        private void OnNextLevelClicked()
        {
            if (_victoryPanel != null) _victoryPanel.SetActive(false);

            if (SaveService.Instance != null)
            {
                var save = SaveService.Instance.LoadGame() ?? new GameSaveData();
                save.currentLevel++;
                SaveService.Instance.SaveGame(save);
                Debug.Log($"[WinLosePopup] Tiến lên màn tiếp theo: Level {save.currentLevel}");
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnRetryClicked()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            Debug.Log("[WinLosePopup] Người chơi chọn Thử lại màn hiện tại...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnBuyNoAdsClicked()
        {
            if (Game.Features.Monetization.RevenueCatIAPManager.Instance != null)
            {
                Game.Features.Monetization.RevenueCatIAPManager.Instance.PurchaseProduct(
                    Game.Features.Monetization.RevenueCatIAPManager.ProductNoAds,
                    onSuccess: () =>
                    {
                        Debug.Log("[WinLosePopup] 🚫 Mua gói No-Ads thành công! Quảng cáo đã được gỡ bỏ.");
                        if (SaveService.Instance != null) SaveService.Instance.SetNoAdsPurchased();
                        if (_buyNoAdsButton != null) _buyNoAdsButton.interactable = false;
                    },
                    onFailure: (err) =>
                    {
                        Debug.LogWarning($"[WinLosePopup] Giao dịch mua No-Ads thất bại: {err}");
                    }
                );
            }
        }

        private void SubmitScoreToLeaderboard()
        {
            int currentScore = 0;
            int currentLevel = 1;

            if (SaveService.Instance != null)
            {
                var save = SaveService.Instance.LoadGame();
                if (save != null)
                {
                    currentScore = save.score;
                    currentLevel = save.currentLevel;
                }
            }

            if (Game.Features.Leaderboard.SupabaseLeaderboardManager.Instance != null)
            {
                Game.Features.Leaderboard.SupabaseLeaderboardManager.Instance.SubmitScore(
                    Game.Features.Leaderboard.SupabaseLeaderboardManager.Instance.CurrentPlayerGamerTag,
                    currentScore,
                    currentLevel
                );
            }
        }

        private void OnLeaderboardClicked()
        {
            if (_leaderboardPopup != null)
            {
                _leaderboardPopup.Open();
            }
        }
    }
}
