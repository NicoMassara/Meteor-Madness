using System;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.GlobalValues.Tools;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public bool CanPlay { get; set; }
        public bool IsPaused { get; private set; }

        public event Action OnPaused;
        public event Action OnResumed;
        public bool HadCorruptedSaveData { get; set; }

        public EventBusManager EventManager { get; private set; }
        public IInputReader InputReader { get; private set; }
        
        public bool AntiEpileptic { get; set; } = true;
        public StatsController StatsController { get; private set; } 
        public FlagsController FlagsController { get; private set; }
        
        private void Awake()
        {
            EventManager = new EventBusManager();
            StatsController = new StatsController();
            FlagsController = new FlagsController();
        }
        
        public void SetInputReader(IInputReader inputReader)
        {
            if(inputReader == null) return;
            
            InputReader = inputReader;
        }

        #region Screen Loading

        public void LoadTutorial() => LoadGameScreen(ScreenType.Tutorial);
        public void LoadGameMode() => LoadGameScreen(ScreenType.GameMode);
        public void LoadMainMenu() => LoadGameScreen(ScreenType.MainMenu);
        public void LoadStatsScreen() => LoadGameScreen(ScreenType.Stats);
        public void LoadCosmeticMenu() => LoadGameScreen(ScreenType.Cosmetic);
        public void LoadOptionsMenu() => LoadGameScreen(ScreenType.OptionsMenu);
        public void LoadDefeatScreen() => LoadGameScreen(ScreenType.Defeat);
        public void LoadPauseScreen() => LoadGameScreen(ScreenType.Pause);
        public void LoadLastScreen() => GameScreenEventCaller.LoadLastScreen();
        private void LoadGameScreen(ScreenType type) => GameScreenEventCaller.EnableScreen(type, EventRequestType.Requested);

        #endregion

        public void PauseGame()
        {
            SetPauseChannels(true);
            IsPaused = true;
            OnPaused?.Invoke();
        }

        public void ResumeGame()
        {
            SetPauseChannels(false);
            IsPaused = false;
            OnResumed?.Invoke();
        }

        private void SetPauseChannels(bool isPaused)
        {
            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Effects,
                
            }, isPaused);
        }

        public void QuitGame()
        {
            QuitUtility.Quit();
        }
    }
}