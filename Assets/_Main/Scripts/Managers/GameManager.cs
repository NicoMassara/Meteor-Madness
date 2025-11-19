using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public bool CanPlay { get; set; }
        public bool IsPaused { get; set; }
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

        private void LoadGameScreen(ScreenType type)
        {
            GameScreenEventCaller.EnableScreen(type, EventRequestType.Requested);
        }

        #endregion

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}