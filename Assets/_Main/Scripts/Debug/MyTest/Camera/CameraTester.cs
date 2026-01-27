using System;
using _Main.Scripts.Contracts.Interfaces;
using _Main.Scripts.EventBus;
using _Main.Scripts.GameCamera;
using MeteorMadness.Common.Shaker;
using MeteorMadness.Contracts;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Camera
{
    public class CameraTester : MonoBehaviour
    {
        [SerializeField] private ShakeDataSo shakeData;

        #region Private Class
        
        [System.Serializable]
        public class TransportData : ICameraTransportData
        {
            #region Movement

            [System.Serializable]
            private class MovementData : IMovementData
            {
                [SerializeField] private Vector2 position;
                [SerializeField] private AnimationCurve curve;
                public Vector3 Position => new Vector3(position.x, position.y, -10);
                public AnimationCurve Curve => curve;
                public bool DoesChange => true;
            
            }
        
            [SerializeField] private MovementData movementValues;

            #endregion

            #region Zoom

            [System.Serializable]
            private class ZoomData : IZoomData
            {
                [SerializeField] private float value;
                [SerializeField] private AnimationCurve curve;
                
                public float Value => value;
                public AnimationCurve Curve => curve;
                public bool DoesChange => true;
            }

            [SerializeField] private ZoomData zoomValues;

            #endregion
            
            [Space]
            [Min(0)]
            [SerializeField] private float time;
            
            public float Time => time;
            
            public IMovementData CameraMovementData => movementValues;
            public IZoomData CameraZoomData => zoomValues;
        }
        
        #endregion
        
        [SerializeField] private CameraTransportDataSo targetTransportData;
        [SerializeField] private TransportData zoomOutData;

        public bool IsGrayscaleEnable { get; private set; }
        public bool IsZoomingIn { get; private set; }


        private void Start()
        {
            CameraEventSubscriber.NotifyTransportStarted(EventBus_Camera_Transport_Started);
            CameraEventSubscriber.NotifyTransportFinished(EventBus_Camera_Transport_Finished);
        }

        #region Event Bus
        

        private void EventBus_Camera_Transport_Started(CameraEvents.TransportStarted input)
        {
            UnityEngine.Debug.Log($"{input.Type} Started");
        }
        private void EventBus_Camera_Transport_Finished(CameraEvents.TransportFinished input)
        {
            UnityEngine.Debug.Log($"{input.Type} Finished");
        }
        
        #endregion

        public void Shake()
        {
            CameraEventCaller.Shake(shakeData);
        }

        public void MoveToTarget()
        {
            CameraEventCaller.Transport(targetTransportData);
        }

        public void RemoveTarget()
        {
            CameraEventCaller.Transport(zoomOutData);
        }

        public void ToggleGrayscale()
        {
            IsGrayscaleEnable =  !IsGrayscaleEnable;
            
            if (IsGrayscaleEnable)
                CameraEventCaller.EnableGrayscale();
            else
                CameraEventCaller.DisableGrayscale();
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(CameraTester))]
    public class CameraTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CameraTester script = (CameraTester)target;
            if (GUILayout.Button("Shake")) script.Shake();
            if (GUILayout.Button("Move to Target")) script.MoveToTarget();
            if (GUILayout.Button("Remove Target")) script.RemoveTarget();
            if (GUILayout.Button(script.IsGrayscaleEnable ? "Disable Grayscale" : "Enable Grayscale")) 
                script.ToggleGrayscale();
        }
    }
    
#endif
}