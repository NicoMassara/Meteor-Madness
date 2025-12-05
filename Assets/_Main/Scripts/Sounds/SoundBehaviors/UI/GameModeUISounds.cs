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
            ComponentToSound.OnPauseButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
        }
    }
}