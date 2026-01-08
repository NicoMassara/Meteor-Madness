using System;
using MeteorMadness.Contracts;
using MeteorMadness.Gameplay.Abilities;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Utilities;
using NicolasMassara.CustomActionManager;
using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldTrail))]
    public class ShieldAppereance : MonoBehaviour
    {
        private static readonly int AbilityColor = Shader.PropertyToID("_AbilityColor");
        private static readonly int MaterialOpacity = Shader.PropertyToID("_Opacity");

        [Header("Components")] 
        [SerializeField] private SpriteRenderer normalSprite;
        [SerializeField] private SpriteRenderer superSprite;
        [SerializeField] private SpriteRenderer abilityRenderer;

        [Header("Values")] 
        [Range(0, 1)] 
        [SerializeField] private float materialOpacityLerpTime = 0.25f;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToEnableSuperShield;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToDisableSuperShield;

        private ShieldTrail _shieldTrail;
        private IAbilityShield _abilityShield;

        private void Awake()
        {
            _shieldTrail = GetComponent<ShieldTrail>();

            _abilityShield = GetComponent<IAbilityShield>();
        }
        
        private void Start()
        {
            _abilityShield.OnAbilitySetActive += Shield_OnAbilitySetActiveHandler;
            _abilityShield.OnEnableSuperShield += Shield_OnEnableSuperShieldHandler;
            _abilityShield.OnDisableSuperShield += Shield_OnDisableSuperShieldHandler;
            _abilityShield.OnDisableAbility += Shield_OnDisableAbilityHandler;
            
            SetAbilityOpacity(0);
            SetSuperShieldOpacity(0);
            SetActiveSuperShield(false);
        }
        
        #region Material

        private void EnableMaterial()
        {
            var action = ActionBuilder.Start()
                .Do(new SetMaterialOpacityAction(SetAbilityOpacity, 
                    1,0,materialOpacityLerpTime))
                .Build();

            ActionManager.Add(action,ActionManager.UpdateType.Update);
        }
        
        private void DisableMaterial()
        {
            var action = ActionBuilder.Start()
                .Do(new SetMaterialOpacityAction(SetAbilityOpacity, 
                    0,1,materialOpacityLerpTime))
                .Build();

            ActionManager.Add(action,ActionManager.UpdateType.Update);
        }
        
        private class SetMaterialOpacityAction : IQueueAction
        {
            private readonly Action<float> _updateOpacity;
            private readonly float _targetValue;
            private readonly float _startValue;
            private readonly float _targetTime;
            private float _currentValue;
            private float _elapsedTime;
            
            public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;


            public SetMaterialOpacityAction(Action<float> updateOpacity, 
                float targetValue, float startValue, float targetTime)
            {
                _updateOpacity = updateOpacity;
                _targetValue = targetValue;
                _startValue = startValue;
                _targetTime = targetTime;
            }

            public void OnStart()
            {
                _currentValue = _startValue;
                CurrentStatus = ActionStatus.Running;
            }

            public ActionStatus OnUpdate(float deltaTime)
            {
                _elapsedTime += deltaTime;
                
                var ratio = Mathf.Clamp01(_elapsedTime / _targetTime);
                _currentValue = Mathf.Lerp(_startValue, _targetValue, ratio);

                if (ratio >= 1)
                {
                    CurrentStatus = ActionStatus.Success;
                }
                
                _updateOpacity.Invoke(_currentValue);


                return CurrentStatus;
            }

            public void OnInterrupt()
            {
                _updateOpacity.Invoke(_targetValue);
            }

            public IQueueAction Copy()
            {
                return null;
            }
        }
        
        private void SetMaterialColor(AbilityType abilityType)
        {
            var color = AbilityColorHelper.GetColor(abilityType);
            abilityRenderer.material.SetColor(AbilityColor, color);
        }

        private void SetAbilityOpacity(float opacity)
        {
            abilityRenderer.material.SetFloat(MaterialOpacity, opacity);
        }

        private void SetNormalShieldOpacity(float opacity)
        {
            normalSprite.material.SetFloat(MaterialOpacity, opacity);
        }
        
        private void SetSuperShieldOpacity(float opacity)
        {
            superSprite.material.SetFloat(MaterialOpacity, opacity);
        }

        private void SetTrailColor(AbilityType abilityType)
        {
            if (abilityType == AbilityType.None)
            {
                _shieldTrail.SetDefault();
                return;
            }

            var color = AbilityColorHelper.GetColor(abilityType);
            _shieldTrail.SetTrailColor(color);
        }
        

        private void SetMaterialData(bool isActive, AbilityType abilityType)
        {
            if (isActive)
            {
                SetMaterialColor(abilityType);
                SetTrailColor(abilityType);
                EnableMaterial();
            }
            else
            {
                SetTrailColor(AbilityType.None);
                DisableMaterial();
            }
        }

        #endregion

        #region Abilities
        
        private void SetActiveSuperShield(bool isActive)
        {
            superSprite.gameObject.SetActive(isActive);
        }

        private void SetActiveNormalShield(bool isActive)
        {
            normalSprite.gameObject.SetActive(isActive);
        }

        private void SetActiveAutomatic(bool isActive)
        {
            SetMaterialData(isActive, abilityType: AbilityType.Automatic);
        }
        
        private void SetActiveGold(bool isActive)
        {
            SetMaterialData(isActive, abilityType: AbilityType.DoublePoints);
        }

        private void SetActiveSlow(bool isActive)
        {
            SetMaterialData(isActive, abilityType: AbilityType.SlowMotion);
        }
        
        #endregion

        #region Handlers

        private void Shield_OnAbilitySetActiveHandler(AbilityType type, bool isActive)
        {
            switch (type)
            {
                case AbilityType.SuperShield:
                    SetActiveSuperShield(isActive);
                    break;
                case AbilityType.SlowMotion:
                    SetActiveSlow(isActive);
                    break;
                case AbilityType.DoublePoints:
                    SetActiveGold(isActive);
                    break;
                case AbilityType.Automatic:
                    SetActiveAutomatic(isActive);
                    break;
            }
        }
        
        private void Shield_OnEnableSuperShieldHandler(float targetTime)
        {
            var disableNormalShield = new SetMaterialOpacityAction(SetNormalShieldOpacity,
                0,1,targetTime);
            var enableSuperShield = new SetMaterialOpacityAction(SetSuperShieldOpacity,
                1,0,targetTime);
            
            var actions = ActionBuilder.Start()
                .Do(new SetBoolAction(true, SetActiveSuperShield))
                .Then(new ParallelAction(new []{disableNormalShield, enableSuperShield}))
                .Then(new SetBoolAction(false, SetActiveNormalShield))
                .Then(new WaitFramesAction(1))
                .Build();
            
            ActionManager.Add(actions,ActionManager.UpdateType.Update);
        }
        
        private void Shield_OnDisableSuperShieldHandler(float targetTime)
        {
            var disableSuperShield = new SetMaterialOpacityAction(SetSuperShieldOpacity,
                0,1,targetTime);
            var enableNormalShield = new SetMaterialOpacityAction(SetNormalShieldOpacity,
                1,0,targetTime);
            
            var actions = ActionBuilder.Start()
                .Do(new SetBoolAction(true, SetActiveNormalShield))
                .Then(new ParallelAction(new []{enableNormalShield, disableSuperShield}))
                .Then(new SetBoolAction(false, SetActiveSuperShield))
                .Then(new WaitFramesAction(1))
                .Build();
            
            ActionManager.Add(actions,ActionManager.UpdateType.Update);
        }
        
        private void Shield_OnDisableAbilityHandler()
        {
            SetAbilityOpacity(0);
        }
        
        #endregion
    }
    
    public interface IAbilityShield
    {
        public event Action<AbilityType, bool> OnAbilitySetActive;
        
        public event Action<float> OnEnableSuperShield;
        public event Action<float> OnDisableSuperShield;

        public event Action OnDisableAbility;
    }
}