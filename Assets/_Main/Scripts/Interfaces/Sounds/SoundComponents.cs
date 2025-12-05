using System;

namespace _Main.Scripts.Interfaces.Sounds
{
    public interface ISoundComponent { }

    public interface IProjectileSounds : ISoundComponent
    {
        public event Action OnStart;
        public event Action OnStop;
    }
    
    // Tutorial
    
    public interface ITutorialSounds : ISoundComponent
    {
        public event Action OnTutorialEnable;
        public event Action OnTutorialFinished;
    }
    
    // Pause
    
    public interface IPausePanelUISounds : ISoundComponent
    {
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;
    }
    
    // Multi Page
    
    public interface IMultiPageUISounds : ISoundComponent
    {
        public event Action OnPreviousButtonPressed;
        public event Action OnNextButtonPressed;
    }

    // Settings
    
    public interface ISettingsUISounds : ISoundComponent
    {
        public event Action<float> OnVolumeChanged;
        public event Action<int> OnLanguageChanged;
        public event Action<bool> OnVibrationChanged;
        public event Action OnBackButtonPressed;
    }
    
    // Ability
    
    public interface IAbilitySounds : ISoundComponent
    {
        public event Action OnAbilityTriggered;
        public event Action OnAbilityAdded;
        public event Action OnTimeSlowDown;
        public event Action OnTimeSpeedUp;
    }
    
    // Shield
    
    public interface IShieldSounds : ISoundComponent
    {
        public event Action<bool> OnShieldActivated;
        public event Action OnRotate;
        public event Action OnDeflect;
        public event Action<AbilityType> OnAbilityStarted;
        public event Action<AbilityType> OnAbilityRunning;
        public event Action OnAbilityFinished;
    }
    
    // Earth

    public interface IEarthSounds : ISoundComponent
    {
        public event Action OnCollision;
        public event Action OnHealing;
        public event Action OnDestruction;
    }
    
    // Main Menu
    
    public interface IMainMenuUISounds : ISoundComponent
    {
        public event Action OnConfirmButtonClicked;
        public event Action OnCancelButtonClicked;
        public event Action OnBackButtonClicked;
    }
    
    public interface IMainMenuSounds : ISoundComponent
    {
        public event Action OnMainMenuEnable;
    }

    // Game Mode
    
    public interface IGameModeSounds : ISoundComponent
    {
        public event Action OnInitialized;
        public event Action OnCountDownStarted;
        public event Action<float> OnCountdownUpdated;
        public event Action OnCountdownUpdatedFinished;
        public event Action OnStopMusic;
        public event Action OnPlayMusic;
    }

    public interface IGameModeUISounds : ISoundComponent
    {
        public event Action OnPauseButtonPressed;
    }
    
    // Cosmetics

    public interface ICosmeticUISounds : ISoundComponent
    {
        public event Action OnMainMenuButtonPressed;
    }
    
    // Defeat
    
    public interface IDefeatAnimationSounds : ISoundComponent
    {
        public event Action OnPlayMusic;
        public event Action OnStopMusic;
    }

    public interface IDefeatUiSounds : ISoundComponent
    {
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
    }
}