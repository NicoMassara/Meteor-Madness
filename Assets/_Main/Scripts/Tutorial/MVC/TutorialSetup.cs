using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Tutorial.MVC
{
    [RequireComponent(typeof(TutorialView))]
    [RequireComponent(typeof(TutorialUIView))]
    public class TutorialSetup : ManagedBehavior
    {
        private TutorialMotor _motor;
        private TutorialController _controller;
        private TutorialView _view;
        private TutorialUIView _ui;
        
        private void Awake()
        {
            _view = GetComponent<TutorialView>();
            _ui = GetComponent<TutorialUIView>();
            
            _motor = new TutorialMotor();
            _controller = new TutorialController(_motor);
            
            SetEventBus();
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
        }

        #endregion
    }
}