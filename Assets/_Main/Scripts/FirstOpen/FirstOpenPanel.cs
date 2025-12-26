using System;
using _Main.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.FirstOpen
{
    public class FirstOpenPanel : MonoBehaviour
    {
        [Header("Localization Codes")]
        [SerializeField] private string titleCode;
        [SerializeField] private string descriptionCode;
        [Header("Texts")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [Header("Button")]
        [SerializeField] private Button closeButton;
        
        public event Action OnClose;
        
        private void OnEnable()
        {
            titleText.text = GetLocalizatedString(titleCode);
            descriptionText.text = GetLocalizatedString(descriptionCode);
            closeButton.onClick.AddListener(TriggerOnClose);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(TriggerOnClose);
        }

        private void TriggerOnClose() => OnClose?.Invoke();

        private string GetLocalizatedString(string key) => LocalizationManager.Instance.GetText(key);
    }
}