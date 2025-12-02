using System;
using _Main.Scripts.Localization;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
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
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
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
            GetUiComponents().DisableActivePanel();
        }

        private void HandleSphereDeflected()
        {
            SetHintText(GetLocalizedText(ShieldHintCode));
        }

        private void HandleMultiPage()
        {
            GetUiComponents().DisableActivePanel();
        }

        private void HandleEnable()
        {
            GetUiComponents().SetActivePanel(GetUiComponents().HintPanel);
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
            GetUiComponents().DisableAllPanels();
        }

        private void SetHintText(string text)
        {
            GetUiComponents().HintText.text = text;
            GetUiComponents().SetActivePanel(GetUiComponents().HintPanel);
        }

        private string GetLocalizedText(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}