using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class GameModeSounds : MusicBehavior<IGameModeSounds>
    {
        [Header("Music")]
        [SerializeField] private SoundClassSo gameMusicData;
        [SerializeField] private SoundClassSo deathMusicData;
        [SerializeField] private SoundClassSo pauseMusicData;
        [Space]
        [Header("Countdown")]
        [SerializeField] private SoundClassSo countdownSound;
        [SerializeField] private SoundClassSo countdownFinish;
        
        private GeneratedId _gameMusicId;
        private GeneratedId _deathMusicId;
        private GeneratedId _pauseMusicId;

        private void PlayGameMusic(bool isIsolated = false)
        {
            _gameMusicId = PlayMusic(gameMusicData, _gameMusicId,isIsolated);
        }

        private void Start()
        {
            // Music
            
            ComponentToSound.OnCountdownFinished += () =>
            {
                // Adds a delay
                
                TimerManager.Add(new TimerData(0.5f,
                    () => { PlayGameMusic(true); }));
            };
            
            ComponentToSound.OnGameModePaused += (isPaused) =>
            {
                if (isPaused)
                {
                    PauseSound(_gameMusicId);

                    _pauseMusicId = PlayMusic(pauseMusicData, _pauseMusicId);
                }
                else
                {
                    Debug.Log("Trying to Pause Music of Paused Panel");

                }
            };   
            
            ComponentToSound.OnCountDownStarted += () =>
            {
                if(_pauseMusicId == null) return;
                
                StopSound(_pauseMusicId);
            };  
            
            ComponentToSound.OnGameModeFinished += () =>
            {
                _deathMusicId = PlayMusic(deathMusicData, _deathMusicId);
            };   
            
            ComponentToSound.OnStopMusic += () =>
            {
                StopAllMusic();
            };  
            
            // Sounds
            
            ComponentToSound.OnCountdownUpdated += () =>
            {
                PlaySound(countdownSound);
            };
            
            ComponentToSound.OnCountdownUpdatedFinished += () =>
            {
                PlaySound(countdownFinish);
            };
        }
    }
}