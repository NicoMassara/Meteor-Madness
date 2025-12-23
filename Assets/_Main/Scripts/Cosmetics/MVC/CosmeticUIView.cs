using System;
using _Main.Scripts.Cosmetics.Components;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Localization;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using Unity.Mathematics;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : BaseViewUI<CosmeticUIPanelSelector, CosmeticUIComponents>, 
        ICosmeticUISounds, ICosmeticUIVibration, CosmeticUIView.ICosmeticUIView, IUpdatable
    {
        public interface ICosmeticUIView
        {
            public event Action OnMainMenuButtonPressed;
            public event Action<int> OnSkinSelected;
            
            public event Action<SkinType> OnUnlockButtonPressed;
        }

        private class NumberDecrementor
        {
            private readonly float IncreaseTime;
            private float _elapsedTime;
            private uint _currentValue;
            private uint _startValue;
            private uint _endValue;
            public bool IsActive { get; private set; }
            
            private readonly Action<uint> _decrementAction;
            private readonly Action _actionOnFinished;

            public NumberDecrementor(float increaseTime, Action<uint> decreaseAction, Action onFinishedAction)
            {
                IncreaseTime = increaseTime;
                _decrementAction = decreaseAction;
                _actionOnFinished = onFinishedAction;
            }

            public void Execute(float deltaTime)
            {
                _elapsedTime += deltaTime;
            
                float ratio = Mathf.Clamp01(_elapsedTime / IncreaseTime);
                
                _currentValue = (uint)math.lerp(_startValue, _endValue, ratio);
                
                Debug.Log(_currentValue);
                
                if (ratio >= 1f)
                {
                    _startValue = _currentValue;
                    _decrementAction?.Invoke(_currentValue);
                    _actionOnFinished?.Invoke();
                    ClearValues();
                }
                else
                {
                    _decrementAction?.Invoke(_currentValue);
                }
            }

            public void SetStartValue(uint startValue, uint endValue)
            {
                _currentValue = startValue;
                _startValue = startValue;
                _endValue = endValue;
                _elapsedTime = 0;
                IsActive = true;
                
                Debug.Log($"Start: {startValue}, End: {endValue}");

            }

            private void ClearValues()
            {
                IsActive = false;
                _startValue = 0;
                _endValue = 0;
                _currentValue = 0;
            }
        }
        
        private NumberDecrementor _numberDecrementor;
        private SkinButtonCreator _skinButtonController;
        
        // Hack
        private uint _lastCoins;
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnScroll;
        public event Action OnUnlockFailed;
        public event Action OnUnlocked;
        public event Action OnCoinsFinishedDecrement;
        public event Action<int> OnSkinSelected;
        public event Action<SkinType> OnUnlockButtonPressed;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            _numberDecrementor = new NumberDecrementor(1f, UpdateCoinsText, OnDecrementFinished);
            UIComponents.SetActiveDescriptionPanel(false);
            _skinButtonController = new SkinButtonCreator(()=> Instantiate(UIComponents.SkinSelectButton, UIComponents.ButtonsContainer));
            _skinButtonController.OnSkinSelected += ButtonControllerOnSkinSelectedHandler;
        }

        private void Start()
        {
            _skinButtonController.Initialize();   
        }
        public void ExecuteUpdate(float deltaTime)
        {
            if (_numberDecrementor.IsActive)
            {
                _numberDecrementor.Execute(deltaTime);
            }
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
                case CosmeticObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case CosmeticObserverMessage.Enable:
                    HandleEnable();
                    break;
                case CosmeticObserverMessage.Disable:
                    HandleDisable();
                    break;
                case CosmeticObserverMessage.Opened:
                    HandleOpened();
                    break;
                
                // === Unlock === // 
                case CosmeticObserverMessage.Unlocked:
                    HandleUnlocked((int)args[0]);
                    break;
                case CosmeticObserverMessage.FailedToUnlock:
                    UnlockFailedToUnlock();
                    break;
            }
        }

        #region Enable / Disable

        private void HandleInitialize()
        {
            _skinButtonController.DisableCurrentSkinButtonInteraction();
            UpdateDescriptionText(SkinManager.Instance.GetCurrentSkinType());
            UpdateCoinsText(SkinManager.Instance.GetCoins());
            UIComponents.SetActiveLockedPanel(false);
        }
        
        private void HandleEnable()
        {
            UIComponents.AddListenerToUnlockButton(UnlockButton_OnClickHandler);
            UIComponents.AddListenerToBackButton(TriggerMainMenuButtonPressed);
            _skinButtonController.AddListenerToAllButtons();
            UIComponents.AddScrollListener(TriggerOnScroll);
        }
        
        private void HandleOpened()
        {
            UIComponents.SetActiveDescriptionPanel(true);
        }
        
        private void HandleDisable()
        {
            _skinButtonController.ClearDisableButton();
            UIComponents.RemoveListenerToUnlockButton(UnlockButton_OnClickHandler);
            UIComponents.RemoveListenerBackButton(TriggerMainMenuButtonPressed);
            UIComponents.RemoveScrollListener(TriggerOnScroll);
            _skinButtonController.RemoveListenerFromAllButtons();
            UIComponents.SetActiveDescriptionPanel(false);
            
        }

        #endregion

        #region Unlock

        private void HandleUnlocked(int skinIndex)
        {
            OnUnlocked?.Invoke();
            _skinButtonController.RemoveListenerFromAllButtons();
            UIComponents.RemoveListenerToUnlockButton(UnlockButton_OnClickHandler);
            UIComponents.RemoveListenerBackButton(TriggerMainMenuButtonPressed);
            UIComponents.SetLockedTextGreen();
            UIComponents.ClearRequirementText();
            UIComponents.SetLockedText("Cosmetic.Unlocked");
            UIComponents.DisableUnlockButton();
            UIComponents.RemoveScrollListener(TriggerOnScroll);
            
            _numberDecrementor.SetStartValue(_lastCoins, SkinManager.Instance.GetCoins());
        }

        private void UnlockFailedToUnlock()
        {
            OnUnlockFailed?.Invoke();
            UIComponents.SetUnlockButtonInteractable(false);
            UIComponents.SetLockedText("Cosmetic.NotEnough");
            
            TimerManager.Add(new TimerData(1.5f, () =>
            {
                UIComponents.SetUnlockButtonInteractable(true);
                UIComponents.SetLockedTextRed();
                UIComponents.SetLockedText("Cosmetic.Locked");
                UIComponents.EnableUnlockButton();
            }));
        }

        #endregion
        
        private void ButtonControllerOnSkinSelectedHandler(SkinType skinSelected)
        {
            OnSkinSelected?.Invoke((int)skinSelected);
            UpdateDescriptionText(skinSelected);
            
            if (SkinManager.Instance.GetIsLocked((int)skinSelected))
            {
                UIComponents.SetActiveLockedPanel(true);
                UpdateRequirementText(skinSelected);
            }
            else
            {
                UIComponents.SetActiveLockedPanel(false);
            }
        }

        #region Texts Update
        
        private void UpdateDescriptionText(SkinType skinType)
        {
            var descriptionCode = SkinManager.Instance.GetSkinInformationByType(skinType).DescriptionCode;
            UIComponents.SetDescriptionText(descriptionCode);
        }


        private void UpdateCoinsText(uint coinsAmount) 
            => UIComponents.SetCoinsText("Cosmetic.StoredCoins", coinsAmount);

        private void UpdateRequirementText(SkinType skinType)
        {
            var coinsPrize = SkinManager.Instance.GetSkinInformationByType(skinType).UnlockPrice;
            var coinsCode = coinsPrize != 1 ? "Cosmetic.Coins.Multiple" : "Cosmetic.Coins.Single";
            UIComponents.SetRequirementText("Cosmetic.Requirement", coinsPrize, coinsCode);
        }

        #endregion
        
        #region Tools

        private string GetLocalizedString(string key) => LocalizationManager.Instance.GetText(key);

        #endregion

        #region Handlers
        
        private void UnlockButton_OnClickHandler()
        {
            _lastCoins = SkinManager.Instance.GetCoins();
            var skinToUnlock = SkinManager.Instance.GetPreviewedSkin();
            OnUnlockButtonPressed?.Invoke(skinToUnlock);
        }

        private void TriggerMainMenuButtonPressed() => OnMainMenuButtonPressed?.Invoke();

        
        private void OnDecrementFinished()
        {
            OnCoinsFinishedDecrement?.Invoke();
            UIComponents.SetActiveLockedPanel(false);
            UIComponents.AddListenerToUnlockButton(UnlockButton_OnClickHandler);
            UIComponents.AddListenerToBackButton(TriggerMainMenuButtonPressed);
            UIComponents.AddScrollListener(TriggerOnScroll);
            _skinButtonController.AddListenerToAllButtons();
        }
        
        private void TriggerOnScroll(Vector2 scrollPosition) => OnScroll?.Invoke();
        
        #endregion
    }
}