﻿using _Main.Scripts.FiniteStateMachine;
using _Main.Scripts.Gameplay.Earth;

namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthBaseState<T> : State<T>
    {
        protected EarthController Controller { get; private set; }

        public void Initialize(EarthController controller)
        {
            this.Controller = controller;
        }
    }
}