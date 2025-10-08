using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.GameScreens
{
    [RequireComponent(typeof(GameScreenView))]
    public class GameScreenSetup : ManagedBehavior, IUpdatable
    {
        private GameScreenMotor _motor;
        private GameScreenController _controller;
        private GameScreenView _view;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;

        private void Awake()
        {
            _motor = new GameScreenMotor();
            _controller = new GameScreenController(_motor);
            _view = GetComponent<GameScreenView>();
            
            SetEventBus();
        }

        private void Start()
        {
            _motor.Subscribe(_view);
            _controller.Initialize();
        }

        public void ManagedUpdate()
        {
            _controller.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            
            eventManager.Subscribe<GameScreenEvents.SetGameScreen>(EventBus_OnSetGameScreen);
        }

        private void EventBus_OnSetGameScreen(GameScreenEvents.SetGameScreen input)
        {
            switch (input.Index)
            {
                case 0:
                    _controller.TransitionToMainMenu();
                    break;
                case 1:
                    _controller.TransitionToGameplay();
                    break;
                case 2:
                    _controller.TransitionToTutorial();
                    break;
                default:
                    Debug.LogWarning("GameScene Index is out of range.");
                    break;
            }
            
        }
        #endregion
    }
}