using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class GameManager : ManagedBehavior
    {
        public static GameManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static GameManager _instance;
        
        public bool CanPlay { get; set; }
        public bool IsPaused { get; set; }
        private int _currentPoints;
        
        public EventBusManager EventManager { get; private set; }
        public IInputReader InputReader { get; private set; }
        
        private static GameManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(GameManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<GameManager>();
        }

        private void Awake()
        {
            EventManager = new EventBusManager();
            SceneLoader.LoadModules();
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