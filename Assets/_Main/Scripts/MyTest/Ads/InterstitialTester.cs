using _Main.Scripts.AdsSystem;
using UnityEngine;

namespace _Main.Scripts.MyTest.Ads
{
    public class InterstitialTester : BaseAdTester
    {
        protected override void Button_OnClick()
        {
            if(AdManager.GetInterstitial() == null)
                return;
            
            AdManager.GetInterstitial().TryLoad(Loaded,
                onFailed: error =>
                {
                    EnableButton();
                }
            );
        }

        #region Load
        
        
        private void Loaded(string input)
        {
            if(AdManager.GetInterstitial() == null)
                return;
            
            AdManager.GetInterstitial().TryShow(onShowed: null, 
                    ShowClick,
                    ShowFailed, ShowComplete, ShowSkipped);
        }
        
        #endregion

        #region Show
        
        private void ShowFailed(string input)
        {
            Debug.Log($"Show Failed: {input}");
            
            EnableButton();
        }
        
        private void ShowClick(string input)
        {
            Debug.Log($"Show Clicked: {input}");
        }
        
        private void ShowComplete(string input)
        {
            Debug.Log($"Show Complete: {input}");
            
            EnableButton();
        }
        
        private void ShowSkipped(string input)
        {
            Debug.Log($"Show Skipped: {input}");
            
            EnableButton();
        }

        #endregion
    }
}