using _Main.Scripts.GameCamera;
using UnityEditor;
using UnityEngine;

namespace EditorUtilities
{
    
/*#if UNITY_EDITOR
    
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
    
#endif*/
}