using System;

namespace _Main.Scripts.Interfaces.Vibration
{
    //=== Base ===//

    public interface IVibrationComponent { }


    //=== Cosmetic ===//

    public interface ICosmeticUIVibration : IVibrationComponent
    {
        public event Action OnMainMenuButtonPressed;
    }
    
    //=== Defeat ===///

    public interface IDefeatUIVibration : IVibrationComponent
    {
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
    }
    
    //=== Ability ===//
    
    public interface IAbilityVibration : IVibrationComponent
    {
        public event Action OnAbilityFinished;
        public event Action OnAbilityTriggered;
        public event Action OnAbilityAdded;
    }
    
    //=== Earth ===//
    
    public interface IEarthVibration : IVibrationComponent
    {
        public event Action OnPreDestruction;
        public event Action OnCollision;
        public event Action OnDestruction;
    }
    
    //=== GameMode ===//
    
    public interface IGameModeUIVibration : IVibrationComponent
    {
        public event Action OnPauseButtonPressed;
        public event Action OnPointsAdded;
    }
    
    public interface IGameModeVibration : IVibrationComponent
    {
        public event Action OnCountDownFinished;
        public event Action<float> OnCountdownUpdated;
    }
    
    //=== Shield ===//
    
    public interface IShieldVibration : IVibrationComponent
    {
        public event Action OnRotate;
        public event Action OnDeflect;
    }
    
    //=== MainMenu ===///
    
    public interface IMainMenuUiVibration : IVibrationComponent
    {
        public event Action OnConfirmButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action OnBackButtonClicked;
    }
    
    //=== MultiPage ===//
    
    public interface IMultiPageUIVibration : IVibrationComponent
    {
        public event Action OnNextButtonPressed;
        public event Action OnPreviousButtonPressed;  
    }
    
    //=== Settings ===//
    
    public interface ISettingsUiVibration: IVibrationComponent
    {
        public event Action<float> OnVolumeChanged;
        public event Action<int> OnLanguageChanged;
        public event Action<bool> OnVibrationChanged;
        public event Action OnBackButtonPressed;  
    }
    
    //=== Pause ===//
    
    public interface IPauseUIVibration : IVibrationComponent
    {
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;
    }
    
    //=== Tutorial ===//
    
    public interface ITutorialUIVibration : IVibrationComponent
    {
        
    }
    
}