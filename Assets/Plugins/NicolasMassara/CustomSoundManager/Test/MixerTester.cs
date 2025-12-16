using System;
using UnityEditor;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager.Test
{
    public class MixerTester : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private MixerVolumes mixerVolume;
        
        [Serializable]
        private class MixerVolumes
        {
            [Range(0, 1)] public float masterVolume = 1;
            [Range(0, 1)] public float sfxVolume = 1;
            [Range(0, 1)] public float musicVolume = 1;
            [Range(0, 1)] public float uiVolume = 1;
        }
        
        private bool _hasStarted;
        public bool GetHasStarted() => _hasStarted;
        
        private void Start()
        {
            _hasStarted = true;
            SoundManager.LoadInstance();
        }
        
        public void StopAllSounds()
        {
            SoundManager.ClearAllSounds();
        }
        
        private void OnValidate()
        {
            if(_hasStarted == false) return;
            
            AudioMixerTools.SetMixerChannelVolume(MixerChannels.Master,mixerVolume.masterVolume);
            AudioMixerTools.SetMixerChannelVolume(MixerChannels.Sfx,mixerVolume.sfxVolume);
            AudioMixerTools.SetMixerChannelVolume(MixerChannels.Music,mixerVolume.musicVolume);
            AudioMixerTools.SetMixerChannelVolume(MixerChannels.UI,mixerVolume.uiVolume);  
        }
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MixerTester))]
    public class MixerTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();
            
            // Agrega el botón
            MixerTester script = (MixerTester)target;
            
            if(script.GetHasStarted() == false) 
                return;
            
            GUILayout.Label("Sounds");

            if (GUILayout.Button("Stop All")) script.StopAllSounds();
            
        }
    }
#endif
}