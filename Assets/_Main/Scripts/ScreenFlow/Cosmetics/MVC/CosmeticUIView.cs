using System;
using _Main.Scripts.Cosmetics.Components;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Base;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using Unity.Mathematics;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : BaseViewUI<CosmeticUIPanelSelector, CosmeticUIComponents>, CosmeticUIView.ICosmeticUIView, 
        IUpdatable,
        ICosmeticUISounds, 
        ICosmeticUIVibration
    {
        public interface ICosmeticUIView
        {
            public event Action OnMainMenuButtonPressed;
            public event Action<int> OnSkinSelected;
            public event Action<SkinType> OnUnlockButtonPressed;
        }

        private class NumberDecrement
        {
            private readonly float _duration;

            private float _elapsedTime;
            private uint _startValue;
            private uint _endValue;
            private uint _currentValue;

            public bool IsActive { get; private set; }

            private readonly Action<uint> _onValueChanged;
            private readonly Action _onFinished;

            public NumberDecrement(
                float duration,
                Action<uint> onValueChanged,
                Action onFinished)
            {
                _duration = Mathf.Max(0.0001f, duration);
                _onValueChanged = onValueChanged;
                _onFinished = onFinished;
            }

            public void Execute(float deltaTime)
            {
                if (!IsActive)
                    return;

                _elapsedTime += deltaTime;
                float t = Mathf.Clamp01(_elapsedTime / _duration);

                _currentValue = (uint)Mathf.Lerp(_startValue, _endValue, t);
                _currentValue = _startValue > _endValue
                    ? Math.Max(_endValue, _currentValue)
                    : Math.Min(_endValue, _currentValue);
                
                _onValueChanged?.Invoke(_currentValue);

                if (t >= 1f)
                {
                    _currentValue = _endValue;
                    IsActive = false;
                    _onFinished?.Invoke();
                }
            }

            public void Start(uint startValue, uint endValue)
            {
                _startValue = startValue;
                _endValue = endValue;
                _currentValue = startValue;
                _elapsedTime = 0f;
                IsActive = true;
            }
        }

        
        private NumberDecrement numberDecrement;
        private SkinButtonCreator _skinButtonController;
        
        // Hack
        private uint _lastCoins;

        #region ICosmeticUIView

        public event Action OnMainMenuButtonPressed;
        public event Action<int> OnSkinSelected;
        public event Action<SkinType> OnUnlockButtonPressed;

        #endregion
        
        #region ICosmeticUISounds

        

        #endregion
        
        #region ICosmeticUIVibration

        public event Action OnUnlockFailed;
        public event Action OnUnlocked;
        public event Action OnCoinsFinishedDecrement;

        public event Action<int> OnScroll;

        #endregion


        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            numberDecrement = new NumberDecrement(1f, UpdateCoinsText, OnDecrementFinished);
            UIComponents.SetActiveDescriptionPanel(false);
            UIComponents.SetActiveFirstOpenPanel(false);
            UIComponents.SetLockedTextRed();
            _skinButtonController = new SkinButtonCreator(()=> Instantiate(UIComponents.SkinSelectButton, UIComponents.ButtonsContainer));
            _skinButtonController.OnSkinSelected += ButtonControllerOnSkinSelectedHandler;
        }

        private void Start()
        {
            _skinButtonController.Initialize();
            LocalizationEvents.OnLanguageChanged += OnLanguageChanged;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (numberDecrement.IsActive)
            {
                numberDecrement.Execute(deltaTime);
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
                
                // === First Open === //
                case CosmeticObserverMessage.FirstOpen:
                    HandleFirstOpen();
                    break;
            }
        }



        #region Enable / Disable

        private void HandleInitialize()
        {
            _skinButtonController.DisableCurrentSkinButtonInteraction();
            UpdateDescriptionText(SkinManager.Instance.GetCurrentSkinType());
            UpdateCoinsText(SkinManager.Instance.GetCoins());
            UIComponents.SetLockedText("Cosmetic.Locked");
            UIComponents.SetLockedTextRed();
            UIComponents.EnableUnlockButton();
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
            
            numberDecrement.Start(_lastCoins, SkinManager.Instance.GetCoins());
        }

        private void UnlockFailedToUnlock()
        {
            OnUnlockFailed?.Invoke();
            UIComponents.SetUnlockButtonInteractable(false);
            UIComponents.SetLockedText("Cosmetic.NotEnough");
            _skinButtonController.RemoveListenerFromAllButtons();
            UIComponents.RemoveListenerToUnlockButton(UnlockButton_OnClickHandler);
            UIComponents.RemoveListenerBackButton(TriggerMainMenuButtonPressed);
            UIComponents.RemoveScrollListener(TriggerOnScroll);
            
            TimerManager.Add(new TimerData(1.5f, () =>
            {
                UIComponents.SetUnlockButtonInteractable(true);
                UIComponents.SetLockedTextRed();
                UIComponents.SetLockedText("Cosmetic.Locked");
                UIComponents.EnableUnlockButton();
                UIComponents.AddListenerToUnlockButton(UnlockButton_OnClickHandler);
                UIComponents.AddListenerToBackButton(TriggerMainMenuButtonPressed);
                UIComponents.AddScrollListener(TriggerOnScroll);
                _skinButtonController.AddListenerToAllButtons();
            }));
        }

        #endregion

        #region FirstOpen

        private void HandleFirstOpen()
        {
            UIComponents.AddListenerToCloseFirstOpenButton(OnFirstPanelClosed);
            UIComponents.SetInteractiveCloseFirstOpenButton(false);
            UIComponents.SetActiveFirstOpenPanel(true);
            TimerManager.Add(new TimerData(1f, 
                () => UIComponents.SetInteractiveCloseFirstOpenButton(true)));
        }

        private void OnFirstPanelClosed()
        {
            OnSkinSelected?.Invoke(-1);
            UIComponents.SetActiveFirstOpenPanel(false);
        }

        #endregion
        
        private void ButtonControllerOnSkinSelectedHandler(SkinType skinSelected)
        {
            OnSkinSelected?.Invoke((int)skinSelected);
            UpdateDescriptionText(skinSelected);
            
            if (SkinManager.Instance.GetIsLocked((int)skinSelected))
            {
                UIComponents.SetLockedTextRed();
                UIComponents.EnableUnlockButton();
                UIComponents.SetLockedText("Cosmetic.Locked");
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
            var skinInfo = SkinManager.Instance.GetSkinInformationByType(skinType);
            if (skinInfo == null)
            {
                UIComponents.SetDescriptionText("Null");
                return;
            }

            var descriptionCode = skinInfo.DescriptionCode;
            UIComponents.SetDescriptionText(descriptionCode);
        }


        private void UpdateCoinsText(uint coinsAmount) 
            => UIComponents.SetCoinsText("Cosmetic.StoredCoins", coinsAmount);

        private void UpdateRequirementText(SkinType skinType)
        {            
            var skinInfo = SkinManager.Instance.GetSkinInformationByType(skinType);
            if (skinInfo == null)
            {
                UIComponents.SetRequirementText("Cosmetic.Requirement", int.MaxValue, "Cosmetic.Coins.Multiple");
                return;
            }
            
            var coinsPrize = skinInfo.UnlockPrice;
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
        
        private void TriggerOnScroll(int scrollDirection) => OnScroll?.Invoke(scrollDirection);
        
        private void OnLanguageChanged()
        {
            _skinButtonController.UpdateButtonsName();
        }
        
        #endregion
    }
}