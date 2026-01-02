using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MyTest.Ads
{
    public abstract class BaseAdTester : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        [SerializeField] protected Button loadAddButton;
        [SerializeField] protected Button openAddButton;


        private void Awake()
        {
            loadAddButton.onClick.AddListener(LoadAd);
            openAddButton.onClick.AddListener(ShowAd);
        }

#endif
        protected abstract void ShowAd();
        protected abstract void LoadAd();
    }
}