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
    
    public interface IComet : IFlyingObject<CometValues> { }

    public interface IDebugComet : IDebugFlyingObject
    {
        public float TravelRatio { get; set; }
        public float Distance { get; set; }
        public float Scale { get; set; }

    }
}