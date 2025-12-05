using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyTools;
using _Main.Scripts.CustomId;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public bool CanPlay { get; set; }
        public bool IsPaused { get; private set; }
        public bool HadCorruptedSaveData { get; set; }

        public EventBusManager EventManager { get; private set; }
        public IInputReader InputReader { get; private set; }
        
        // Game Stored Data

        public GeneratedId CurrentScoreSecuredId { get; set; }
        
        private GeneratedId _highScoreSecuredId;

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
        
        public void LoadDefeatScreen()
        {
            LoadGameScreen(ScreenType.Defeat);
        }
        
        public void LoadPauseScreen()
        {
            LoadGameScreen(ScreenType.Pause);
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
            SetPauseChannels(true);
            IsPaused = true;
        }

        public void UnpauseGame()
        {
            SetPauseChannels(false);
            IsPaused = false;
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

        public void ClearScoreData()
        {
            CurrentScoreSecuredId = null;
        }

        public bool GetHasNewHighScore()
        {
            if(SecureValueManager.GetDoesContainValue<float>(CurrentScoreSecuredId, out var currentScore) == false) 
                return false;
            
            if(SecureValueManager.GetDoesContainValue<float>(GetHighScoreSecuredId(), out var highScore) == false) 
                return false;
            
            return currentScore > highScore;
        }

        public GeneratedId GetHighScoreSecuredId()
        {
            if (_highScoreSecuredId == null)
            {
                var temp = DataManager.Instance.GetData<DataManager.ScoreSaveData>(DataManager.SaveDataType.Score);
                _highScoreSecuredId = SecureValueManager.RegisterValue(temp.HighScore);
            }

            return _highScoreSecuredId;
        }
    }
}