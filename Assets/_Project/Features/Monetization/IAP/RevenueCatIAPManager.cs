using System;
using System.Collections;
using UnityEngine;
using Game.Data;
using Game.Gameplay;

#if REVENUECAT_PURCHASES
using PurchasesModel;
#endif

namespace Game.Features.Monetization
{
    /// <summary>
    /// Quản lý In-App Purchases (IAP) tích hợp RevenueCat StoreKit / Google Play Billing.
    /// Hỗ trợ cả 2 chế độ:
    /// 1. RevenueCat Purchases SDK Native (com.revenuecat.purchases-unity).
    /// 2. Mock Simulation trên Windows Desktop Standalone / Editor (giả lập thanh toán mượt mà, lưu SaveService).
    /// </summary>
    public class RevenueCatIAPManager : MonoBehaviour, IIAPService
    {
        public static RevenueCatIAPManager Instance { get; private set; }

        public const string ProductNoAds = "shadowreflex.noads";
        public const string ProductBoosterSmall = "shadowreflex.coins.starter";

        [Header("RevenueCat Configuration")]
        [SerializeField] private string _androidApiKey = "goog_sample_test_revenuecat_key";
        [SerializeField] private string _iosApiKey = "appl_sample_test_revenuecat_key";
        [SerializeField] private bool _useLiveProduction = false;

        [Header("Simulation Settings (Windows / Editor)")]
        [SerializeField] private bool _enableMockOnDesktop = true;
        [SerializeField] private float _mockPurchaseDelay = 0.8f;

        public bool UseLiveProduction
        {
            get => _useLiveProduction;
            set => _useLiveProduction = value;
        }

        public string AndroidApiKey
        {
            get => _androidApiKey;
            set => _androidApiKey = value;
        }

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

#if REVENUECAT_PURCHASES
            Debug.Log($"[RevenueCatIAPManager] Khởi tạo RevenueCat Purchases SDK thật. Android Key: {_androidApiKey}");
            try
            {
#if UNITY_ANDROID
                var config = PurchasesConfiguration.Builder.Init(_androidApiKey).Build();
                Purchases.Configure(config);
#elif UNITY_IOS
                var config = PurchasesConfiguration.Builder.Init(_iosApiKey).Build();
                Purchases.Configure(config);
#endif
                _isInitialized = true;
                Debug.Log("[RevenueCatIAPManager] RevenueCat Purchases SDK đã khởi tạo thành công.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RevenueCatIAPManager] Lỗi cấu hình RevenueCat SDK: {ex.Message}");
                _isInitialized = _enableMockOnDesktop;
            }
#else
            Debug.Log($"[RevenueCatIAPManager] Đang chạy ở chế độ Desktop Standalone / Offline-First Mock. Hỗ trợ sản phẩm: '{ProductNoAds}', '{ProductBoosterSmall}'.");
            _isInitialized = true;
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

#if REVENUECAT_PURCHASES
            bool finished = false;
            bool success = false;
            string errorDetail = null;

            Purchases.GetOfferings((offerings, error) =>
            {
                if (error != null || offerings == null || offerings.Current == null)
                {
                    errorDetail = error != null ? error.Message : "No offerings available";
                    finished = true;
                    return;
                }

                Package targetPkg = null;
                foreach (var pkg in offerings.Current.AvailablePackages)
                {
                    if (pkg.StoreProduct != null && pkg.StoreProduct.Identifier == productId)
                    {
                        targetPkg = pkg;
                        break;
                    }
                }

                if (targetPkg == null)
                {
                    errorDetail = $"Không tìm thấy package tương ứng với product ID '{productId}'.";
                    finished = true;
                    return;
                }

                Purchases.PurchasePackage(targetPkg, (productIdentifier, customerInfo, userCancelled, purchaseError) =>
                {
                    if (userCancelled)
                    {
                        errorDetail = "Giao dịch đã bị người dùng hủy.";
                    }
                    else if (purchaseError != null)
                    {
                        errorDetail = purchaseError.Message;
                    }
                    else
                    {
                        success = true;
                    }
                    finished = true;
                });
            });

            while (!finished)
            {
                yield return null;
            }

            if (success)
            {
                DeliverProduct(productId);
                onSuccess?.Invoke();
                OnPurchaseSuccess?.Invoke(productId);
            }
            else
            {
                Debug.LogError($"[RevenueCatIAPManager] Thanh toán thất bại: {errorDetail}");
                onFailure?.Invoke(errorDetail);
                OnPurchaseFailed?.Invoke(productId, errorDetail);
            }
#else
            // Mock transaction trên Desktop / Editor / CI
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
                        SaveService.Instance.SetNoAdsPurchased(true);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(SaveService.NoAdsKey, 1);
                        PlayerPrefs.Save();
                    }
                    Debug.Log($"[RevenueCatIAPManager] 🌟 MUA THÀNH CÔNG GÓI NO-ADS ({productId})! Đã tắt hoàn toàn quảng cáo vĩnh viễn.");
                    break;

                case ProductBoosterSmall:
                    const int coinBonus = 1000; // Gói 1,000 vàng
                    if (SaveService.Instance != null)
                    {
                        SaveService.Instance.AddCoins(coinBonus);
                    }
                    Debug.Log($"[RevenueCatIAPManager] 🎁 MUA THÀNH CÔNG GÓI STARTER COINS ({productId})! +{coinBonus} Coins.");
                    break;

                default:
                    Debug.LogWarning($"[RevenueCatIAPManager] Sản phẩm chưa được định nghĩa: {productId}");
                    break;
            }
        }

        public void RestorePurchases(Action onSuccess, Action<string> onFailure)
        {
            Debug.Log("[RevenueCatIAPManager] Đang khôi phục các giao dịch đã mua (Restore Purchases)...");

#if REVENUECAT_PURCHASES
            Purchases.RestorePurchases((customerInfo, error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"[RevenueCatIAPManager] Lỗi khôi phục giao dịch: {error.Message}");
                    onFailure?.Invoke(error.Message);
                    return;
                }

                if (customerInfo != null && customerInfo.Entitlements.Active.ContainsKey("no_ads"))
                {
                    DeliverProduct(ProductNoAds);
                }
                onSuccess?.Invoke();
            });
#else
            if (HasPurchasedNoAds())
            {
                Debug.Log("[RevenueCatIAPManager] Đã khôi phục thành công gói No-Ads.");
            }
            onSuccess?.Invoke();
#endif
        }
    }
}
