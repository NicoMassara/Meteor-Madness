using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Earth.States;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController 
    {
        private readonly IEarthDestruction _earthDestructionTimeValues;
        private readonly EarthMotor _motor;
        private FSM<States> _fsm;

        private enum States
        {
            Default,
            Dead,
            Shaking,
            Destruction,
            Heal
        }

        public EarthController(EarthMotor motor, IEarthDestruction earthDestructionTimeValues)
        {
            _motor = motor;
            _earthDestructionTimeValues = earthDestructionTimeValues;
        }

        public void Initialize()
        {
            InitializeFsm();
            _motor.OnDeath += Motor_OnDeathHandler;
        }

        public void Execute(float deltaTime)
        {
            _fsm.Execute(deltaTime);
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<EarthBaseState<States>>();
            _fsm = new FSM<States>();

            #region Variables

            var defaultEarth = new EarthDefaultState<States>();
            var dead = new EarthDeadState<States>();
            var shaking = new EarthDeadShakingState<States>();
            var destruction = new EarthDestructionState<States>();
            var heal = new EarthHealState<States>();
            
            temp.Add(defaultEarth);
            temp.Add(dead);
            temp.Add(shaking);
            temp.Add(destruction);
            temp.Add(heal);

            #endregion

            #region Transitions

            defaultEarth.AddTransition(States.Dead, dead);
            defaultEarth.AddTransition(States.Heal, heal);
            
            dead.AddTransition(States.Shaking, shaking);
            
            shaking.AddTransition(States.Destruction, destruction);
            
            destruction.AddTransition(States.Default, defaultEarth);
            destruction.AddTransition(States.Heal, heal);
            
            heal.AddTransition(States.Default, defaultEarth);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(defaultEarth);
            _fsm.FSMName = "Earth";
        }

        #region Transitions

        private void SetTransition(States state)
        {
            _fsm?.Transitions(state);
        }
        
        public void TransitionToDefault()
        {
            SetTransition(States.Default);
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

        #endregion

        #endregion

        public IEarthDestruction GetEarthDestructionTimeValues()
        {
            return _earthDestructionTimeValues;
        }

        #region Motor

        #region Health

        public void RestartHealth()
        {
            _motor.RestartHealth();
        }

        public void HandleCollision(float damage, Vector3 position, Quaternion rotation, Vector2 direction)
        {
            _motor.HandleCollision(damage, position, rotation, direction);
        }

        public void Heal(float healAmount)
        {
            _motor.Heal(healAmount);
        }

        #endregion

        #region Death

        public void TriggerDeath()
        {
            _motor.TriggerDeath();
        }

        public void TriggerDestruction()
        {
            _motor.TriggerDestruction();
        }

        public void TriggerEndDestruction()
        {
            _motor.TriggerEndDestruction();
        }

        public void SetDeathShake(bool isShaking)
        {
            _motor.SetDeathShake(isShaking);
        }

        #endregion

        public void SetRotation(bool canRotate)
        {
            _motor.SetRotation(canRotate);
        }

        public void SetEnableDamage(bool canTakeDamage)
        {
            _motor.SetEnableDamage(canTakeDamage);
        }

        #endregion

        #region Handlers

        private void Motor_OnDeathHandler()
        {
            TransitionToDead();
        }

        #endregion
    }

    #region States

    public class EarthDeadShakingState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();

        public override void Awake()
        {
            var temp = new ActionData[]
            {
                new(()=>Controller.SetDeathShake(true),
                    Controller.GetEarthDestructionTimeValues().StartShake),
                new(()=>Controller.SetDeathShake(false),
                    Controller.GetEarthDestructionTimeValues().DeathShakeDuration),
                new(()=>Controller.TransitionToDestruction(),
                    Controller.GetEarthDestructionTimeValues().ShowEarthDestruction)
            };
            
            _queue.AddAction(temp);
        }

        public override void Execute(float deltaTime)
        {
            _queue.Run(deltaTime);
        }
    }
    
    public class EarthDeadState<T> : EarthBaseState<T>
    {
        
        public override void Awake()
        {
            Controller.SetRotation(false);
            Controller.TriggerDeath();
        }
    }
    
    public class EarthDefaultState<T> : EarthBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetRotation(true);
        }
    }
    
    public class EarthDestructionState<T> : EarthBaseState<T>
    {
        private readonly ActionQueue _queue = new ActionQueue();

        public override void Awake()
        {
            if (Controller == null)
            {
                Debug.Log("Controller is null");
                return;
            }

            var temp = new ActionData[]
            {
                new (()=>Controller.TriggerDestruction(),
                    Controller.GetEarthDestructionTimeValues().StartTriggerDestructionTime),
                new (()=>Controller.SetRotation(true),
                    Controller.GetEarthDestructionTimeValues().StartRotatingAfterDeath),
                new (()=>Controller.TriggerEndDestruction(),
                    Controller.GetEarthDestructionTimeValues().EndTriggerDestructionTime),
            };

            _queue.AddAction(temp);
        }

        public override void Execute(float deltaTime)
        {
            _queue.Run(deltaTime);
        }
    }
    
    public class EarthHealState<T> : EarthBaseState<T>
    {
        public override void Awake()
        {
            Controller.RestartHealth();
        }
    }

    #endregion
}