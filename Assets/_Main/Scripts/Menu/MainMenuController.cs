using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject lorePanel;
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button loreButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button backButton;
        [Header("Values")]
        [SerializeField] private string gameplaySceneName;

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
        }

        private void Start()
        {
            mainPanel.SetActive(false);
            lorePanel.SetActive(false);
            SetActiveMainPanel();
            themeSound.PlaySound();
        }

        private void LoadGameplayScene()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void SetActivePanel(GameObject panel)
        {
            if (_currentPanel != null)
            {
                _currentPanel.SetActive(false);
            }
            _currentPanel = panel;
            _currentPanel.SetActive(true);
        }

        private void SetActiveMainPanel()
        {
            menuSound.PlaySound();
            SetActivePanel(mainPanel);
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
                OnEndAction = LoadGameplayScene
            });
        }

        private void QuitGame()
        {
            menuSound.PlaySound();
            Application.Quit();
        }
    }
}