using _Main.Scripts.Contracts.Events;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.NetworkVibrationSender
{
    public class VibrationCaller : MonoBehaviour
    {
        [Tooltip("In ms")]
        [Range(10,3000)]
        public long Duration;
        [Range(1,255)]
        public int Intensity;
        
        public void SendVibration() => VibrationEvents.TriggerOnVibrate(Duration,Intensity);
        public void Cancel() => VibrationEvents.TriggerOnCancel();
    }
#if UNITY_EDITOR
    
    [CustomEditor(typeof(VibrationCaller))]
    public class VibrationCallerTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            VibrationCaller script = (VibrationCaller)target;
            
            if (GUILayout.Button("Send")) script.SendVibration();
            if (GUILayout.Button("Cancel")) script.Cancel();
        }
    }
    
#endif
    
}