using System;
using _Main.Scripts.AdsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;


namespace _Main.Scripts.MyTest.Ads
{
    public class BannerTest : BaseAdTester
    {
        [SerializeField] private BannerPosition bannerPosition;

        private bool _isOpen;

        private void Start()
        {
            openAddButton.interactable = false;
        }

        protected override void Button_OnClick()
        {
            if(AdManager.GetBanner() == null)
                return;
            
            if (AdManager.GetBanner().IsActive)
            {
                AdManager.GetBanner().TryHide();
            }
            else
            {
                AdManager.GetBanner().TryLoad(bannerPosition, 
                    onLoaded: OnLoaded,
                    onFailed: (error) =>
                    {
                        Debug.Log($"Banner Load Failed: {error}");
                        EnableButton();
                    }
                );
            }
        }

        public override void EnableButton()
        {
            openAddButton.interactable = true;
            openAddButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Open Banner";
        }

        protected override void DisableButton()
        {
            openAddButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Close Banner";
        }

        #region Load
        

        private void OnLoaded()
        {
            Debug.Log("Banner Loaded");
            AdManager.GetBanner().TryShow(
                
                onShown: () =>
                {
                    Debug.Log("Banner Shown");
                },
                
                onClicked: () =>
                {
                    Debug.Log("Banner Clicked");
                },
                
                onHidden: () =>
                {
                    EnableButton();
                    Debug.Log("Banner Hidden");
                }
            );
        }
        
        #endregion
    }
}