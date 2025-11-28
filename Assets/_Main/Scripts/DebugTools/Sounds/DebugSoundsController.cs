using System;
using _Main.Scripts.Interfaces.Sounds;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.DebugTools.Sounds
{
    public interface IDebugSounds : ISoundComponent
    {
        public event Action OnSoundPlayed;
        public event Action OnSoundStopped;
    }

    public class DebugSoundsController : MonoBehaviour, IDebugSounds
    {
        public event Action OnSoundPlayed;
        public event Action OnSoundStopped;
        
        public void PlaySound()
        {
            OnSoundPlayed?.Invoke();
        }

        public void StopSound()
        {
            OnSoundStopped?.Invoke();
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(DebugSoundsController))]
    public class DebugSoundsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            DebugSoundsController script = (DebugSoundsController)target;
            if (GUILayout.Button("Play Sound"))
            {
                // Llama al método normalmente
                script.PlaySound();
            }
            
            if (GUILayout.Button("Stop Sound"))
            {
                // Llama al método normalmente
                script.StopSound();
            }
        }
    }
#endif
}