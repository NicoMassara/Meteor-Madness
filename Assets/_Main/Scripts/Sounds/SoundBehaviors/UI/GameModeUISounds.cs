using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class GameModeUISounds : UiSoundBehavior<IGameModeUISounds>
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
            GetComponentToSound.OnOptionsButtonPressed += () =>
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