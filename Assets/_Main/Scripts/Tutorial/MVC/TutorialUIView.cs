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

        private const string MeteorHintCode = "Tutorial.Hint.Meteor";
        private const string AbilityHintCode = "Tutorial.Hint.Ability";
        private const string ShieldHintCode = "Tutorial.Hint.Shield";
        private const string MoveRightHintCode = "Tutorial.Hint.MoveRight";
        private const string MoveLeftHintCode = "Tutorial.Hint.MoveLeft";
        private TutorialUiComponents _uiComponents;

        public event Action OnHintTextEnable;
        public event Action OnHintTextDisable;
        
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                
                // === Multi Page === //
                
                case TutorialObserverMessage.MultiPage:
                    HandleMultiPage();
                    break;
                
                // === Movement === //
                
                case TutorialObserverMessage.RightMovement:
                    HandleRightMovement();
                    break;
                
                case TutorialObserverMessage.LeftMovement:
                    HandleLeftMovement();
                    break;
                
                // === Meteor === //
                
                case TutorialObserverMessage.Meteor:
                    HandleMeteor();
                    break;
                
                // === Ability === //
                
                case TutorialObserverMessage.SphereDeflected:
                    HandleSphereDeflected();
                    break;
                
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
                
                case TutorialObserverMessage.AbilityRunning:
                    HandleAbilityRunning();
                    break;
            }
        }



        #region Movement

        private void HandleRightMovement()
        {
            SetHintText(GetLocalizedText(MoveRightHintCode));
        }
        
        private void HandleLeftMovement()
        {
            SetHintText(GetLocalizedText(MoveLeftHintCode));
        }

        #endregion
        
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
        
        private void HandleMeteor()
        {
            SetHintText(GetLocalizedText(MeteorHintCode));
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