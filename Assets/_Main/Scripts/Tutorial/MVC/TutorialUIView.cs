using System;
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
        [SerializeField] private Button finishButton;

        private GameObject _currentActivePanel;

        public event Action OnNext;
        public event Action OnFinish;
        
        private void Awake()
        {
            nextButton.onClick.AddListener(NextButtonOnClickHandler);
            finishButton.onClick.AddListener(FinishButtonOnClickHandler);
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
            }
        }

        private void HandleStart()
        {
            mainPanel.SetActive(true);
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
            mainPanel.SetActive(false);
        }

        private void SetActivePanel(GameObject input)
        {
            _currentActivePanel?.SetActive(false);
            _currentActivePanel = input;
            _currentActivePanel?.SetActive(true);
        }

        #region Handlers

        private void NextButtonOnClickHandler()
        {
            OnNext?.Invoke();
        }
        
        private void FinishButtonOnClickHandler()
        {
            OnFinish?.Invoke();
        }

        #endregion
    }
}