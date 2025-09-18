using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
        
        private readonly Timer _resumeTimer = new Timer();
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
            _resumeTimer.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
        }

        private void ResumeGame()
        {
            _resumeTimer.OnEnd -= ResumeGame;
            pauseMenuPanel.SetActive(false);
        }
        private void MainMenu()
        {
            _resumeTimer.OnEnd -= MainMenu;
            SceneManager.LoadScene("MainMenu");
        }

        public void ResumeGameOnClickHandler()
        {
            menuSound.PlaySound();
            _resumeTimer.Set(UIPanelTimeValues.ClosePauseMenu);
            _resumeTimer.OnEnd += ResumeGame;
        }

        public void MainMenuOnClickHandler()
        {
            menuSound.PlaySound();
            _resumeTimer.Set(UIPanelTimeValues.ClosePauseMenu);
            _resumeTimer.OnEnd += MainMenu;
        }


    }
}