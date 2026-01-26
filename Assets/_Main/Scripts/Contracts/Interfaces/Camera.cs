using UnityEngine;

namespace _Main.Scripts.Contracts.Interfaces
{
    public interface ICameraTransportData
    {
        public IZoomData  CameraZoomData { get; }
        public IMovementData  CameraMovementData { get; }
        public float Time { get; }
    }

    public interface IZoomData
    {
        public float Value { get; }
        public AnimationCurve Curve { get; }
        public bool DoesChange { get; }
    }
    
    public interface IMovementData
    {
        public Vector3 Position { get; }
        public AnimationCurve Curve { get; }
        public bool DoesChange { get; }
    }
}