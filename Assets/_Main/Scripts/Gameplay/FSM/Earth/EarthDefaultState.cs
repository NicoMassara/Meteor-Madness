﻿namespace _Main.Scripts.Gameplay.FSM.Earth
{
    public class EarthDefaultState<T> : EarthBaseState<T>
    {
        public override void Awake()
        {
            Controller.SetRotation(true);
        }
    }
}