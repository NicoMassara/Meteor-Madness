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
}