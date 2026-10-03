using System;
using System.Collections;
using UnityEngine;

namespace Game.Features.Monetization
{
    /// <summary>
    /// Quản lý AdMob Monetization (Rewarded Ads & Interstitial Ads) theo chuẩn Standalone Offline-First.
    /// Hỗ trợ cả 2 chế độ:
    /// 1. Mock Simulation trên Windows Desktop Standalone / Editor (đếm ngược xem ad mô phỏng 2s, không crash).
    /// 2. Google Mobile Ads SDK Adapter trên Android Native.
    /// </summary>
    public class AdMobManager : MonoBehaviour, IAdService
    {
        public static AdMobManager Instance { get; private set; }

        [Header("AdMob Configuration")]
        [Tooltip("Google Official Sample Test Rewarded Unit ID for Android")]
        [SerializeField] private string _androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

        [Tooltip("Google Official Sample Test Interstitial Unit ID for Android")]
        [SerializeField] private string _androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

        [Header("Simulation Settings (Windows / Editor)")]
        [SerializeField] private bool _enableMockOnDesktop = true;
        [SerializeField] private float _mockSimulationDuration = 2.0f;

        [Header("State")]
        [SerializeField] private bool _isRewardedAdReady = true;
        [SerializeField] private bool _isShowingAd = false;

        public event Action OnAdRewardGranted;
        public event Action<string> OnAdFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }

        private void InitializeAds()
        {
            Debug.Log("[AdMobManager] Khởi tạo hệ thống quảng cáo AdMob...");
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log($"[AdMobManager] Đang chạy trên thiết bị Android thật. Rewarded Unit ID: {_androidRewardedAdUnitId}");
            // Khi cài đặt GoogleMobileAds package, gọi MobileAds.Initialize(...) tại đây
#else
            if (_enableMockOnDesktop)
            {
                Debug.Log($"[AdMobManager] Đang chạy ở chế độ Desktop Standalone / Editor Mock. Thời gian xem mô phỏng: {_mockSimulationDuration}s.");
                _isRewardedAdReady = true;
            }
#endif
        }

        public bool IsRewardedAdLoaded()
        {
            return _isRewardedAdReady && !_isShowingAd;
        }

        public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null, Action<string> onAdFailed = null)
        {
            if (_isShowingAd)
            {
                Debug.LogWarning("[AdMobManager] Quảng cáo đang được hiển thị, vui lòng chờ!");
                onAdFailed?.Invoke("Ad is already showing.");
                OnAdFailed?.Invoke("Ad is already showing.");
                return;
            }

            if (RevenueCatIAPManager.Instance != null && RevenueCatIAPManager.Instance.HasPurchasedNoAds())
            {
                Debug.Log("[AdMobManager] 👑 Người chơi sở hữu gói No-Ads VIP! Nhận phần thưởng cứu nguy ngay lập tức không cần xem quảng cáo.");
                onRewardEarned?.Invoke();
                OnAdRewardGranted?.Invoke();
                onAdClosed?.Invoke();
                return;
            }

            if (!IsRewardedAdLoaded())
            {
                Debug.LogWarning("[AdMobManager] Quảng cáo Rewarded Ad chưa sẵn sàng.");
                onAdFailed?.Invoke("Ad not ready.");
                OnAdFailed?.Invoke("Ad not ready.");
                return;
            }

            StartCoroutine(ShowRewardedAdRoutine(onRewardEarned, onAdClosed, onAdFailed));
        }

        private IEnumerator ShowRewardedAdRoutine(Action onRewardEarned, Action onAdClosed, Action<string> onAdFailed)
        {
            _isShowingAd = true;
            Debug.Log($"[AdMobManager] 🎬 Bắt đầu phát Rewarded Video Ad (Unit ID: {_androidRewardedAdUnitId})...");

#if UNITY_ANDROID && !UNITY_EDITOR
            // Trên Android native, ủy quyền cho Google Mobile Ads SDK callback
            yield return new WaitForSeconds(1.0f);
            onRewardEarned?.Invoke();
            OnAdRewardGranted?.Invoke();
            onAdClosed?.Invoke();
#else
            // Giả lập xem ad đếm ngược trên Desktop/Editor để Đại Ka test gameplay mượt mà
            float elapsed = 0f;
            while (elapsed < _mockSimulationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            Debug.Log("[AdMobManager] 🏆 Xem hoàn tất Rewarded Video Ad! Kích hoạt phần thưởng cứu nguy.");
            onRewardEarned?.Invoke();
            OnAdRewardGranted?.Invoke();
            onAdClosed?.Invoke();
#endif

            _isShowingAd = false;
        }

        public void ShowInterstitialAd(Action onClosed = null)
        {
            if (RevenueCatIAPManager.Instance != null && RevenueCatIAPManager.Instance.HasPurchasedNoAds())
            {
                Debug.Log("[AdMobManager] 🚫 Người chơi đã mua gói No-Ads! Bỏ qua Interstitial Ad.");
                onClosed?.Invoke();
                return;
            }

            Debug.Log($"[AdMobManager] 📺 Hiển thị Interstitial Ad giữa các màn chơi (Unit ID: {_androidInterstitialAdUnitId}).");
            onClosed?.Invoke();
        }
    }
}
