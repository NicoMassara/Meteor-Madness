﻿using System;
using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.FSM.Shield;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldController
    {
        private ShieldMotor _motor;
        private ShieldView _view;
        
        private class ActionGate
        {
            public bool RotationEnable { get; private set; }
            public ActionGate(FSM<States> fsm)
            {
                fsm.OnEnterState += state =>
                {
                    RotationEnable = state == States.Active;
                };
            }
        }

        private enum States
        {
            Unactive,
            Active,
            Total
        }

        private FSM<States> _fsm;

        private ActionGate _actionGate;

        public ShieldController(ShieldMotor motor)
        {
            _motor = motor;
        }

        public void Initialize()
        {
            InitializeFsm();
        }

        public void Execute(float deltaTime)
        {
            _fsm.Execute(deltaTime);

            
            //Tester
            if (Input.GetKeyDown(KeyCode.R))
            {
                TransitionToTotal();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                TransitionToActive();
            }
        }

        #region FSM

        private void InitializeFsm()
        {
            var temp = new List<ShieldBaseState<States>>();
            _fsm = new FSM<States>();
            _actionGate = new ActionGate(_fsm);

            #region Variables

            var unactive = new ShieldUnactiveState<States>();
            var active = new ShieldActivateState<States>();
            var total = new ShieldSuperState<States>();
            
            temp.Add(unactive);
            temp.Add(active);
            temp.Add(total);

            #endregion

            #region Transitions

            unactive.AddTransition(States.Active, active);
            
            active.AddTransition(States.Total, total);
            active.AddTransition(States.Unactive, unactive);
            
            total.AddTransition(States.Active, active);

            #endregion

            foreach (var state in temp)
            {
                state.Initialize(this);
            }
            
            _fsm.SetInit(unactive);
            _fsm.FSMName = "Shield";
        }

        private void SetTransitions(States state)
        {
            _fsm.Transitions(state);
        }

        public void TransitionToActive()
        {
            SetTransitions(States.Active);
        }

        public void TransitionToUnactive()
        {
            SetTransitions(States.Unactive);
        }

        public void TransitionToTotal()
        {
            SetTransitions(States.Total);
        }

        #endregion
        
        #region Movement

        public void Rotate(float direction)
        {
            if (_actionGate.RotationEnable)
            {
                _motor.Rotate(direction);
            }
        }

        public void ForceRotation()
        {
            _motor.Rotate(0.5f);
        }

        public void StopRotate()
        {
            _motor.StopRotate();
        }

        public void RestartPosition()
        {
            _motor.RestartPosition();
        }

        #endregion

        #region Sprites

        public void SetSpriteByEnum(SpriteType spriteType)
        {
            _motor.SetSpriteByEnum(spriteType);
        }

        public void SetActiveShield(bool isActive)
        {
            _motor.SetActiveShield(isActive);
        }

        #endregion
        
        public void HandleHit(Vector3 inputPosition)
        {
            _motor.HandleHit(inputPosition);
        }

        #region Handlers
        

        #endregion
    }
}