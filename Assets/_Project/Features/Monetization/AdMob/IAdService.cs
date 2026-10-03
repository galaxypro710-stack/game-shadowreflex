using System;

namespace Game.Features.Monetization
{
    /// <summary>
    /// Giao diện dịch vụ quảng cáo di động (Mobile Ad Service Interface).
    /// Tuân thủ Dependency Inversion Principle giúp dễ dàng tráo đổi giữa Mock và AdMob SDK thật.
    /// </summary>
    public interface IAdService
    {
        bool IsRewardedAdLoaded();
        void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null, Action<string> onAdFailed = null);
        void ShowInterstitialAd(Action onClosed = null);
    }
}
