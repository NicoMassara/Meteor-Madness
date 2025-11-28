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
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
            ComponentToSound.OnRestartButtonPressed += () =>
            {
                PlayUISound(UISoundType.Default);
            };
            ComponentToSound.OnResumeButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            ComponentToSound.OnPauseButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            ComponentToSound.OnOptionsButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            ComponentToSound.OnPointsAdded += () =>
            {
                PlaySound(pointsAdded);
            };
        }
    }
}