using System;
using _Main.Scripts.Cosmetics;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyTools;
using _Main.Scripts.CustomId;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
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

        public string DeathTitle { get; set; } = "No Title";
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

        #region Skin Values

        public uint GetStoredCoins() => SkinManager.Instance.GetCoins();
        public bool TryAddCoins(uint score) => SkinManager.Instance.TryAddCoins(score);
        public void SaveCoins() => SkinManager.Instance.SaveStoredCoins();

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