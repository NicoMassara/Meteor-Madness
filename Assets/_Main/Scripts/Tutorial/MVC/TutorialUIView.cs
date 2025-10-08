using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialUIView : ManagedBehavior, IObserver
    {
        [Header("Main Panel")]
        [SerializeField] private GameObject mainPanel;
        [Space]
        [Header("Sub Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject movementPanel;
        [SerializeField] private GameObject abilityPanel;
        [SerializeField] private GameObject finishPanel;
        [Space] 
        [Header("Buttons")] 
        [SerializeField] private Button nextButton;
        [SerializeField] private Button[] mainMenuButtons;

        private GameObject _currentActivePanel;

        public event Action OnNext;
        
        private void Awake()
        {
            nextButton.onClick.AddListener(NextButtonOnClickHandler);

            foreach (var button in mainMenuButtons)
            {
                button.onClick.AddListener(FinishButtonOnClickHandler);
            }
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case TutorialObserverMessage.Start:
                    HandleStart();
                    break;
                case TutorialObserverMessage.Movement:
                    HandleMovement();
                    break;
                case TutorialObserverMessage.Ability:
                    HandleAbility();
                    break;
                case TutorialObserverMessage.Finish:
                    HandleFinish();
                    break;
                case TutorialObserverMessage.Disable:
                    HandleDisable();
                    break;
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;
            }
        }

        private void HandleEnable()
        {
            mainPanel.SetActive(true);
        }

        private void HandleStart()
        {
            SetActivePanel(startPanel);
        }
        
        private void HandleMovement()
        {
            SetActivePanel(movementPanel);
        }
        
        private void HandleAbility()
        {
            SetActivePanel(abilityPanel);
        }

        private void HandleFinish()
        {
            SetActivePanel(finishPanel);
        }
        
        private void HandleDisable()
        {
            DisableActivePanel();
            mainPanel.SetActive(false);
        }

        private void SetActivePanel(GameObject input)
        {
            _currentActivePanel?.SetActive(false);
            _currentActivePanel = input;
            _currentActivePanel?.SetActive(true);
        }

        private void DisableActivePanel()
        {
            _currentActivePanel?.SetActive(false);
            _currentActivePanel = null;
        }

        #region Handlers

        private void NextButtonOnClickHandler()
        {
            OnNext?.Invoke();
        }
        
        private void FinishButtonOnClickHandler()
        {
            GameManager.Instance.LoadMainMenu();
        }

        #endregion
    }
}