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

        public event Action OnHintTextEnable;
        public event Action OnHintTextDisable;
        
        
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
            }
        }
        
        private TutorialUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }

        private void HandleAbilityRunning()
        { 
            OnHintTextDisable?.Invoke();
        }

        private void HandleSphereDeflected()
        {
            SetHintText(GetLocalizedText(ShieldHintCode));
        }

        private void HandleMultiPage()
        {
            OnHintTextDisable?.Invoke();
        }
        
        private void HandleMovement()
        {
            SetHintText(GetLocalizedText(MovementHintCode));
        }
        
        private void HandleAbility()
        {
            SetHintText(GetLocalizedText(AbilityHintCode));
        }

        private void SetHintText(string text)
        {
            GetUiComponents().HintText.text = text;
            OnHintTextEnable?.Invoke();
        }

        private string GetLocalizedText(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}