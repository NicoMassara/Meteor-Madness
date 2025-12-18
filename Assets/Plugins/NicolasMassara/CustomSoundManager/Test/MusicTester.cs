using System;
using UnityEditor;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager.Test
{
    public class MusicTester : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private MusicData primaryMusicData;
        [SerializeField] private MusicData secondaryMusicData;

        [Serializable]
        private class MusicData
        {
            public SoundSourceDataSo sourceData;
            public bool doesPauseCurrent;
            [Range(0.1f, 0.75f)] public float backgroundVolume = 0.5f;
        }

        private SoundManager.GeneratedId _primaryId;
        private SoundManager.GeneratedId _secondaryId;
        
        private bool _hasStarted;
        private bool _isPrimaryBackground;
        private bool _isSecondaryBackground;
        
        public bool GetHasStarted() => _hasStarted;

        public bool IsPrimaryBackground => _isPrimaryBackground;

        public bool IsSecondaryBackground => _isSecondaryBackground;

        private void Start()
        {
            _hasStarted = true;
        }

        public bool GetIsPrimaryPlaying()
        {
            return _primaryId != null && _primaryId.IsActive;
        }

        public bool GetIsSecondaryPlaying()
        {
            return _secondaryId != null && _secondaryId.IsActive;
        }

        private void PlayMusic(MusicData musicData, ref SoundManager.GeneratedId soundId)
        {
            var data = musicData.sourceData;
            var doesPauseCurrent = musicData.doesPauseCurrent;
            
            soundId = SoundManager.PlayMusic(data,null, doesPauseCurrent);
        }
        private void StopMusic(ref SoundManager.GeneratedId soundId) => SoundManager.StopSound(soundId);
        private void PauseMusic(ref SoundManager.GeneratedId soundId) => SoundManager.PauseSound(soundId);
        private void ResumeMusic(MusicData musicData, ref SoundManager.GeneratedId soundId)
        {
            var doesPauseCurrent = musicData.doesPauseCurrent;
            SoundManager.ResumeMusic(soundId,doesPauseCurrent);
        }

        private void SetAsBackground(MusicData musicData, ref SoundManager.GeneratedId soundId, ref bool isBackground)
        {
            isBackground = true;
            var volume = musicData.backgroundVolume;
            SoundManager.SetMusicBackground(soundId,volume);
        }
        
        private void SetAsForeground(ref SoundManager.GeneratedId soundId, ref bool isBackground)
        {
            isBackground = false;
            SoundManager.SetMusicForeground(soundId);
        }

        public void PlayPrimaryMusic() => PlayMusic(primaryMusicData, ref _primaryId);
        public void StopPrimaryMusic() => StopMusic(ref _primaryId);
        public void PausePrimaryMusic() => PauseMusic(ref _primaryId);
        public void ResumePrimaryMusic() => ResumeMusic(primaryMusicData, ref _primaryId);
        public void BackgroundPrimary() => SetAsBackground(primaryMusicData, ref _primaryId, ref _isPrimaryBackground);
        public void ForegroundPrimary() => SetAsForeground(ref _primaryId, ref _isPrimaryBackground);

        public void PlaySecondaryMusic() => PlayMusic(secondaryMusicData, ref _secondaryId);
        public void StopSecondaryMusic() => StopMusic(ref _secondaryId);
        public void PauseSecondaryMusic() => PauseMusic(ref _secondaryId);
        public void ResumeSecondaryMusic() => ResumeMusic(secondaryMusicData, ref _secondaryId);
        public void BackgroundSecondary() => SetAsBackground(secondaryMusicData, ref _secondaryId, ref _isSecondaryBackground);
        public void ForegroundSecondary() => SetAsForeground(ref _secondaryId, ref _isSecondaryBackground);
        
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MusicTester))]
    public class MusicTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();
            
            // Agrega el botón
            MusicTester script = (MusicTester)target;
            
            if(script.GetHasStarted() == false) 
                return;
            
            GUILayout.Label("Primary Music");
            if (script.GetIsPrimaryPlaying())
            {
                if (GUILayout.Button("Stop")) script.StopPrimaryMusic();  
                if (GUILayout.Button("Pause")) script.PausePrimaryMusic();  
                if (GUILayout.Button("Resume")) script.ResumePrimaryMusic();
                if (script.IsPrimaryBackground == false)
                {
                    if (GUILayout.Button("Set As Background")) script.BackgroundPrimary();  
                }
                else
                {
                    if (GUILayout.Button("Set As Foreground")) script.ForegroundPrimary();  
                }
                
            }
            else
            {
                if (GUILayout.Button("Play")) script.PlayPrimaryMusic();
            }
            
            GUILayout.Space(15);
            
            GUILayout.Label("Secondary Music");
            if (script.GetIsSecondaryPlaying())
            {
                if (GUILayout.Button("Stop")) script.StopSecondaryMusic();  
                if (GUILayout.Button("Pause")) script.PauseSecondaryMusic();  
                if (GUILayout.Button("Resume")) script.ResumeSecondaryMusic();
                if (script.IsSecondaryBackground == false)
                {
                    if (GUILayout.Button("Set As Background")) script.BackgroundSecondary();  
                }
                else
                {
                    if (GUILayout.Button("Set As Foreground")) script.ForegroundSecondary();  
                }
            }
            else
            {
                if (GUILayout.Button("Play")) script.PlaySecondaryMusic();
            }
        }
    }
#endif
}