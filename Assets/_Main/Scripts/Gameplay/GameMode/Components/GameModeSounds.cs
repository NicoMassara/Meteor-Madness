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

        private void Start()
        {
            GetComponentToSound.OnGameModeEnable += () =>
            {
                StopMusic();
            };

            GetComponentToSound.OnGameModeStarted += () =>
            {
                PlayMusic(gameMusic);
            };
            GetComponentToSound.OnGameModeFinished += () =>
            {
                PlayMusic(deathMusic);
            };
            
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