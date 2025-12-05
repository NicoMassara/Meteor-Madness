using System;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/MVC")]
    public class DefeatUIView : MonoBehaviour, IObserver
    {
        [SerializeField] private DefeatUiPanelSelector uiComponentSelector;
        
        private DefeatUIComponents _uiComponents;
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
        
        private DefeatUIComponents GetUiComponents()
        {
            return _uiComponents ?? uiComponentSelector.GetPanelData();
        }


        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case DefeatObserverMessage.EnableButtons:
                    HandleEnableButtons();
                    break;
            }
        }

        private void HandleStartDisable()
        {
            GetUiComponents().RestartButton.onClick.RemoveAllListeners();
            GetUiComponents().MainMenuButtons.onClick.RemoveAllListeners();
        }

        private void HandleEnableButtons()
        {
            GetUiComponents().RestartButton.onClick.AddListener(() =>
            {
                OnRestartButtonPressed?.Invoke();
            });
            
            GetUiComponents().MainMenuButtons.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
        }
    }
}