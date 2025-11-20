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

        private void Start()
        {
            
            // Music
            
            GetComponentToSound.OnGameModeEnable += () =>
            {
                StopMusic();
            };
            
            GetComponentToSound.OnGameModeDisable += () =>
            {
                StopMusic();
            };   
            
            GetComponentToSound.OnGameModePaused += (isPaused) =>
            {
                if (isPaused)
                {
                    SoundManager.Instance.PauseMusic();
                    PlayMusic(deathMusic,_deathMusicId);
                }
                else
                {
                    //StopMusic();
                    SoundManager.Instance.ResumeMusic();
                }
            };   

            GetComponentToSound.OnGameModeStarted += () =>
            {
                PlayMusic(gameMusic,_gameMusicId);
            };
            GetComponentToSound.OnGameModeFinished += () =>
            {
                PlayMusic(deathMusic,_deathMusicId);
            };
            
            GetComponentToSound.OnEarthDeath += () =>
            {
                StopMusic();
            };         
            
            GetComponentToSound.OnCountDownStarted += () =>
            {
                StopMusic();
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