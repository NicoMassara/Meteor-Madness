using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.UI
{
    public class LevelUIMotor : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject countdownPanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject deathPanel;
        [Header("Texts")]
        [SerializeField] private Text countdownText;
        [SerializeField] private Text playPointsText;
        [SerializeField] private Text deathPointsText;
        
        private GameObject _currentPanel;
        private int _displayedPoints;

        private void Start()
        {
            playPanel.SetActive(false);
            deathPanel.SetActive(false);
            countdownPanel.SetActive(false);
        }

        #region Displayed Points

        public int GetDisplayedPoints()
        {
            return _displayedPoints;
        }

        public void SetDisplayedPoints(int finalPoints)
        {
            _displayedPoints = finalPoints * GameValues.VisualMultiplier;
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

        #endregion
        
        #region Update Texts

        public void UpdatePointsText(int points)
        {
            playPointsText.text = $"Save Points: {points}";
        }

        public void UpdateCountdownText(int elapsedTime)
        {
            if (elapsedTime > 0)
            {
                countdownText.text = $"{elapsedTime}...";
            }
            else
            {
                countdownText.text = "Defend!";
            }
        }

        public void UpdateDeathPointsText(int points)
        {
            deathPointsText.text = $"Points: {points}";
        }

        #endregion
    }
}