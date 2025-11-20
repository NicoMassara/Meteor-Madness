using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeSounds : SoundBehaviour<GameModeView>
    {
        [Header("Music")]
        [SerializeField] private SoundClassSo gameMusic;
        [SerializeField] private SoundClassSo deathMusic;
        [Space]
        [Header("Countdown")]
        [SerializeField] private SoundClassSo countdownSound;
        [SerializeField] private SoundClassSo countdownFinish;
        
        private SoundId _gameMusicId;
        private SoundId _deathMusicId;
        private SoundId _pauseMusicId;

        private void Start()
        {
            // Music
            
            GetComponentToSound.OnGameModeEnable += () =>
            {
                StopMusic();
            };
            
            GetComponentToSound.OnGameModePaused += (isPaused) =>
            {
                if (isPaused)
                {
                    SoundManager.Instance.PauseMusic();
                    _pauseMusicId = PlayMusic(deathMusic,_pauseMusicId);
                }
                else
                {
                    SoundManager.Instance.ResumeMusic();
                }
            };   

            GetComponentToSound.OnGameModeStarted += () =>
            {
                _gameMusicId = PlayMusic(gameMusic,_gameMusicId);
            };
            GetComponentToSound.OnGameModeFinished += () =>
            {
                _deathMusicId = PlayMusic(deathMusic,_deathMusicId);
            };
            GetComponentToSound.OnEarthDeath += () =>
            {
                StopMusic();
            };         
            
            GetComponentToSound.OnCountDownStarted += () =>
            {
                StopMusic();
            };

            GetComponentToSound.OnLeaving += () =>
            {
                RemoveAllSounds();
            };
            
            GetComponentToSound.OnGameModeRestarted += () =>
            {
                RemoveAllSounds();
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

        private void RemoveAllSounds()
        {
            SoundManager.Instance.ClearAllMusic();
            //
            SoundManager.Instance.RemoveMusic(ref _gameMusicId);
            SoundManager.Instance.RemoveMusic(ref _deathMusicId);
            SoundManager.Instance.RemoveMusic(ref _pauseMusicId);
        }
    }
}