using System;
using JetBrains.Annotations;
using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet
{
    public class CometValues : FlyingObjectValues
    {
        public Vector2 Scale;
    }
    
    public interface IComet
    {
        public Vector2 Position { get; }

        public void SetValues(CometValues data);
        public void Recycle();
    }

    public interface IDebugComet : IDebugFlyingObject
    {
        public float TravelRatio { get; set; }
        public float Distance { get; set; }
        public float Scale { get; set; }
        public bool DebugEnable { get; set; }
    }
}