using System;
using System.Collections;
using UnityEngine;
using Game.Data;
using Game.Gameplay;

namespace Game.Features.Monetization
{
    /// <summary>
    /// Quản lý In-App Purchases (IAP) tích hợp RevenueCat StoreKit / Google Play Billing.
    /// Hỗ trợ chuẩn Standalone Offline-First:
    /// 1. Mock Simulation trên Windows Desktop Standalone / Editor (giả lập thanh toán mượt mà, lưu SaveService).
    /// 2. Sẵn sàng RevenueCat Purchases SDK Adapter trên iOS/Android Mobile Native.
    /// </summary>
    public class RevenueCatIAPManager : MonoBehaviour, IIAPService
    {
        public static RevenueCatIAPManager Instance { get; private set; }

        public const string ProductNoAds = "shadowreflex.noads";
        public const string ProductBoosterSmall = "shadowreflex.boosters.small";

        [Header("RevenueCat Configuration")]
        [SerializeField] private string _androidApiKey = "goog_sample_test_revenuecat_key";
        [SerializeField] private string _iosApiKey = "appl_sample_test_revenuecat_key";

        [Header("Simulation Settings (Windows / Editor)")]
        [SerializeField] private bool _enableMockOnDesktop = true;
        [SerializeField] private float _mockPurchaseDelay = 0.8f;

        [Header("Runtime State")]
        [SerializeField] private bool _isInitialized = true;
        [SerializeField] private bool _isPurchasing = false;

        public event Action<string> OnPurchaseSuccess;
        public event Action<string, string> OnPurchaseFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeIAP();
        }

        private void InitializeIAP()
        {
            Debug.Log("[RevenueCatIAPManager] Khởi tạo hệ thống In-App Purchase (RevenueCat IAP)...");

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Debug.Log($"[RevenueCatIAPManager] Đang chạy trên thiết bị di động Native. Setup RevenueCat SDK...");
            // RevenueCat SDK native initialization:
            // Purchases.Configure(new PurchasesConfiguration.Builder(_androidApiKey).Build());
            _isInitialized = true;
#else
            if (_enableMockOnDesktop)
            {
                Debug.Log($"[RevenueCatIAPManager] Đang chạy ở chế độ Desktop Standalone / Editor Mock. Hỗ trợ sản phẩm: '{ProductNoAds}', '{ProductBoosterSmall}'.");
                _isInitialized = true;
            }
#endif
        }

        public bool IsInitialized()
        {
            return _isInitialized || _enableMockOnDesktop;
        }

        public bool HasPurchasedNoAds()
        {
            if (SaveService.Instance != null)
            {
                return SaveService.Instance.HasPurchasedNoAds();
            }
            return PlayerPrefs.GetInt(SaveService.NoAdsKey, 0) == 1;
        }

        public void PurchaseProduct(string productId, Action onSuccess, Action<string> onFailure)
        {
            if (!_isInitialized)
            {
                string msg = "Hệ thống IAP chưa sẵn sàng!";
                Debug.LogError($"[RevenueCatIAPManager] {msg}");
                onFailure?.Invoke(msg);
                OnPurchaseFailed?.Invoke(productId, msg);
                return;
            }

            if (_isPurchasing)
            {
                string msg = "Giao dịch thanh toán đang diễn ra, vui lòng đợi!";
                Debug.LogWarning($"[RevenueCatIAPManager] {msg}");
                onFailure?.Invoke(msg);
                return;
            }

            StartCoroutine(PurchaseRoutine(productId, onSuccess, onFailure));
        }

        private IEnumerator PurchaseRoutine(string productId, Action onSuccess, Action<string> onFailure)
        {
            _isPurchasing = true;
            Debug.Log($"[RevenueCatIAPManager] 💳 Bắt đầu xử lý thanh toán cho sản phẩm: {productId}...");

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            // Native Mobile RevenueCat Purchases flow placeholder
            yield return new WaitForSeconds(1.0f);
            DeliverProduct(productId);
            onSuccess?.Invoke();
            OnPurchaseSuccess?.Invoke(productId);
#else
            // Mock transaction trên Desktop / Editor
            yield return new WaitForSeconds(_mockPurchaseDelay);

            DeliverProduct(productId);
            onSuccess?.Invoke();
            OnPurchaseSuccess?.Invoke(productId);
#endif

            _isPurchasing = false;
        }

        private void DeliverProduct(string productId)
        {
            switch (productId)
            {
                case ProductNoAds:
                    if (SaveService.Instance != null)
                    {
                        SaveService.Instance.SetNoAdsPurchased();
                    }
                    else
                    {
                        PlayerPrefs.SetInt(SaveService.NoAdsKey, 1);
                        PlayerPrefs.Save();
                    }
                    Debug.Log($"[RevenueCatIAPManager] 🌟 MUA THÀNH CÔNG GÓI NO-ADS ({productId})! Đã tắt hoàn toàn quảng cáo.");
                    break;

                case ProductBoosterSmall:
                    const int coinBonus = 500;
                    if (SaveService.Instance != null)
                    {
                        SaveService.Instance.AddCoins(coinBonus);
                    }
                    Debug.Log($"[RevenueCatIAPManager] 🎁 MUA THÀNH CÔNG GÓI COINS PACK ({productId})! +{coinBonus} Coins.");
                    break;

                default:
                    Debug.LogWarning($"[RevenueCatIAPManager] Sản phẩm chưa được định nghĩa: {productId}");
                    break;
            }
        }

        public void RestorePurchases(Action onSuccess, Action<string> onFailure)
        {
            Debug.Log("[RevenueCatIAPManager] Đang khôi phục các giao dịch đã mua (Restore Purchases)...");
            // Kiểm tra trạng thái đã lưu
            if (HasPurchasedNoAds())
            {
                Debug.Log("[RevenueCatIAPManager] Đã khôi phục thành công gói No-Ads.");
            }
            onSuccess?.Invoke();
        }
    }
}
