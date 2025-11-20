using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public bool CanPlay { get; set; }
        public bool IsPaused { get; private set; }
        private int _currentPoints;
        
        public EventBusManager EventManager { get; private set; }
        public IInputReader InputReader { get; private set; }
        
        
        private void Awake()
        {
            EventManager = new EventBusManager();
        }
        
        public void SetInputReader(IInputReader inputReader)
        {
            if(inputReader == null) return;
            
            InputReader = inputReader;
        }

        #region Screen Loading

        public void LoadTutorial()
        {
            LoadGameScreen(ScreenType.Tutorial);
        }

        public void LoadGameMode()
        {
            LoadGameScreen(ScreenType.GameMode);
        }

        public void LoadMainMenu()
        {
            LoadGameScreen(ScreenType.MainMenu);
        }
        
        public void LoadCosmeticMenu()
        {
            LoadGameScreen(ScreenType.Cosmetic);
        }
        
        public void LoadOptionsMenu()
        {
            LoadGameScreen(ScreenType.OptionsMenu);
        }

        public void LoadLastScreen()
        {
            GameScreenEventCaller.LoadLastScreen();
        }

        private void LoadGameScreen(ScreenType type)
        {
            GameScreenEventCaller.EnableScreen(type, EventRequestType.Requested);
        }

        #endregion

        public void PauseGame()
        {
            SetPauseInChannels(true);
            IsPaused = true;
        }

        public void UnpauseGame()
        {
            SetPauseInChannels(false);
            IsPaused = false;
        }

        private void SetPauseInChannels(bool isPaused)
        {
            CustomTime.SetChannelPaused(new []
            {
                UpdateGroup.Gameplay,
                UpdateGroup.Ability, 
                UpdateGroup.Shield,
                UpdateGroup.Earth,
                UpdateGroup.Effects,
                UpdateGroup.Camera
                
            }, isPaused);
        }

        public void QuitGame()
        {
            Application.Quit();
        }


    }
}