using System;
using UnityEngine;

namespace _Main.Scripts.Comet
{
    [RequireComponent(typeof(CometView))]
    public class CometSetup : MonoBehaviour
    {
        [SerializeField] private LayerMask cometWallMask;
        private CometMotor _motor;
        private CometController _controller;
        private CometView _view;

        private void Awake()
        {
            _view = GetComponent<CometView>();
            
            _motor = new CometMotor();
            _controller = new CometController(_motor, cometWallMask);
            
            _motor.Subscribe(_view);
            _view.SetController(_controller);
        }
    }
}