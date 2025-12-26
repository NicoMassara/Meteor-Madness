using System.Collections.Generic;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Systems;
using NicolasMassara.CustomActionManager;
using UnityEngine;

namespace _Main.Scripts.Earth
{
    public class EarthController :
        EarthController.IEarthController,
        EarthController.IController
    {
        #region Interfaces

        public interface IEarthController
        {
            public void Initialize();
            public void Execute(float deltaTime);

            public void EnableDamage();
            public void DisableDamage();
            public void TryHeal(float healAmount);
            public void TryCollision(float damage, Vector3 position, Quaternion rotation, Vector2 direction);
            public void TryRestart();
            public void TryShake();
            public void TryPreSlice();
            public void FinishHealing();
        }

        #endregion

        #region Main Controller

        private interface IController
        {
            public IEarthDestruction GetEarthDestructionTimeValues();
            public void GoToDestruction();
            public void EnableDeathShake();
            public void DisableDeathShake();
            public void StartDestruction();
            public void EndDestruction();
            public void EnableRotation();
            public void DisableRotation();
            public void RestartHealth();
            public void TriggerDeath();
        }
        
        private class MainController
        {
            #region States

            private class BaseState<T> : State<T>
            {
                protected IController Controller { get; private set; }

                public void Initialize(IController controller)
                {
                    this.Controller = controller;
                }
            }
            private class IdleState<T> : BaseState<T> { }
            private class GameplayState<T> : BaseState<T> { }
            private class DeadState<T> : BaseState<T>
            {
                public override void Awake()
                {
                    Controller.DisableRotation();
                    Controller.TriggerDeath();
                }
            }
            private class DeadShakingState<T> : BaseState<T>
            {
                private IQueueAction _queue;

                public override void Awake()
                {
                    _queue = ActionBuilder.Start()
                        .Do(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().StartShake))
                        .Then(new InstantAction(() => { Controller.EnableDeathShake(); }))
                        .Then(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().DeathShakeDuration))
                        .Then(new InstantAction(() => { Controller.DisableDeathShake(); }))
                        .Then(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().ShowEarthDestruction))
                        .Then(new InstantAction(() => { Controller.GoToDestruction(); }))
                        .Build();
                }

                public override void Execute(float deltaTime) => _queue.OnUpdate(deltaTime);
            }
            private class DestructionState<T> : BaseState<T>
            {
                private IQueueAction _queue;

                public override void Awake()
                {
                    _queue = ActionBuilder.Start()
                        .Do(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().StartTriggerDestructionTime))
                        .Then(new InstantAction(() => { Controller.StartDestruction(); }))
                        .Then(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().StartRotatingAfterDeath))
                        .Then(new InstantAction(() => { Controller.EnableRotation(); }))
                        .Then(new WaitSecondsAction(Controller.GetEarthDestructionTimeValues().EndTriggerDestructionTime))
                        .Then(new InstantAction(() =>
                        {
                            Controller.EndDestruction();
                        }))
                        .Build();
                }

                public override void Execute(float deltaTime) => _queue.OnUpdate(deltaTime);
            }
            private class HealState<T> : BaseState<T>
            {
                public override void Awake() => Controller.RestartHealth();
            }
            
            #endregion
            
            private enum States
            {
                None,
                Idle,
                Gameplay,
                Dead,
                Shaking,
                Destruction,
                Heal
            }
            
            private FSM<States> _fsm;

            private class ActionGate : FsmActionGate<States>
            {
                public bool IsDamageDisable { get; private set; }
                public bool IsInIdle { get; private set; }
                
                public bool WasInIdle { get; private set; }
                public bool WasInGameplay { get; private set; }
                public bool WasInDead { get; private set; }
                public ActionGate(FSM<States> fsm) : base(fsm) { }

                protected override void OnEnterState(States state)
                {
                    IsDamageDisable = state is not States.Gameplay;
                    IsInIdle = state is States.Idle;
                }

                protected override void OnExitState(States state)
                {
                    WasInIdle = state is States.Idle;
                    WasInGameplay = state is States.Gameplay;
                }
            }
            
            private ActionGate _actionGate;

            public MainController(IController controller)
            {
                InitializeFsm(controller);
                TransitionToIdle(); 
            }

            #region FSM

            private void InitializeFsm(IController controller)
            {
                var temp = new List<BaseState<States>>();
                _fsm = new FSM<States>("Earth");
                _actionGate = new ActionGate(_fsm);

                #region Variables

                var none = new BaseState<States>();
                var idle = new IdleState<States>();
                var gameplay = new GameplayState<States>();
                var dead = new DeadState<States>();
                var shaking = new DeadShakingState<States>();
                var destruction = new DestructionState<States>();
                var heal = new HealState<States>();
                
                temp.Add(idle);
                temp.Add(gameplay);
                temp.Add(dead);
                temp.Add(shaking);
                temp.Add(destruction);
                temp.Add(heal);

                #endregion

                #region Transitions

                none.AddTransition(States.Idle, idle);
                
                idle.AddTransition(States.Gameplay, gameplay);
                idle.AddTransition(States.Heal, heal);
                
                gameplay.AddTransition(States.Dead, dead);
                gameplay.AddTransition(States.Heal, heal);
                gameplay.AddTransition(States.Idle, idle);
                
                dead.AddTransition(States.Shaking, shaking);
                
                shaking.AddTransition(States.Destruction, destruction);
                
                destruction.AddTransition(States.Idle, gameplay);
                destruction.AddTransition(States.Heal, heal);
                
                heal.AddTransition(States.Gameplay, gameplay);
                heal.AddTransition(States.Idle, idle);

                #endregion

                foreach (var state in temp)
                {
                    state.Initialize(controller);
                }
                
                _fsm.SetInit(none);
            }

            #region Transitions

            private void SetTransition(States state)
            {
                _fsm?.Transitions(state);
            }
            
            public void TransitionToIdle()
            {
                SetTransition(States.Idle);
            }

            public void TransitionToDead()
            {
                SetTransition(States.Dead);
            }
            
            public void TransitionToShaking()
            {
                SetTransition(States.Shaking);
            }
            
            public void TransitionToDestruction()
            {
                SetTransition(States.Destruction);
            }

            public void TransitionToHeal()
            {
                SetTransition(States.Heal);
            }
            
            public void TransitionToGameplay()
            {
                SetTransition(States.Gameplay);
            }

            #endregion

            #endregion

            #region Public Getters
            public bool GetIsDamageDisable() => _actionGate.IsDamageDisable;
            public bool GetIsInIdle() => _actionGate.IsInIdle;
            public bool GetWasInIdle() => _actionGate.WasInIdle;
            public bool GetWasInGameplay() => _actionGate.WasInGameplay;
            
            #endregion

            public void Execute(float deltaTime)
            {
                _fsm?.Execute(deltaTime);
            }
        }

        #endregion
        
        private readonly IEarthDestruction _earthDestructionTimeValues;
        private readonly EarthMotor _motor;
        private MainController _mainController;

        public EarthController(EarthMotor motor, IEarthDestruction earthDestructionTimeValues)
        {
            _motor = motor;
            _earthDestructionTimeValues = earthDestructionTimeValues;
        }
        
        #region IEarthController
        
        public void Initialize()
        {
            _motor.OnDeath += Motor_OnDeathHandler;
            _mainController = new MainController(this);
        }

        public void Execute(float deltaTime)
        {
            _mainController.Execute(deltaTime);
        }
        
        public IEarthDestruction GetEarthDestructionTimeValues()
        {
            return _earthDestructionTimeValues;
        }

        public void GoToDestruction()
        {
            _mainController.TransitionToDestruction();
        }

        public void TryCollision(float damage, Vector3 position, Quaternion rotation, Vector2 direction)
        {
            if (_mainController.GetIsDamageDisable())
            {
                return;
            }

            _motor.HandleCollision(damage, position, rotation, direction);
        }
        
        public void TryHeal(float healAmount)
        {
            _motor.Heal(healAmount);
        }

        public void EnableDamage()
        {
            _mainController.TransitionToGameplay();
        }

        public void DisableDamage()
        {
            _mainController.TransitionToIdle();
        }

        public void TryRestart()
        {
            _mainController.TransitionToHeal();
        }

        public void TryShake()
        {
            _mainController.TransitionToShaking();
        }

        public void TryPreSlice()
        {
            if (_mainController.GetIsInIdle())
            {
                _motor.PreSlice();
            }
        }

        public void FinishHealing()
        {
            _mainController.TransitionToIdle();
        }

        #endregion
        
        #region IController
        
        public void RestartHealth()
        {
            _motor.RestartHealth();
        }
        
        public void TriggerDeath()
        {
            _motor.TriggerDeath();
        }
        
        public void StartDestruction()
        {
            _motor.TriggerDestruction();
        }

        public void EndDestruction()
        {
            _motor.TriggerEndDestruction();
        }

        public void EnableRotation()
        {
            _motor.SetRotation(true);
        }
        
        public void DisableRotation()
        {
            _motor.SetRotation(false);
        }
        
        public void EnableDeathShake()
        {
            _motor.SetDeathShake(true);
        }

        public void DisableDeathShake()
        {
            _motor.SetDeathShake(false);
        }

        #endregion
        
        #region Handlers

        private void Motor_OnDeathHandler()
        {
            _mainController.TransitionToDead();
        }

        #endregion
    }
}