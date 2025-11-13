using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUISounds : SoundBehaviour<GameModeUIView>
    {
        [SerializeField] private SoundClassSo pointsAdded;
        [Header("Buttons")]
        [SerializeField] private SoundClassSo confirmButton;
        [SerializeField] private SoundClassSo cancelButton;
        [SerializeField] private SoundClassSo pauseButton;
        
        private void Start()
        {
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlaySound(cancelButton);
            };
            ComponentToSound.OnRestartButtonPressed += () =>
            {
                PlaySound(confirmButton);
            };
            ComponentToSound.OnResumeButtonPressed += () =>
            {
                PlaySound(confirmButton);
            };
            ComponentToSound.OnPauseButtonPressed += () =>
            {
                PlaySound(pauseButton);
            };
            ComponentToSound.OnPointsAdded += () =>
            {
                PlaySound(pointsAdded);
            };
        }
    }
}