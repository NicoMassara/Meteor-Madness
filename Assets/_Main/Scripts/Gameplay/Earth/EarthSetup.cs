﻿using System;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    [RequireComponent(typeof(EarthView))]
    public class EarthSetup : MonoBehaviour
    {
        private EarthMotor _motor;
        private EarthController _controller;
        private EarthView _view;
        
        private void Awake()
        {
            _view = GetComponent<EarthView>();
            
            _motor = new EarthMotor();
            _motor.Subscribe(_view);
            _controller = new EarthController(_motor);
            _view.SetController(_controller);
            
            SetEventBus();
        }

        private void Update()
        {
            _motor.Execute();
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventBus = GameManager.Instance.EventManager;
            
            eventBus.Subscribe<MeteorCollision>(EventBus_OnMeteorCollision);
            eventBus.Subscribe<EarthRestart>(EventBus_OnEarthRestart);
        }

        private void EventBus_OnEarthRestart(EarthRestart input)
        {
            _controller.RestartHealth();
        }

        private void EventBus_OnMeteorCollision(MeteorCollision input)
        {
            _controller.HandleCollision(GameManager.Instance.GetMeteorDamage(), input.Position, input.Rotation);
        }
        
        #endregion
    }
}