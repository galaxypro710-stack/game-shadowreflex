using System;

namespace Game.Features.Monetization
{
    /// <summary>
    /// Giao diện chuẩn hóa cho hệ thống In-App Purchase (IAP).
    /// Tuân thủ nguyên tắc Loose Coupling để hoán đổi giữa Mock Desktop và Native RevenueCat StoreKit/Google Play Billing.
    /// </summary>
    public interface IIAPService
    {
        bool IsInitialized();
        bool HasPurchasedNoAds();
        void PurchaseProduct(string productId, Action onSuccess, Action<string> onFailure);
        void RestorePurchases(Action onSuccess, Action<string> onFailure);
    }
}
