using _Main.Scripts.Contracts.Interfaces;
using UnityEditor;
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
            [SerializeField] private Vector2 position;
            [SerializeField] private AnimationCurve curve;
            [Space] 
            [SerializeField] private bool doesChange = true;
            
            public Vector3 Position => new Vector3(position.x, position.y, -10);
            public AnimationCurve Curve => curve;
            public bool DoesChange => doesChange;
            
            public void SetPosition(Vector2 newPosition)
            {
                position = newPosition;
            }
            
        }
        
        [SerializeField] private MovementData movementValues;

        #endregion

        #region Zoom

        [System.Serializable]
        private class ZoomData : IZoomData
        {
            [SerializeField] private float value;
            [SerializeField] private AnimationCurve curve;
            [Space]
            [SerializeField] private bool doesChange = true;

            public float Value => value;
            public AnimationCurve Curve => curve;
            public bool DoesChange => doesChange;

            public void SetZoom(float zoomValue)
            {
                value = zoomValue;
            }
        }

        [SerializeField] private ZoomData zoomValues;

        #endregion
        
        [Space]
        [Min(0)]
        [SerializeField] private float time;
            
        public float Time => time;

        public IMovementData CameraMovementData => movementValues;
        public IZoomData CameraZoomData => zoomValues;

        public void SetPositionFromCamera()
        {
            if (Camera.main != null)
            {
                var cameraPos = Camera.main.transform.position;
            
                movementValues.SetPosition(cameraPos);
            }
        }
        
        public void SetZoomFromCamera()
        {
            if (Camera.main != null)
            {
                var cameraZoom = Camera.main.orthographicSize;
            
                zoomValues.SetZoom(cameraZoom);
            }
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(CameraTransportDataSo))]
    public class CameraTransportDataSoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // dibuja los campos normales

            CameraTransportDataSo so = (CameraTransportDataSo)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Update Zoom from Camera"))
            {
                so.SetZoomFromCamera();

                // Marca el asset como modificado si cambias datos
                EditorUtility.SetDirty(so);
            }
            
            if (GUILayout.Button("Update Position from Camera"))
            {
                so.SetPositionFromCamera();

                // Marca el asset como modificado si cambias datos
                EditorUtility.SetDirty(so);
            }
        }
    }
    
#endif

}