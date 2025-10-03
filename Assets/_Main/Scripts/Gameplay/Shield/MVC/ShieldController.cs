﻿using System.Collections.Generic;
using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Shield.States;
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
                    RotationEnable = state is States.Active or States.Gold;
                };
            }
        }

        private enum States
        {
            Unactive,
            Active,
            Super,
            Gold,
            Automatic
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
                TransitionToSuper();
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
            var super = new ShieldSuperState<States>();
            var gold = new ShieldGoldState<States>();
            var automatic = new ShieldAutomaticState<States>();
            
            temp.Add(unactive);
            temp.Add(active);
            temp.Add(super);
            temp.Add(gold);
            temp.Add(automatic);

            #endregion

            #region Transitions

            unactive.AddTransition(States.Active, active);
            
            active.AddTransition(States.Unactive, unactive);
            active.AddTransition(States.Super, super);
            active.AddTransition(States.Gold, gold);
            active.AddTransition(States.Automatic, automatic);
            
            super.AddTransition(States.Active, active);
            
            gold.AddTransition(States.Active, active);
            gold.AddTransition(States.Unactive, unactive);
            
            automatic.AddTransition(States.Active, active);

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

        public void TransitionToSuper()
        {
            SetTransitions(States.Super);
        }
        
        public void TransitionToGold()
        {
            SetTransitions(States.Gold);
        }
        
        public void TransitionToAutomatic()
        {
            SetTransitions(States.Automatic);
        }

        #endregion
        
        #region Movement

        public void TryRotate(float direction)
        {
            if (_actionGate.RotationEnable)
            {
                _motor.Rotate(direction);
            }
        }

        public void SetActiveSuperShield(bool isActive)
        {
            _motor.SetActiveSuperShield(isActive);
        }

        public void TryStopRotate()
        {
            if (_actionGate.RotationEnable)
            {
                _motor.StopRotate();
            }
        }

        public void ForceRotate(float direction)
        {
            _motor.ForceRotate(direction);
        }
        
        public void RestartPosition()
        {
            _motor.RestartPosition();
        }
        
        #endregion

        #region Sprites

        public void SetActiveShield(bool isActive)
        {
            _motor.SetActiveShield(isActive);
        }
        
        public void SetActiveGold(bool isActive)
        {
            _motor.SetActiveGold(isActive);
        }

        #endregion
        
        public void HandleHit(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            _motor.HandleHit(position,rotation,direction);
        }

        public void SetActiveAutomatic(bool isActive)
        {
            _motor.SetActiveAutomatic(isActive);
        }


        #region Handlers
        

        #endregion
    }
}