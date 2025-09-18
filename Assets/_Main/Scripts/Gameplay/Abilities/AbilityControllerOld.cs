using System;
using System.Collections;
using System.Collections.Generic;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityControllerOld : ManagedBehavior, IUpdatable
    {
        private Dictionary<AbilityType, AbilityData> _abilities = new Dictionary<AbilityType, AbilityData>();
        
        private EventBusManager _eventBus;
        private AbilityData _currentAbility;
        private Timer _abilityTimer;
        private bool _canUseAbility;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;
        
        private void Start()
        {
            SetEventBus();
            CreateAbilityData();
        }
        
        private void TriggerAbility(AbilityType enumType)
        {
            if(_canUseAbility == false) return;
            
            _currentAbility = enumType switch
            {
                AbilityType.TotalShield => _abilities[AbilityType.TotalShield],
                AbilityType.SlowMotion => _abilities[AbilityType.SlowMotion],
                AbilityType.Health => _abilities[AbilityType.Health],
                _ => throw new ArgumentOutOfRangeException(nameof(enumType), enumType, null)
            };

            StartCoroutine(RunAbilityQueue(_currentAbility.GetStartActionQueue()));
        }

        private IEnumerator RunAbilityTimer()
        {
            while (!_abilityTimer.GetHasEnded)
            {
                _abilityTimer.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);

                yield return null;
            }
        }


        private IEnumerator RunAbilityQueue(ActionQueue actionQueue)
        {
            while (!actionQueue.IsEmpty)
            {
                actionQueue.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);

                yield return null;
            }
        }

        public void ManagedUpdate()
        {
            
        }

        private void CreateAbilityData()
        {
            #region TotalShield
            
            //Start Queue
            ActionQueue shieldStartQueue = new ActionQueue(new []
            {
                new ActionData(() =>
                {
                    _eventBus.Publish(new SetEnableInputs{IsEnabled = false});
                }, 0f),
                new ActionData(() =>
                {
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 0;
                }, TotalShieldStartTimeValues.TimeBeforeDecreasingTimeScale),
                new ActionData(() =>
                {
                    _eventBus.Publish(new CameraZoomIn());
                }, TotalShieldStartTimeValues.TimeToZoomIn),
                new ActionData(() =>
                {
                    _eventBus.Publish(new SetTotalShield{});
                }, TotalShieldStartTimeValues.TimeBeforeTotalShield),
                new ActionData(() =>
                {
                    _eventBus.Publish(new SpawnRingMeteor());
                }, TotalShieldStartTimeValues.TimeToSpawnRingMeteor),
                new ActionData(() =>
                {
                    _eventBus.Publish(new CameraZoomOut());
                }, TotalShieldStartTimeValues.TimeToZoomOut),
                new ActionData(() =>
                {
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 1;
                }, TotalShieldStartTimeValues.TimeBeforeIncreasingTimeScale),
                new ActionData(() =>
                {
                    _abilityTimer.Set(_abilities[AbilityType.TotalShield].ActiveTime);
                    _abilityTimer.OnEnd += AbilityTimer_OnEndHandler;
                    StartCoroutine(RunAbilityTimer());
                }, TotalShieldStartTimeValues.TimeBeforeIncreasingTimeScale),
            });
            
            //End Queue
            ActionQueue shieldEndQueue = new ActionQueue(new []
            {
                new ActionData(() =>
                {
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 0;
                }, 1f),
                new ActionData(() =>
                {
                    _eventBus.Publish(new SetNormalShield());
                }, 0f),
                new ActionData(() =>
                {
                    _eventBus.Publish(new SetEnableInputs{IsEnabled = true});
                }, 1f),
                new ActionData(() =>
                {
                    CustomTime.GetChannel(UpdateGroup.Gameplay).TimeScale = 1;
                }, 1f),
            });

            var shieldData = new AbilityData
            {
                ActiveTime = 5f,
                AbilityType = AbilityType.TotalShield,
                StartActionQueue = shieldStartQueue,
                EndActionQueue = shieldEndQueue,
            };
            
            _abilities.Add(shieldData.AbilityType, shieldData);

            #endregion
            
            #region SlowMotion

            //TotalShield
            ActionQueue slowQueue = new ActionQueue();
            ActionData[] slowActionData = new []
            {
                new ActionData(() =>
                {
                    //Disable Player Inputs
                    //Stops GameplayTime
                }, 0f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
            };

            #endregion
        }

        #region Handlers

        private void AbilityTimer_OnEndHandler()
        {
            _abilityTimer.OnEnd -= AbilityTimer_OnEndHandler;
            
            if(_currentAbility == null) return;

            StartCoroutine(RunAbilityQueue(_currentAbility.GetEndActionQueue()));
            _currentAbility = null;
        }

        #endregion
        
        
        #region Event Bus

        private void SetEventBus()
        {
            _eventBus = GameManager.Instance.EventManager;
            
            _eventBus.Subscribe<SetEnableAbility>(EventBus_OnSetEnableAbility);
        }

        private void EventBus_OnSetEnableAbility(SetEnableAbility input)
        {
            _canUseAbility = input.IsEnable;
        }
        
        #endregion
    }

    public enum AbilityType
    {
        TotalShield,
        SlowMotion,
        Health
    }

    public class AbilityData
    {
        public float ActiveTime;
        public AbilityType AbilityType;
        public ActionQueue StartActionQueue;
        public ActionQueue EndActionQueue;
        

        public ActionQueue GetStartActionQueue()
        {
            return StartActionQueue;
        }

        public ActionQueue GetEndActionQueue()
        {
            return EndActionQueue;
        }
    }
} 