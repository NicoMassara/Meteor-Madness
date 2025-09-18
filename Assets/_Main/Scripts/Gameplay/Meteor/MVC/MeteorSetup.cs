using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    [RequireComponent(typeof(MeteorView))]
    public class MeteorSetup : MonoBehaviour
    {
        [SerializeField] private LayerMask shieldLayerMask;
        [SerializeField] private LayerMask earthLayerMask;
        
        private MeteorMotor _motor;
        private MeteorController _controller;
        
        private MeteorView _view;

        private void Awake()
        {
            _view = GetComponent<MeteorView>();
            
            _motor = new MeteorMotor();
            _controller = new MeteorController(_motor, shieldLayerMask, earthLayerMask);
            
            _view.SetController(_controller);
            _motor.Subscribe(_view);
        }
    }
}