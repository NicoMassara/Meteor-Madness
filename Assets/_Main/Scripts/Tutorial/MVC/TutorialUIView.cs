using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;
using TMPro;
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
        [SerializeField] private GameObject hintPanel;
        [Space] 
        [Header("Buttons")] 
        [SerializeField] private Button startButton;
        [SerializeField] private Button mainMenuButtons;
        [Header("Text Components")]
        [SerializeField] private TMP_Text hintText;

        private GameObject _currentActivePanel;
        public event Action OnStartTutorialButtonPressed;
        
        private void Awake()
        {
            startButton.onClick.AddListener(NextButtonOnClickHandler);
            mainMenuButtons.onClick.AddListener(FinishButtonOnClickHandler);
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
                case TutorialObserverMessage.MultiPage:
                    HandleMultiPage();
                    break;
                case TutorialObserverMessage.SphereDeflected:
                    HandleSphereDeflected();
                    break;
                case TutorialObserverMessage.AbilityRunning:
                    HandleAbilityRunning();
                    break;
                case TutorialObserverMessage.Disable:
                    HandleDisable();
                    break;
                case TutorialObserverMessage.Enable:
                    HandleEnable();
                    break;
            }
        }

        private void HandleAbilityRunning()
        {
            DisableActivePanel();
        }

        private void HandleSphereDeflected()
        {
            SetHintText("Trigger the Super Shield!");
        }

        private void HandleStart()
        {
            SetActivePanel(startPanel);
        }

        private void HandleMultiPage()
        {
            DisableActivePanel();
        }

        private void HandleEnable()
        {
            mainPanel.SetActive(true);
        }
        
        private void HandleMovement()
        {
            SetHintText("Try Moving and Deflect a Meteor!");
        }
        
        private void HandleAbility()
        {
            SetHintText("Try To Deflect the mysterious Sphere!");
        }
        
        private void HandleDisable()
        {
            DisableActivePanel();
            mainPanel.SetActive(false);
        }

        private void SetHintText(string text)
        {
            hintText.text = text;
            SetActivePanel(hintPanel);
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
            SoundEventCaller.PlayUIButton(UISoundType.Accept);
            OnStartTutorialButtonPressed?.Invoke();
        }
        
        private void FinishButtonOnClickHandler()
        {
            GameManager.Instance.LoadMainMenu();
        }

        #endregion
    }
}