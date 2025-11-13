using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUISounds : SoundBehaviour<GameModeUIView>
    {
        [SerializeField] private SoundClassSo pointsAdded;
        
        private void Start()
        {
            GetComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
            GetComponentToSound.OnRestartButtonPressed += () =>
            {
                PlayUISound(UISoundType.Default);
            };
            GetComponentToSound.OnResumeButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            GetComponentToSound.OnPauseButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            GetComponentToSound.OnPointsAdded += () =>
            {
                PlaySound(pointsAdded);
            };
        }
    }
}