using System;
using _Main.Scripts.Interfaces.Sounds;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.Sounds
{
    public interface IDebugSounds : ISoundComponent
    {
        public event Action OnSoundPlayed;
        public event Action OnSoundPaused;
        public event Action OnSoundStopped;
        public event Action OnSoundResumed;
    }

    public class DebugSoundsController : MonoBehaviour, IDebugSounds
    {
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        
        public event Action OnSoundPlayed;
        public event Action OnSoundPaused;
        public event Action OnSoundStopped;
        public event Action OnSoundResumed;
        
        private void Start()
        {
            BootEvents.InitializeMainSystem();
        }
        
        public void PlaySound()
        {
            IsPlaying = true;
            IsPaused = false;
            OnSoundPlayed?.Invoke();
        }

        public void PauseSound()
        {
            IsPlaying = false;
            IsPaused = true;
            OnSoundPaused?.Invoke();
        }
        
        public void ResumeSound()
        {
            IsPlaying = true;
            IsPaused = false;
            OnSoundResumed?.Invoke();
        }

        public void StopSound()
        {
            IsPlaying = false;
            IsPaused = false;
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

            if (script.IsPlaying == false)
            {
                if (GUILayout.Button("Play Sound"))
                {
                    // Llama al método normalmente
                    script.PlaySound();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Sound"))
                {
                    // Llama al método normalmente
                    script.StopSound();
                }
            }
            
            if(script.IsPlaying && script.IsPaused == false)
            {
                if (GUILayout.Button("Pause Sound"))
                {
                    // Llama al método normalmente
                    script.PauseSound();
                }
            }
            
            if (script.IsPaused)
            {
                if (GUILayout.Button("Resume Sound"))
                {
                    // Llama al método normalmente
                    script.ResumeSound();
                }
            }

            
        }
    }
#endif
}