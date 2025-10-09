using System.Collections;
using _Main.Scripts.Gameplay.AutoTarget;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldMovement : ManagedBehavior, IUpdatable, IFixedUpdatable
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [Header("Scriptable Objects")]
        [SerializeField] private ShieldMovementDataSo movementData;
        [Header("Values")]
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToEnableSuperShield;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToDisableSuperShield;
        [Range(0, 1000)]
        [SerializeField] private float decayConstant = 100f;
        [Space]
        [SerializeField] private ProjectileDetectorData detectorData;
        
        private ProjectileDetector _projectileDetector;
        private ShieldMovementComponent _movement;
        private ShieldSpeeder _shieldSpeeder;
        private IUpdatable updatableImplementation;
        private bool _isPlayerInputDisable;
        private bool _automaticEnable;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;
        public UpdateGroup SelfFixedUpdateGroup { get; }= UpdateGroup.Shield;

        private void Awake()
        {
            _movement = new ShieldMovementComponent(spriteContainer.transform, movementData);
            _shieldSpeeder = new ShieldSpeeder(_movement,timeToEnableSuperShield,timeToDisableSuperShield,decayConstant);
            _projectileDetector = new ProjectileDetector(detectorData,_movement);
        }
        
        private void Start()
        {
            _projectileDetector.OnTargetFound += Detector_OnTargetFoundHandler;
            _projectileDetector.OnTargetLost += Detector_OnTargetLostHandler;
        }
        
        public void ManagedUpdate()
        {
            _movement.Update(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }

        public void ManagedFixedUpdate()
        {
            if (_automaticEnable)
            {
                Debug.Log("Here");
                _projectileDetector.CheckForProjectile();
            }
        }
        
        public void SetAutomaticEnable(bool automaticEnable)
        {
            Debug.Log("Here");
            _automaticEnable = automaticEnable;
        }
        
        #region Movement

        public void Rotate(int direction)
        {
            if (_isPlayerInputDisable == false)
            {
                _movement.HandleMove(direction, CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            }
        }

        public void ForceStop()
        {
            _movement.HandleMove(0,0);
        }

        public void RestartPosition()
        {
            _movement.Restart();
        }

        #endregion

        #region Speeder

        public void IncreaseSpeed(float deltaTime)
        {
            _shieldSpeeder.IncreaseSpeed(deltaTime);
        }

        public void DecreaseSpeed(float deltaTime)
        {
            _shieldSpeeder.DecreaseSpeed(deltaTime);
        }

        public void RestartSpeedValues()
        {
            _shieldSpeeder.RestartValues();
        }

        #endregion
        
        #region Coroutine

        public void AutoCorrection()
        {

            StartCoroutine(Coroutine_AutoCorrection());
        }

        public void RotateTowardsNearestProjectileSlot()
        {
            StartCoroutine(Coroutine_RotateTowardsNearestProjectileSlot());
        }

        private IEnumerator Coroutine_AutoCorrection()
        {
            _isPlayerInputDisable = true;
            var currentDirection = _projectileDetector.GetSlotDirection();
            var initialDiff = _projectileDetector.GetSlotDiff();

            while (currentDirection != 0 && _isPlayerInputDisable == true)
            {
                var distanceRatio = (float)_projectileDetector.GetSlotDiff() / initialDiff;
                distanceRatio = 1 -distanceRatio;
                var multiplier = MathfCalculations.Remap(distanceRatio, 0, 1f, 1, 1.75f);
                currentDirection = _projectileDetector.GetSlotDirection() * 10;
                _movement.HandleMove((int)currentDirection * multiplier,
                    CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                yield return null;
            }
            
            ForceStop();
            
            _isPlayerInputDisable = false;
        }
        
        private IEnumerator Coroutine_RotateTowardsNearestProjectileSlot()
        {
            var meteorSlot = _projectileDetector.GetNearestProjectileSlot();
            
            if (meteorSlot > -1)
            {
                _movement.SetSpeedMultiplier(0.5f);
            
                while (meteorSlot != _movement.GetCurrentSlot())
                {
                    _movement.HandleMove(1, CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                    yield return null;
                }
            
                _movement.HandleMove(0, 0);
                _movement.SetSpeedMultiplier(1);
            }
            
            CustomTime.SetChannelTimeScale(UpdateGroup.Ability, 1);
        }
        

        #endregion
        
        #region Handlers

        private void Detector_OnTargetLostHandler()
        {

        }

        private void Detector_OnTargetFoundHandler()
        {
            AutoCorrection();
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectorData.CheckRadius);
        }

        #endregion

    }
}