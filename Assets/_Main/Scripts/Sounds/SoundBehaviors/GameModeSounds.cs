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
            
            GetComponentToSound.OnCountdownFinished += () =>
            {
                // Adds a delay
                
                TimerManager.Add(new TimerData(0.5f,
                    () => { PlayGameMusic(true); }));
            };
            
            GetComponentToSound.OnGameModePaused += (isPaused) =>
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
            
            GetComponentToSound.OnCountDownStarted += () =>
            {
                if(_pauseMusicId == null) return;
                
                StopSound(_pauseMusicId);
            };  
            
            GetComponentToSound.OnGameModeFinished += () =>
            {
                _deathMusicId = PlayMusic(deathMusicData, _deathMusicId);
            };   
            
            GetComponentToSound.OnStopMusic += () =>
            {
                StopAllMusic();
            };  
            
            // Sounds
            
            GetComponentToSound.OnCountdownUpdated += () =>
            {
                PlaySound(countdownSound);
            };
            
            GetComponentToSound.OnCountdownUpdatedFinished += () =>
            {
                PlaySound(countdownFinish);
            };
        }
    }
}