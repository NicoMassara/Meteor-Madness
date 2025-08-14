using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.Menu
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(ResumeGame);
            mainMenuButton.onClick.AddListener(MainMenu);
        }

        private void Start()
        {
            pauseMenuPanel.SetActive(false);
        }

        public void ResumeGame()
        {
            GameManager.Instance.SetPaused(false);
            pauseMenuPanel.SetActive(false);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}