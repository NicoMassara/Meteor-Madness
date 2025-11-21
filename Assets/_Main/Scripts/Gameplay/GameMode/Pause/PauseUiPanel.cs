using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode.Pause
{
    public class PauseUiPanel : MonoBehaviour, IPausePanel
    {
        [Space(2)] 
        [Header("Texts Components")]
        [SerializeField] private TMP_Text scoreText;
        [Space]
        [Header("Buttons Components")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button mainMenuButtons;
        
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;

        public GameObject Panel => gameObject;

        public void SetScoreText(int points)
        {
            if(scoreText == null) return;

            scoreText.text = $"{LocalizationManager.Instance.GetText("Gameplay.Score")}: {points:D6}";
        }
        
        private void OnEnable()
        {
            resumeButton?.onClick.AddListener(() =>
            {
                OnResumeButtonPressed?.Invoke();
            });
            
            optionsButton?.onClick.AddListener(() =>
            {
                OnOptionsButtonPressed?.Invoke();
            });
            
            mainMenuButtons?.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
        }

        private void OnDisable()
        {
            resumeButton.onClick.RemoveAllListeners();
            optionsButton.onClick.RemoveAllListeners();
            mainMenuButtons.onClick.RemoveAllListeners();
        }
    }
}