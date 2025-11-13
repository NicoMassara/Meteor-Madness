using System;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialUIView : ManagedBehavior, IObserver
    {
        [SerializeField] private TutorialUiSelector uiSelector;

        private const string MovementHintCode = "Tutorial.Hint.Movement";
        private const string AbilityHintCode = "Tutorial.Hint.Ability";
        private const string ShieldHintCode = "Tutorial.Hint.Shield";
        private TutorialUiComponents _uiComponents;

        private GameObject _currentActivePanel;
        public event Action OnStartTutorialButtonPressed;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.Start:
                    HandleStart();
                    break;
                case TutorialObserverMessage.Movement:
                    HandleMovement();
                    break;
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
                case TutorialObserverMessage.MultiPage:
                    HandleMultiPage();
                    break;
                case TutorialObserverMessage.SphereDeflected:
                    HandleSphereDeflected();
                    break;
                case TutorialObserverMessage.AbilityRunning:
                    HandleAbilityRunning();
                    break;
                case TutorialObserverMessage.Disable:
                    HandleDisable();
                    break;
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;
            }
        }
        
        private TutorialUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }

        private void HandleAbilityRunning()
        {
            DisableActivePanel();
        }

        private void HandleSphereDeflected()
        {
            SetHintText(GetLocalizedText(ShieldHintCode));
        }

        private void HandleStart()
        {
            // Structure has changed, easiest and fastest way to do it
            // This works to auto start tutorial without changing to much code
            // and breaking anything
            OnStartTutorialButtonPressed?.Invoke();
        }

        private void HandleMultiPage()
        {
            DisableActivePanel();
        }

        private void HandleEnable()
        {
            GetUiComponents().MainPanel.SetActive(true);
        }
        
        private void HandleMovement()
        {
            SetHintText(GetLocalizedText(MovementHintCode));
        }
        
        private void HandleAbility()
        {
            SetHintText(GetLocalizedText(AbilityHintCode));
        }
        
        private void HandleDisable()
        {
            DisableActivePanel();
            GetUiComponents().MainPanel.SetActive(false);
        }

        private void SetHintText(string text)
        {
            GetUiComponents().HintText.text = text;
            SetActivePanel(GetUiComponents().HintPanel);
        }

        private void SetActivePanel(GameObject input)
        {
            _currentActivePanel?.SetActive(false);
            _currentActivePanel = input;
            _currentActivePanel?.SetActive(true);
        }

        private void DisableActivePanel()
        {
            _currentActivePanel?.SetActive(false);
            _currentActivePanel = null;
        }

        private string GetLocalizedText(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }

        #region Handlers

        private void NextButtonOnClickHandler()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
            OnStartTutorialButtonPressed?.Invoke();
        }
        
        private void FinishButtonOnClickHandler()
        {
            SoundEventCaller.PlayUIButton(UISoundType.Back);
            GameManager.Instance.LoadMainMenu();
        }

        #endregion
    }
}