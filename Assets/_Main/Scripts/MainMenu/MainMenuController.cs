using _Main.Scripts.Managers;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject lorePanel;
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button loreButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;

        [Header("Sounds")] 
        [SerializeField] private SoundBehavior themeSound;
        [SerializeField] private SoundBehavior menuSound;
        
        private GameObject _currentPanel;

        private void Awake()
        {
            playButton.onClick.AddListener(StartGame);
            tutorialButton.onClick.AddListener(StartTutorial);
            loreButton.onClick.AddListener(SetActiveLorePanel);
            backButton.onClick.AddListener(SetActiveMainPanel);
            exitButton.onClick.AddListener(QuitGame);
            
            GameEventCaller.Subscribe<GameScreenEvents.SetGameScreen>(EventBus_GameScreen_SetScreen);
        }

        private void Initialize()
        {            
            CameraEventCaller.ZoomIn();
            menuPanel.SetActive(false);
            lorePanel.SetActive(false);
            themeSound.PlaySound();
            SetEnableMainPanel(true);
            SetActivePanel(menuPanel);
        }

        private void SetActivePanel(GameObject panelObject)
        {
            if (_currentPanel != null)
            {
                _currentPanel.SetActive(false);
            }
            
            _currentPanel = panelObject;
            _currentPanel.SetActive(true);
        }

        private void SetActiveMainPanel()
        {
            menuSound.PlaySound();
            SetActivePanel(menuPanel);
        }

        private void SetActiveLorePanel()
        {
            menuSound.PlaySound();
            SetActivePanel(lorePanel);
        }

        private void StartGame()
        {
            SetEnableAllButtons(false);
            menuSound.PlaySound();
            TimerManager.Add(new TimerData
            {
                Time = GameConfigManager.Instance.GetGameplayData().GameTimeData.TimeToLoadGameScene,
                OnEndAction = ()=> GameManager.Instance.LoadGameMode()
            });
        }
        
        private void StartTutorial()
        {
            SetEnableAllButtons(false);
            menuSound.PlaySound();
            TimerManager.Add(new TimerData
            {
                Time = GameConfigManager.Instance.GetGameplayData().GameTimeData.TimeToLoadGameScene,
                OnEndAction = ()=> GameManager.Instance.LoadTutorial()
            });
        }

        private void SetEnableAllButtons(bool isEnable)
        {
            playButton.enabled = isEnable; 
            loreButton.enabled = isEnable; 
            backButton.enabled = isEnable; 
            exitButton.enabled = isEnable; 
        }

        private void QuitGame()
        {
            menuSound.PlaySound();
            Application.Quit();
        }

        private void SetEnableMainPanel(bool isActive)
        {
            mainPanel.SetActive(isActive);
        }

        private void EnableMainMenu()
        {
            SetEnableAllButtons(true);
            Initialize();
        }

        private void DisableMainMenu()
        {
            themeSound.StopSound();
            SetEnableMainPanel(false);
        }

        #region EventBus

        private void EventBus_GameScreen_SetScreen(GameScreenEvents.SetGameScreen input)
        {
            if (input.ScreenType == ScreenType.MainMenu)
            {
                EnableMainMenu();
            }
            else
            {
                DisableMainMenu();
            }
        }
        #endregion
    }
}