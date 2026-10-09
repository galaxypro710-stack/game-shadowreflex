using System;
using System.Collections;
using UnityEngine;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace Game.Features.Monetization
{
    /// <summary>
    /// Quản lý AdMob Monetization (Rewarded Ads & Interstitial Ads) theo chuẩn Standalone Offline-First.
    /// Hỗ trợ cả 2 chế độ:
    /// 1. Google Mobile Ads Unity Plugin Native (com.google.ads.mobile) khi có SDK.
    /// 2. Mock Simulation trên Windows Desktop Standalone / Editor (đếm ngược xem ad mô phỏng 2s, không crash).
    /// </summary>
    public class AdMobManager : MonoBehaviour, IAdService
    {
        public static AdMobManager Instance { get; private set; }

        [Header("AdMob Configuration")]
        [Tooltip("Google Official Sample Test Rewarded Unit ID for Android")]
        [SerializeField] private string _androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

        [Tooltip("Google Official Sample Test Interstitial Unit ID for Android")]
        [SerializeField] private string _androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

        [SerializeField] private bool _useLiveProduction = false;

        [Header("Simulation Settings (Windows / Editor)")]
        [SerializeField] private bool _enableMockOnDesktop = true;
        [SerializeField] private float _mockSimulationDuration = 2.0f;

#if GOOGLE_MOBILE_ADS
        private RewardedAd _rewardedAd;
        private InterstitialAd _interstitialAd;
#endif

        public bool UseLiveProduction
        {
            get => _useLiveProduction;
            set => _useLiveProduction = value;
        }

        public string RewardedAdUnitId
        {
            get => _androidRewardedAdUnitId;
            set => _androidRewardedAdUnitId = value;
        }

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

#if GOOGLE_MOBILE_ADS
            Debug.Log($"[AdMobManager] Khởi tạo Google Mobile Ads SDK thật. Rewarded Unit ID: {_androidRewardedAdUnitId}");
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                Debug.Log("[AdMobManager] Google Mobile Ads SDK đã sẵn sàng.");
                LoadRewardedAd();
            });
#else
            Debug.Log($"[AdMobManager] Chế độ Offline-First / Mock Simulation. Unit ID: {_androidRewardedAdUnitId}");
            _isRewardedAdReady = true;
#endif
        }

        public void LoadRewardedAd()
        {
#if GOOGLE_MOBILE_ADS
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            _isRewardedAdReady = false;
            Debug.Log($"[AdMobManager] Đang tải Rewarded Ad thật (ID: {_androidRewardedAdUnitId})...");

            var adRequest = new AdRequest();
            RewardedAd.Load(_androidRewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"[AdMobManager] Tải Rewarded Ad thất bại: {error?.GetMessage()}");
                    _isRewardedAdReady = false;
                    return;
                }

                _rewardedAd = ad;
                _isRewardedAdReady = true;
                Debug.Log("[AdMobManager] Rewarded Ad thật đã tải thành công!");
            });
#else
            _isRewardedAdReady = true;
#endif
        }

        public bool IsRewardedAdLoaded()
        {
#if GOOGLE_MOBILE_ADS
            return _rewardedAd != null && _rewardedAd.CanShowAd() && !_isShowingAd;
#else
            return _isRewardedAdReady && !_isShowingAd;
#endif
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

#if GOOGLE_MOBILE_ADS
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _isShowingAd = true;
                Debug.Log($"[AdMobManager] 🎬 Hiển thị Rewarded Video Ad thật từ Google SDK...");

                _rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    _isShowingAd = false;
                    onAdClosed?.Invoke();
                    LoadRewardedAd(); // Preload ad tiếp theo
                };

                _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    _isShowingAd = false;
                    string errMsg = error != null ? error.GetMessage() : "Unknown error";
                    Debug.LogError($"[AdMobManager] Lỗi hiển thị Rewarded Ad: {errMsg}");
                    onAdFailed?.Invoke(errMsg);
                    OnAdFailed?.Invoke(errMsg);
                    LoadRewardedAd();
                };

                _rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log($"[AdMobManager] 🏆 Nhận thưởng thành công: {reward.Type} - {reward.Amount}");
                    onRewardEarned?.Invoke();
                    OnAdRewardGranted?.Invoke();
                });
                return;
            }
#endif

            // Fallback sang coroutine mô phỏng offline-first
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
            Debug.Log($"[AdMobManager] 🎬 [Mô phỏng] Bắt đầu phát Rewarded Video Ad ({_androidRewardedAdUnitId})...");

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
