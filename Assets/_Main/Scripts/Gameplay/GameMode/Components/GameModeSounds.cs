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
            ComponentToSound.OnGameModeEnable += () =>
            {
                StopMusic();
            };

            ComponentToSound.OnGameModeStarted += () =>
            {
                PlayMusic(gameMusic);
            };
            ComponentToSound.OnGameModeFinished += () =>
            {
                PlayMusic(deathMusic);
            };
            
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