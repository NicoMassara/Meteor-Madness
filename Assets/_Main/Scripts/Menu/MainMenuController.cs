using _Main.Scripts.Managers;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject lorePanel;
        [Header("Buttons")]
        [SerializeField] private Button playButton;
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
            loreButton.onClick.AddListener(SetActiveLorePanel);
            backButton.onClick.AddListener(SetActiveMainPanel);
            exitButton.onClick.AddListener(QuitGame);
            
            SetEventBus();
        }

        private void Initialize()
        {
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
            menuSound.PlaySound();
            TimerManager.Add(new TimerData
            {
                Time = GameTimeValues.TimeToLoadGameScene,
                OnEndAction = ()=> GameManager.Instance.LoadGameplay()
            });
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

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;

            eventManager.Subscribe<MainMenuScreenEnable>(EventBus_OnMainMenuScreen);
            eventManager.Subscribe<GameModeScreenEnable>(EventBus_OnGameplayScreen);
        }

        private void EventBus_OnGameplayScreen(GameModeScreenEnable input)
        {
            themeSound.StopSound();
            SetEnableMainPanel(false);
        }

        private void EventBus_OnMainMenuScreen(MainMenuScreenEnable input)
        {
            Initialize();
        }

        #endregion
    }
}