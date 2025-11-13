using System;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private CosmeticUIPanelSelector uiSelector;
        
        private CosmeticUIComponents _uiComponents;
        private GameObject _currentPanel;
        
        public event Action OnMainMenuButtonPressed;

        private void Awake()
        {
            GetUiComponents().MainMenuButton.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.Disable:
                    HandleDisable();
                    break;
            }
        }
        
        private void HandleEnable()
        {
            SetActivePanel(GetUiComponents().MainPanel);
        }

        private void HandleDisable()
        {
            DisableActivePanel();
            GetUiComponents().MainPanel.SetActive(false);
        }
        
        private void SetActivePanel(GameObject panelObject)
        {
            _currentPanel?.SetActive(false);
            _currentPanel = panelObject;
            _currentPanel.SetActive(true);
        }

        private void DisableActivePanel()
        {
            _currentPanel?.SetActive(false);
        }
        
        private CosmeticUIComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }
    }
}