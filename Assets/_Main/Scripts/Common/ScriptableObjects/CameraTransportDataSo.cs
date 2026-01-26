using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.GameCamera
{
    [CreateAssetMenu(fileName = "So_CameraTransportData_Default", menuName = "Scriptable Objects/Camera Transport Data", order = 0)]
    public class CameraTransportDataSo : ScriptableObject, ICameraTransportData
    {
        #region Movement

        [System.Serializable]
        private class MovementData : IMovementData
        {
            [SerializeField] private float time;
            [SerializeField] private Vector2 position;
            [SerializeField] private AnimationCurve curve;
            [Space]
            [SerializeField] private bool doesChange;
            
            public float Time => time;
            public Vector2 Position => position;
            public AnimationCurve Curve => curve;
            public bool DoesChange => doesChange;
        }
        
        [SerializeField] private MovementData movementValues;

        #endregion

        #region Zoom

        [System.Serializable]
        private class ZoomData : IZoomData
        {
            [SerializeField] private float time;
            [SerializeField] private float value;
            [SerializeField] private AnimationCurve curve;
            [Space]
            [SerializeField] private bool doesChange;
            
            public float Time => time;
            public float Value => value;
            public AnimationCurve Curve => curve;
            public bool DoesChange => doesChange;
        }

        [SerializeField] private ZoomData zoomValues;

        #endregion
        

        public IMovementData CameraMovementData => movementValues;
        public IZoomData CameraZoomData => zoomValues;
    }
}