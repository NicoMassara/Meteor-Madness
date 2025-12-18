using System;
using _Main.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.Interfaces
{
    public interface IPanelSelector<T> 
    where T : UiComponentsData
    {
        public T GetPanelData();
    }

    [Serializable]
    public abstract class UiComponentsData
    {
        protected void SetText(TMP_Text text, string textToPlace) => text.text = textToPlace;
        protected void AddListenerToButton(Button button, UnityAction onClick) => button.onClick.AddListener(onClick);
        protected void RemoveListenerFromButton(Button button, UnityAction onClick) => button.onClick.RemoveListener(onClick);
        protected string GetLocalizedString(string langKey) => LocalizationManager.Instance.GetText(langKey);
    }
}