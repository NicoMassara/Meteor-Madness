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