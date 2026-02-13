using System;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield
{
    public struct ShieldRotationDebugData
    {
        public string State;
        public string LastState;
        public float InputAngle;
        public float InputMagnitude;
        public float TargetAngle;
    }
    
    public class ShieldRotationDebugEvents
    {
        public static event Action<ShieldRotationDebugData> OnDebug;
        public static void TriggerOnDebug(ShieldRotationDebugData data) =>  OnDebug?.Invoke(data);
    }
}