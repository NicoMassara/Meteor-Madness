using System;
using _Main.Scripts.Managers.UpdateManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewUI : ManagedBehavior
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text panelText;
        [SerializeField] private TMP_Text nextButtonText;
        [Header("Buttons")]
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [Header("Components")] 
        [SerializeField] private GameObject mainPanel;

        public event Action OnPreviousButtonPressed;
        public event Action OnNextButtonPressed;
        
        private void Awake()
        {
            previousButton.onClick.AddListener(()=> OnPreviousButtonPressed?.Invoke());
            nextButton.onClick.AddListener(()=> OnNextButtonPressed?.Invoke());
        }

        public void SetNextButtonText(string text)
        {
            nextButtonText.text = text;
        }

        public void SetEnablePreviousButton(bool isEnable)
        {
            previousButton.gameObject.SetActive(isEnable);
        }

        public void SetPanelText(string text)
        {
            panelText.text = text;
        }

        public string GetNextButtonText()
        {
            return nextButtonText.text;
        }

        public void SetActiveMainPanel(bool isActive)
        {
            mainPanel.SetActive(isActive);
        }
    }
}