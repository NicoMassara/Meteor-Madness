using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.Menu
{
    public class PauseMenuController : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior menuSound;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;

        private void Awake()
        {
            resumeButton.onClick.AddListener(ResumeGameOnClickHandler);
            mainMenuButton.onClick.AddListener(MainMenuOnClickHandler);
        }

        private void Start()
        {
            pauseMenuPanel.SetActive(false);
        }
        
        public void ManagedUpdate()
        {
        }

        private void ResumeGame()
        {
            pauseMenuPanel.SetActive(false);
        }
        private void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ResumeGameOnClickHandler()
        {
            menuSound.PlaySound();
            TimerManager.AddTimer(new TimerData
            {
                Time = UIPanelTimeValues.ClosePauseMenu,
                OnEndAction = ResumeGame
            },SelfUpdateGroup);
        }

        public void MainMenuOnClickHandler()
        {
            menuSound.PlaySound();
            TimerManager.AddTimer(new TimerData
            {
                Time = UIPanelTimeValues.ClosePauseMenu,
                OnEndAction = MainMenu
            },SelfUpdateGroup);
        }
    }
}