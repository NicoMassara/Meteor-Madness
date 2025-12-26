using System.Collections;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Services.AdsSystem;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.MyTest.Ads
{
    public class AdsTester : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        [SerializeField] private TMP_Text debugText;
        
        private bool _hasInitializedAds;
        private bool _hasInitializedManager;
        
        private void Awake()
        {
            AdsEvents.OnAdsInitialized += () =>
            {
                _hasInitializedAds = true;
            };
            
            BootEvents.OnMainSystemInitialized += () =>
            {
                _hasInitializedManager = true;
            };
            
            debugText.text = "Initializing...";
        }
        
        private void Start()
        {
            StartCoroutine(Coroutine_LoadManager());
        }

        private IEnumerator Coroutine_LoadManager()
        {
            AdManager.LoadInstance();
            
            debugText.text = "Loading Ad Services...";
            
            yield return new WaitForSeconds(0.25f);
            
            AdsEvents.InitializeAds();

            debugText.text = "Initializing Ad Manager...";
            
            yield return new WaitUntil(()=> _hasInitializedAds == true);
            
            BootEvents.InitializeMainSystem();
            
            yield return new WaitUntil(()=> _hasInitializedManager == true);
            
            debugText.text = "Ad Manager initialized!";

            yield return new WaitForSeconds(0.25f);
            
            debugText.text = "All Loaded!";

            yield return null;
        }
#endif
    }
}