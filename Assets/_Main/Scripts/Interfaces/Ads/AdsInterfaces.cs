using System;

namespace _Main.Scripts.Interfaces.Ads
{
    public interface IRewardedComponent
    {
        public event Action OnLoadAd;
        public event Action OnShowAd;
        public void TriggerReward();
    }
    
    public interface IDefeatAdComponent : IRewardedComponent
    {

    }
}