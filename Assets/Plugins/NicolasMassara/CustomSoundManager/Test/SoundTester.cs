using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager.Test
{
    public class SoundTester : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private SoundSourceDataSo soundDataSo;
        [Range(0, 1f)] 
        [SerializeField] public float volume = 1;
        
        private List<SoundManager.GeneratedId> _soundSourcesId;
        private bool _hasStarted;
        public bool GetIsMuted() => volume <= 0;
        
        public bool GetHasStarted() => _hasStarted;
        
        private void Awake()
        {
            _soundSourcesId = new List<SoundManager.GeneratedId>();
        }

        private void OnValidate()
        {
            if(_hasStarted == false) return;
            UpdateVolume();
        }

        public void Mute()
        {
            volume = 0;
            UpdateVolume();
        }

        public void UnMute()
        {
            volume = 1;
            UpdateVolume();
        }

        private void UpdateVolume()
        {
            foreach (var item in _soundSourcesId)
            {
                SoundManager.SetSoundVolume(item, volume);
            }
        }

        private void Start()
        {
            _hasStarted = true;
        }

        public void PlaySound()
        {
            var id = SoundManager.PlaySound(soundDataSo, transform);
            _soundSourcesId.Add(id);
        }

        public void StopSound()
        {
            if(_soundSourcesId.Count == 0) return;
            
            var id = _soundSourcesId[0];
            SoundManager.StopSound(id);
            _soundSourcesId.RemoveAt(0);
        }

        public bool GetIsPlayingSound()
        {
            if(_soundSourcesId.Count == 0) return false;
            
            return _soundSourcesId.Any(item => item.IsActive);
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(SoundTester))]
    public class SoundTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();
            
            // Agrega el botón
            SoundTester script = (SoundTester)target;
            
            if(script.GetHasStarted() == false) 
                return;
            
            GUILayout.Label("Sounds");
            
            if (script.GetIsPlayingSound())
            {
                if (GUILayout.Button("Play More Sounds")) script.PlaySound();
                if (GUILayout.Button("Stop Sound")) script.StopSound();
                
                if (script.GetIsMuted())
                {
                    if (GUILayout.Button("Unmute")) script.UnMute();
                }
                else
                {
                    if (GUILayout.Button("Mute")) script.Mute();
                }
                
            }
            else
            {
                if (GUILayout.Button("Play Sound")) script.PlaySound();
            }
            
        }
    }
#endif
}