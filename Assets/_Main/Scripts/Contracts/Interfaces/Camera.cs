using UnityEngine;

namespace _Main.Scripts.Contracts.Interfaces
{
    public interface ICameraTransportData
    {
        public IZoomData  CameraZoomData { get; }
        public IMovementData  CameraMovementData { get; }
    }

    public interface IZoomData
    {
        public float Time { get; }
        public float Value { get; }
        public AnimationCurve Curve { get; }
        public bool DoesChange { get; }
    }
    
    public interface IMovementData
    {
        
        public float Time { get; }
        public Vector2 Position { get; }
        public AnimationCurve Curve { get; }
        public bool DoesChange { get; }
    }
}