using System;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Base
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
    
    [Serializable]
    public abstract class UiComponentsData
    {
        // === Texts === //
        protected string GetLocalizedString(string langKey) => LocalizationManager.Instance.GetText(langKey);
        protected void SetText(TMP_Text text, string textToPlace) => text.text = textToPlace;
        protected void ClearText(TMP_Text text) => text.text = "";
        protected void SetTextColor(TMP_Text text, Color color) => text.color = color;
        
        // === Buttons === //
        protected void AddListenerToButton(Button button, UnityAction onClick) => button.onClick.AddListener(onClick);
        protected void RemoveListenerFromButton(Button button, UnityAction onClick) => button.onClick.RemoveListener(onClick);
        protected void SetButtonInteractable(Button button, bool interactable) => button.interactable = interactable;
        
        // === Game Objects === //
        
        protected void SetActiveObject(GameObject obj, bool active) => obj.SetActive(active);
    }
}