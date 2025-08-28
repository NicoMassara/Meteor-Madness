using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldView : MonoBehaviour
    {
        [SerializeField] private GameObject spriteObject;
        [Range(0,2f)]
        [SerializeField] private float shakeTime;
        [Range(0.1f, 200)] 
        [SerializeField] private float shakeIntensity = 20;
        [Range(0,.5f)]
        [SerializeField] private float shakeMagnitude = 0.075f;
        private ShieldMotor _motor;

        private float _shakeTimer;
        private Vector3 _startPosition;

        private void Awake()
        {
            _motor = GetComponent<ShieldMotor>();
        }

        private void Start()
        {
            _motor.OnHit += OnHitHandler;
            _startPosition = spriteObject.transform.localPosition;
        }

        private void Update()
        {
            if (_shakeTimer > 0)
            {
                HandleShake();

                if (_shakeTimer <= 0)
                {
                    spriteObject.transform.localPosition = _startPosition;
                }
            }
        }

        private void HandleShake()
        {
            _shakeTimer -= Time.deltaTime;
            spriteObject.transform.localPosition = GetShakeOffset(_shakeTimer) + _startPosition;
        }
        
        public Vector3 GetShakeOffset(float time)
        {
            float angle = (shakeIntensity * Mathf.PI * 2f) * time;
            float offsetX = Mathf.Sin(angle) * shakeMagnitude;
            float offsetY = Mathf.Cos(angle) * shakeMagnitude;
            return new Vector3(offsetX, offsetY, 0f);
        }

        private void OnHitHandler()
        {
            _shakeTimer = shakeTime;
        }
    }
}