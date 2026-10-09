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

        [Header("Hero Upgrades UI")]
        [SerializeField] private Button _upgradeHpButton;
        [SerializeField] private Button _upgradeDamageButton;
        [SerializeField] private Button _upgradeDodgeButton;
        [SerializeField] private Text _coinsBalanceText;
        [SerializeField] private Text _hpUpgradeText;
        [SerializeField] private Text _damageUpgradeText;
        [SerializeField] private Text _dodgeUpgradeText;

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

            if (_upgradeHpButton != null) _upgradeHpButton.onClick.AddListener(OnUpgradeHpClicked);
            if (_upgradeDamageButton != null) _upgradeDamageButton.onClick.AddListener(OnUpgradeDamageClicked);
            if (_upgradeDodgeButton != null) _upgradeDodgeButton.onClick.AddListener(OnUpgradeDodgeClicked);

            UpdateStatus();
        }

        private void OnDisable()
        {
            if (_buyNoAdsButton != null) _buyNoAdsButton.onClick.RemoveListener(OnBuyNoAdsClicked);
            if (_buyCoinPackButton != null) _buyCoinPackButton.onClick.RemoveListener(OnBuyCoinPackClicked);
            if (_restorePurchasesButton != null) _restorePurchasesButton.onClick.RemoveListener(OnRestorePurchasesClicked);
            if (_closeButton != null) _closeButton.onClick.RemoveListener(Close);

            if (_upgradeHpButton != null) _upgradeHpButton.onClick.RemoveListener(OnUpgradeHpClicked);
            if (_upgradeDamageButton != null) _upgradeDamageButton.onClick.RemoveListener(OnUpgradeDamageClicked);
            if (_upgradeDodgeButton != null) _upgradeDodgeButton.onClick.RemoveListener(OnUpgradeDodgeClicked);
        }

        public void Open()
        {
            Game.Audio.AudioManager.Instance?.PlayClick();
            gameObject.SetActive(true);
            UpdateStatus();
        }

        public void Close()
        {
            Game.Audio.AudioManager.Instance?.PlayClick();
            gameObject.SetActive(false);
        }

        private void UpdateStatus()
        {
            bool hasNoAds = SaveService.Instance != null && SaveService.Instance.HasPurchasedNoAds();
            int currentCoins = SaveService.Instance != null ? SaveService.Instance.GetCoins() : 0;

            if (_coinsBalanceText != null)
            {
                _coinsBalanceText.text = $"VÀNG: {currentCoins:N0}";
            }

            if (_noAdsStatusText != null)
            {
                _noAdsStatusText.text = hasNoAds ? "ĐÃ MUA (NO ADS ACTIVE)" : "$1.99 - GỠ QUẢNG CÁO";
            }

            if (_buyNoAdsButton != null)
            {
                _buyNoAdsButton.interactable = !hasNoAds;
            }

            // Cập nhật trạng thái nâng cấp Hero
            var upgradeMgr = Game.Gameplay.HeroUpgradeManager.Instance;
            if (upgradeMgr != null)
            {
                var data = upgradeMgr.Data;

                if (_hpUpgradeText != null)
                {
                    bool isMax = data.hpLevel >= Game.Gameplay.HeroUpgradeData.MaxHpLevel;
                    int cost = data.GetUpgradeCost(data.hpLevel);
                    _hpUpgradeText.text = isMax 
                        ? $"MÁU MAX (LV {data.hpLevel})" 
                        : $"NÂNG MÁU +25 HP (LV {data.hpLevel}) - {cost} Vàng";
                    if (_upgradeHpButton != null) _upgradeHpButton.interactable = !isMax && currentCoins >= cost;
                }

                if (_damageUpgradeText != null)
                {
                    bool isMax = data.damageLevel >= Game.Gameplay.HeroUpgradeData.MaxDamageLevel;
                    int cost = data.GetUpgradeCost(data.damageLevel);
                    _damageUpgradeText.text = isMax 
                        ? $"SÁT THƯƠNG MAX (LV {data.damageLevel})" 
                        : $"NÂNG CÔNG +8 DMG (LV {data.damageLevel}) - {cost} Vàng";
                    if (_upgradeDamageButton != null) _upgradeDamageButton.interactable = !isMax && currentCoins >= cost;
                }

                if (_dodgeUpgradeText != null)
                {
                    bool isMax = data.dodgeLevel >= Game.Gameplay.HeroUpgradeData.MaxDodgeLevel;
                    int cost = data.GetUpgradeCost(data.dodgeLevel);
                    _dodgeUpgradeText.text = isMax 
                        ? $"CỬA SỔ NÉ MAX (LV {data.dodgeLevel})" 
                        : $"CỬA SỔ NÉ +0.03s (LV {data.dodgeLevel}) - {cost} Vàng";
                    if (_upgradeDodgeButton != null) _upgradeDodgeButton.interactable = !isMax && currentCoins >= cost;
                }
            }
        }

        private void OnUpgradeHpClicked()
        {
            if (Game.Gameplay.HeroUpgradeManager.Instance != null)
            {
                if (Game.Gameplay.HeroUpgradeManager.Instance.TryUpgradeHP())
                {
                    Game.Audio.AudioManager.Instance?.PlayReward();
                    ShowFeedback("NÂNG CẤP MÁU THÀNH CÔNG! (+25 HP)");
                    UpdateStatus();
                }
                else
                {
                    Game.Audio.AudioManager.Instance?.PlayClick();
                    ShowFeedback("Không đủ vàng hoặc đã đạt cấp tối đa!");
                }
            }
        }

        private void OnUpgradeDamageClicked()
        {
            if (Game.Gameplay.HeroUpgradeManager.Instance != null)
            {
                if (Game.Gameplay.HeroUpgradeManager.Instance.TryUpgradeDamage())
                {
                    Game.Audio.AudioManager.Instance?.PlayReward();
                    ShowFeedback("NÂNG CẤP SÁT THƯƠNG THÀNH CÔNG! (+8 DMG)");
                    UpdateStatus();
                }
                else
                {
                    Game.Audio.AudioManager.Instance?.PlayClick();
                    ShowFeedback("Không đủ vàng hoặc đã đạt cấp tối đa!");
                }
            }
        }

        private void OnUpgradeDodgeClicked()
        {
            if (Game.Gameplay.HeroUpgradeManager.Instance != null)
            {
                if (Game.Gameplay.HeroUpgradeManager.Instance.TryUpgradeDodge())
                {
                    Game.Audio.AudioManager.Instance?.PlayReward();
                    ShowFeedback("NÂNG CẤP CỬA SỔ NÉ THÀNH CÔNG! (+0.03s)");
                    UpdateStatus();
                }
                else
                {
                    Game.Audio.AudioManager.Instance?.PlayClick();
                    ShowFeedback("Không đủ vàng hoặc đã đạt cấp tối đa!");
                }
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
                        Game.Audio.AudioManager.Instance?.PlayReward();
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
                        Game.Audio.AudioManager.Instance?.PlayReward();
                        if (SaveService.Instance != null)
                        {
                            SaveService.Instance.AddCoins(5000);
                            ShowFeedback($"MUA THÀNH CÔNG! Số dư mới: {SaveService.Instance.GetCoins():N0} Vàng.");
                        }
                        else
                        {
                            ShowFeedback("MUA THÀNH CÔNG! +5,000 Vàng.");
                        }
                        UpdateStatus();
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
