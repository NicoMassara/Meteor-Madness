using System;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldStreakController
    {
        private int _currentStreak;
        private int _initialStreak;

        private int _currentExtendedHitCount;

        public UnityAction OnReset;
        
        public float GetSteakRatio()
        {
            return (float)_currentStreak / GameValues.MaxStreakShield;
        }

        public void IncreaseStreak()
        {
            if (_initialStreak >= GameValues.StartStreakShield)
            {
                if (_currentStreak < GameValues.MaxStreakShield)
                {
                    _currentStreak++;
                }
            }
            
            _initialStreak++;
        }

        public void IncreaseExtendedHitCount()
        {
            _currentExtendedHitCount++;

            if (_currentExtendedHitCount >= GameValues.ExtendedMaxHit)
            {
                OnReset?.Invoke();
                _currentExtendedHitCount = 0;
            }
        }

        public void ResetStreak()
        {
            _currentStreak = 0;
            _initialStreak = 0;
        }
    }
    
    public class ShieldMotor : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private SoundBehavior shieldHitSound;
        [Header("Components")]
        [SerializeField] private GameObject sprite;
        [Header("Values")]
        [Range(1,10)]
        [SerializeField] private float rotateSpeed;
        [Range(0.1f, 2)] 
        [SerializeField] private float extendTime = 1.25f;
        [SerializeField] private AnimationCurve pithCurve;
        
        private ShieldStreakController _shieldStreakController;

        private bool _canExtendShield;

        private void Awake()
        {
            _shieldStreakController = new ShieldStreakController();
        }

        private void Start()
        {
            _shieldStreakController.OnReset += ShieldStreakController_OnResetHandler;
        }

        private void Update()
        {
            if (_canExtendShield)
            {
                ExtendShield();
            }
        }

        public void Rotate(float direction = 1)
        {
            transform.RotateAround(transform.position, Vector3.forward, (rotateSpeed/10) * direction);
        }

        public void SetActiveSprite(bool isActive)
        {
            sprite.SetActive(isActive);
        }

        public void HitShield()
        {
            _shieldStreakController.IncreaseStreak();
            var currentRatio = _shieldStreakController.GetSteakRatio();
            if(currentRatio >= 1)
            {
                _shieldStreakController.IncreaseExtendedHitCount();
            }
            var currValue = pithCurve.Evaluate(currentRatio);
            
            if (currentRatio >= 1)
            {
                _canExtendShield = true;
                ExtendShield();
            }

            shieldHitSound.PlaySound(1f,currValue);
        }

        private void ExtendShield()
        {
            var temp = Mathf.Lerp(sprite.transform.localScale.y,GameValues.ShieldExtend, Time.deltaTime * extendTime);

            if (Math.Abs(sprite.transform.localScale.y - GameValues.ShieldExtend) < 0.1f)
            {
                temp = GameValues.ShieldExtend;
                _canExtendShield = false;
            }
            
            sprite.transform.localScale = new Vector3(1,temp, 1);
        }

        public void ShrinkShield()
        {
            _shieldStreakController.ResetStreak();
            sprite.transform.localScale = new Vector3(1,1,1);
        }

        #region Handlers

        private void ShieldStreakController_OnResetHandler()
        {
            ShrinkShield();
        }

        #endregion
    }
}