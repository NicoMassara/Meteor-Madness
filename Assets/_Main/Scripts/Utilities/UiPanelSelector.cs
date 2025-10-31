using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public abstract class UiPanelSelector<T> : MonoBehaviour, IPanelSelector<T>
    where T : UiComponentsData
    {
        [SerializeField] private string panelName;
        [Space(5)]
        [SerializeField] private T mobileData;
        [Space(5)]
        [SerializeField] private T desktopData;

        protected bool GetIsMobile()
        {
            return SystemInfo.deviceType == DeviceType.Handheld;
        }

        public T GetPanelData()
        {
            bool isMobile = GetIsMobile();
            string panelType = isMobile ? "Mobile" : "Desktop";
            Debug.Log($"{panelName} Panel Selected for {panelType}");
            Destroy(isMobile ? desktopData.MainPanel : mobileData.MainPanel);
            return isMobile ? mobileData : desktopData;
        }
    }
}