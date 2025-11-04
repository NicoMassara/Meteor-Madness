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

        public void LoadTutorial()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.Tutorial, true);
        }

        public void LoadGameMode()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.GameMode, true);
        }

        public void LoadMainMenu()
        {
            GameScreenEventCaller.SetGameScreen(ScreenType.MainMenu, true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}