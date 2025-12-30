using System;
using MeteorMadness.Managers.Localization;
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
        [Header("Panel")]
        [SerializeField] private GameObject panel;
        public event Action OnClose;

        private void Start()
        {
            panel.SetActive(false);
        }

        public void EnablePanel()
        {
            titleText.text = GetLocalizatedString(titleCode);
            descriptionText.text = GetLocalizatedString(descriptionCode);
            closeButton.onClick.AddListener(TriggerOnClose);
            panel.SetActive(true);
        }
        public void DisablePanel()
        {
            closeButton.onClick.RemoveListener(TriggerOnClose);
            panel.SetActive(false);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(TriggerOnClose);
        }

        private void TriggerOnClose() => OnClose?.Invoke();
        private string GetLocalizatedString(string key) => LocalizationManager.Instance.GetText(key);
        public void SetButtonInteractable(bool value) => closeButton.interactable = value;
    }
}