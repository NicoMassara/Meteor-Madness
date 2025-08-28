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

        public bool IsShieldExtended { get; set; }
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
                ResetStreak();
            }
        }

        public void ResetStreak()
        {
            _currentExtendedHitCount = 0;
            _currentStreak = 0;
            _initialStreak = 0;
            IsShieldExtended = false;
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

        public UnityAction OnHit;

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
            var currValue = pithCurve.Evaluate(currentRatio);
            
            if(currentRatio >= 1 && _shieldStreakController.IsShieldExtended)
            {
                _shieldStreakController.IncreaseExtendedHitCount();
            }
            else if (currentRatio >= 1 && !_shieldStreakController.IsShieldExtended)
            {
                _canExtendShield = true;
                ExtendShield();
                _shieldStreakController.IsShieldExtended = true;
            }

            shieldHitSound.PlaySound(1f,currValue);
            OnHit?.Invoke();
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
            sprite.transform.localScale = new Vector3(1,1,1);
            _shieldStreakController.ResetStreak();
        }

        #region Handlers

        private void ShieldStreakController_OnResetHandler()
        {
            sprite.transform.localScale = new Vector3(1,1,1);
        }

        #endregion

        public void RestartPosition()
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}