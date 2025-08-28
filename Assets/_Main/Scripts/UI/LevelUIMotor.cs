using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.UI
{
    public class LevelUIMotor : MonoBehaviour
    {
        [Header("UI Elements")]
        [Space]
        [Header("Panels")]
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject deathPanel;
        [SerializeField] private GameObject restartSubPanel;
        [SerializeField] private GameObject pausePanel;
        [Header("Texts")]
        [SerializeField] private Text countdownText;
        [SerializeField] private Text playPointsText;
        [SerializeField] private Text deathPointsText;
        [SerializeField] private Text deathText;
        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior menuSound;
        
        private GameObject _currentPanel;
        private int _displayedPoints;
        public UnityAction OnRestartPressed;

        private void Start()
        {
            playPanel.SetActive(false);
            deathPanel.SetActive(false);
            countdownPanel.SetActive(false);
            deathText.text = $"{UITextValues.DeathText}";
            
            restartButton.onClick.AddListener(RestartButton_OnClickHandler);
            SetActiveCountdownPanel();
        }

        private void RestartButton_OnClickHandler()
        {
            menuSound.PlaySound();
            OnRestartPressed?.Invoke();
        }


        public void StartLevel()
        {
            SetActiveCountdownPanel();
            SetDisplayedPoints(0);
        }

        public void EndLevel()
        {
            SetActiveDeathPanel();
        }

        #region Displayed Points

        public int GetDisplayedPoints()
        {
            return _displayedPoints;
        }

        public void SetDisplayedPoints(int points)
        {
            _displayedPoints = points;
        }

        public void RestartPoints()
        {
            _displayedPoints = 0;
            UpdatePointsText(0);
        }

        public int GetDisplayedPointsFromText()
        {
            // Reads the number currently shown in the text
            if (int.TryParse(playPointsText.text, out int val))
                return val;
            return 0;
        }

        #endregion

        #region Panels

        private void ChangeCurrentPanel(GameObject panel)
        {
            if (_currentPanel != null)
            {
                _currentPanel.SetActive(false);
            }
            
            _currentPanel = panel;
            _currentPanel.SetActive(true);
        }

        public void SetActiveCountdownPanel()
        {
            ChangeCurrentPanel(countdownPanel);
        }

        public void SetActivePlayPanel()
        {
            ChangeCurrentPanel(playPanel);
        }

        public void SetActiveDeathPanel()
        {
            ChangeCurrentPanel(deathPanel);
        }

        public void DisableCurrentPanel()
        {
            if (_currentPanel != null)
            {
                _currentPanel.SetActive(false);
            }
        }

        public void SetActiveRestartSubPanel(bool isActive)
        {
            restartSubPanel.SetActive(isActive);
        }

        public void EnablePausePanel()
        {
            pausePanel.SetActive(true);
        }

        #endregion
        
        #region Update Texts

        public void UpdatePointsText(int points)
        {
            playPointsText.text = $"{UITextValues.Points}: {points:D6}";
        }

        public void UpdateCountdownText(int elapsedTime)
        {
            if (elapsedTime > 0)
            {
                countdownText.text = $"{UITextValues.StartText} {elapsedTime}...";
            }
            else
            {
                countdownText.text = "Defend!";
            }
        }

        public void UpdateDeathPointsText(int points)
        {
            deathPointsText.text = $"{UITextValues.DeathPoints}: {points:D6}";
        }

        #endregion
    }
}