using System;

namespace _Main.Scripts.Interfaces.Ads
{
    public interface IRewardedComponent
    {
        public event Action OnLoadAd;
        public event Action OnShowAd;
        public void TriggerReward();
    }

    public interface IBannerComponent
    {
        public event Action OnLoadAd;
        public event Action OnShowAd;
        public event Action OnHideAd;
        public event Action OnDestroyAd;
    }

    public interface IDefeatAdComponent : IRewardedComponent { }
    
    public interface IMainMenuBannerComponent : IBannerComponent { }
}