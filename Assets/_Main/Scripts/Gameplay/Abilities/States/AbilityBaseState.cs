﻿using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Abilies;

namespace _Main.Scripts.Gameplay.Ability.States
{
    public class AbilityBaseState<T> : State<T>
    {
        protected AbilityController Controller { get; private set; }

        public void Initialize(AbilityController controller)
        {
            Controller = controller;
        }
    }
}