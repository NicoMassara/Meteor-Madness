using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Earth;
using _Main.Scripts.Gameplay.FSM.Earth;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController 
    {
        private readonly EarthMotor _motor;
        private FSM<States> _fsm;

        private enum States
        {
            Default,
            Dead,
            Shaking,
            Destruction
        }

        public EarthController(EarthMotor motor)
        {
            _motor = motor;
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
            
            temp.Add(defaultEarth);
            temp.Add(dead);
            temp.Add(shaking);
            temp.Add(destruction);

            #endregion

            #region Transitions

            defaultEarth.AddTransition(States.Dead, dead);
            
            dead.AddTransition(States.Shaking, shaking);
            
            shaking.AddTransition(States.Destruction, destruction);
            
            destruction.AddTransition(States.Default, defaultEarth);

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

        #endregion

        #endregion
        
        #region Health

        public void RestartHealth()
        {
            _motor.RestartHealth();
        }

        public void HandleCollision(float damage, Vector3 position, Quaternion rotation)
        {
            _motor.HandleCollision(damage, position, rotation);
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

        #region Handlers

        private void Motor_OnDeathHandler()
        {
            TransitionToDead();
        }

        #endregion
    }
}