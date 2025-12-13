using System;
using _Main.Scripts.Interfaces.Ads;
using UnityEngine;

namespace _Main.Scripts.MyTest.Ads
{
    public class RewardTest : BaseAdTester, RewardTest.IRewardTestComponent
    {
        public interface IRewardTestComponent : IRewardedComponent
        {
            
        }
        
        public event Action OnShowAd;
        public event Action OnLoadAd;
        
        public void TriggerReward()
        {
            Debug.Log("Reward Received!");
        }
        
        protected override void ShowAd()
        {
            OnShowAd?.Invoke();
        }

        protected override void LoadAd()
        {
            OnLoadAd?.Invoke();
        }
    }
}