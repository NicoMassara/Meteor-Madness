using System;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Sphere
{
    [RequireComponent(typeof(AbilitySphereView))]
    public class AbilitySphereSetup : ManagedBehavior
    {
        [SerializeField] private LayerMask shieldLayerMask;
        [SerializeField] private LayerMask earthLayerMask;
        
        private AbilitySphereMotor _motor;
        private AbilitySphereController _controller;
        private AbilitySphereView _view;

        private void Awake()
        {
            _view = GetComponent<AbilitySphereView>();
            _motor = new AbilitySphereMotor();
            _controller = new AbilitySphereController(_motor,shieldLayerMask,earthLayerMask);
            
            _view.SetController(_controller);
            _motor.Subscribe(_view);
        }
    }
}