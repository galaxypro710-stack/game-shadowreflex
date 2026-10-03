using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Features.Monetization;
using Game.Data;

namespace Game.UI
{
    /// <summary>
    /// Popup Cửa Hàng In-App Purchase (IAP Shop).
    /// Chuẩn Universal Core: Quản lý gói NoAds, gói Tiền Vàng và khôi phục giao dịch.
    /// Tích hợp RevenueCat IAP với cơ chế Offline Fallback an toàn.
    /// </summary>
    public class ShopPopup : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _buyNoAdsButton;
        [SerializeField] private Button _buyCoinPackButton;
        [SerializeField] private Button _restorePurchasesButton;
        [SerializeField] private Button _closeButton;

        [Header("Status Labels")]
        [SerializeField] private Text _noAdsStatusText;
        [SerializeField] private Text _feedbackText;

        public const string ProductCoinStarter = "game.coins.starter";

        private void OnEnable()
        {
            if (_buyNoAdsButton != null) _buyNoAdsButton.onClick.AddListener(OnBuyNoAdsClicked);
            if (_buyCoinPackButton != null) _buyCoinPackButton.onClick.AddListener(OnBuyCoinPackClicked);
            if (_restorePurchasesButton != null) _restorePurchasesButton.onClick.AddListener(OnRestorePurchasesClicked);
            if (_closeButton != null) _closeButton.onClick.AddListener(Close);

            UpdateStatus();
        }

        private void OnDisable()
        {
            if (_buyNoAdsButton != null) _buyNoAdsButton.onClick.RemoveListener(OnBuyNoAdsClicked);
            if (_buyCoinPackButton != null) _buyCoinPackButton.onClick.RemoveListener(OnBuyCoinPackClicked);
            if (_restorePurchasesButton != null) _restorePurchasesButton.onClick.RemoveListener(OnRestorePurchasesClicked);
            if (_closeButton != null) _closeButton.onClick.RemoveListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            UpdateStatus();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void UpdateStatus()
        {
            bool hasNoAds = SaveService.Instance != null && SaveService.Instance.HasPurchasedNoAds();

            if (_noAdsStatusText != null)
            {
                _noAdsStatusText.text = hasNoAds ? "ĐÃ MUA (NO ADS ACTIVE)" : "$1.99 - GỠ QUẢNG CÁO";
            }

            if (_buyNoAdsButton != null)
            {
                _buyNoAdsButton.interactable = !hasNoAds;
            }
        }

        private void OnBuyNoAdsClicked()
        {
            ShowFeedback("Đang kết nối hệ thống thanh toán...");

            if (RevenueCatIAPManager.Instance != null)
            {
                RevenueCatIAPManager.Instance.PurchaseProduct(
                    RevenueCatIAPManager.ProductNoAds,
                    onSuccess: () =>
                    {
                        if (SaveService.Instance != null)
                        {
                            SaveService.Instance.SetNoAdsPurchased();
                        }
                        ShowFeedback("MUA THÀNH CÔNG! Đã gỡ bỏ toàn bộ quảng cáo.");
                        UpdateStatus();
                    },
                    onFailure: (err) =>
                    {
                        ShowFeedback($"Giao dịch thất bại: {err}");
                    }
                );
            }
        }

        private void OnBuyCoinPackClicked()
        {
            ShowFeedback("Đang thanh toán Gói 5,000 Coin Vàng...");

            if (RevenueCatIAPManager.Instance != null)
            {
                RevenueCatIAPManager.Instance.PurchaseProduct(
                    ProductCoinStarter,
                    onSuccess: () =>
                    {
                        if (SaveService.Instance != null)
                        {
                            SaveService.Instance.AddCoins(5000);
                            ShowFeedback($"MUA THÀNH CÔNG! Số dư mới: {SaveService.Instance.GetCoins():N0} Vàng.");
                        }
                        else
                        {
                            ShowFeedback("MUA THÀNH CÔNG! +5,000 Vàng.");
                        }
                    },
                    onFailure: (err) =>
                    {
                        ShowFeedback($"Giao dịch thất bại: {err}");
                    }
                );
            }
        }

        private void OnRestorePurchasesClicked()
        {
            ShowFeedback("Đang khôi phục các giao dịch đã mua...");
            if (RevenueCatIAPManager.Instance != null)
            {
                RevenueCatIAPManager.Instance.RestorePurchases(
                    onSuccess: () =>
                    {
                        ShowFeedback("ĐÃ KHÔI PHỤC THÀNH CÔNG!");
                        UpdateStatus();
                    },
                    onFailure: (err) =>
                    {
                        ShowFeedback($"Lỗi khôi phục: {err}");
                    }
                );
            }
        }

        private void ShowFeedback(string msg)
        {
            if (_feedbackText != null)
            {
                _feedbackText.text = msg;
            }
            Debug.Log($"[ShopPopup] {msg}");
        }
    }
}
