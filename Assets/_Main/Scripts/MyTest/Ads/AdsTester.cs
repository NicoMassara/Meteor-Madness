using System;
using System.Collections;
using _Main.Scripts.AdsSystem;
using _Main.Scripts.GlobalEvents;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.MyTest.Ads
{
    public class AdsTester : MonoBehaviour
    {
        private bool _hasInitializedAds;
        private bool _hasInitializedManager;
        
        [SerializeField]
        public UnityEvent OnInitialized;
        
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
        }
        
        private void Start()
        {
            StartCoroutine(Coroutine_LoadManager());
        }

        private IEnumerator Coroutine_LoadManager()
        {
            AdManager.LoadInstance();
            
            yield return new WaitForSeconds(0.25f);
            
            AdsEvents.InitializeAds();

            yield return new WaitUntil(()=> _hasInitializedAds == true);
            
            BootEvents.InitializeMainSystem();
            
            yield return new WaitUntil(()=> _hasInitializedManager == true);
            
            Debug.Log("Ad Manager initialized");

            yield return new WaitForSeconds(0.25f);

            OnInitialized?.Invoke();

            yield return null;
        }

    }
}