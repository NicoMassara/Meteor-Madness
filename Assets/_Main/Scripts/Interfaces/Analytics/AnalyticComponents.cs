using System;

namespace _Main.Scripts.Interfaces.Analytics
{
    public interface IAnalyticComponent { }
    
    public interface IGameModeAnalytics : IAnalyticComponent
    {
        public event Action OnInitialized;
        public event Action OnPaused;
        public event Action OnResume;
        public event Action OnScoreSaved;
        public event Action OnGameInterrupted;
        public event Action<float> OnPointGained;
        public event Action<AbilityType> OnAbilityTriggered;
        public event Action<int> OnLevelUpdate;
        public event Action<uint> OnStreakUpdated;
    }

    public interface ITutorialAnalytics : IAnalyticComponent
    {
        public event Action OnTutorialFinished;
    }
    
    public interface ICosmeticsAnalytics : IAnalyticComponent
    {
        public event Action OnFirstOpen;
        public event Action<string> OnCosmeticChanged;
    }
    
    public interface IDefeatAnalytics : IAnalyticComponent
    {
        public event Action OnGameSaved;
        public event Action OnRestart;
        public event Action OnMainMenu;
    }
    
    public interface IMainMenuAnalytics : IAnalyticComponent
    {
        public event Action OnLoreOpened;
        public event Action OnLoreClosed;
    }
}