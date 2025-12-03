using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Utilities
{
    public abstract class UiComponentsSelector<T> : MonoBehaviour, IPanelSelector<T>
    where T : UiComponentsData
    {
        [SerializeField] private string panelName;
        [Space(5)]
        [SerializeField] private T mobileData;
        [Space(5)]
        [SerializeField] private T desktopData;

        private bool _hasAlreadySelected;
        private bool _isMobile;

        protected bool GetIsMobile()
        {
#if UNITY_EDITOR
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else

            return false;
#endif
#else
            return SystemInfo.deviceType == DeviceType.Handheld;
#endif
        }

        public T GetPanelData()
        {
            if (_hasAlreadySelected == false)
            {
                _isMobile = GetIsMobile();
                if (_isMobile)
                {
                    desktopData = null;
                }
                else
                {
                    mobileData = null;
                }
                
                _hasAlreadySelected = true;
            }

            return _isMobile ? mobileData : desktopData;
        }
    }
}