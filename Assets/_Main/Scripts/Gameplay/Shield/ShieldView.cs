using System;
using _Main.Scripts.Shaker;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldView : MonoBehaviour
    {
        [SerializeField] private GameObject spriteObject;
        [SerializeField] private ShakeDataSo shakeData;
        private ShakerController _shakerController;
        private ShieldMotor _motor;
        
        private void Awake()
        {
            _motor = GetComponent<ShieldMotor>();
        }

        private void Start()
        {
            _motor.OnHit += OnHitHandler;
            _shakerController = new ShakerController(transform);
            _shakerController.SetShakeData(shakeData);
        }

        private void Update()
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake();
            }
        }

        private void OnHitHandler()
        {
            _shakerController.StartShake();
        }
    }
}