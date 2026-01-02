using System;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class GameModeUISounds : UiSoundBehavior<IGameModeUISounds>
    {
        [SerializeField] private SoundSourceDataSo pointsAdded;
        
        private void Start()
        {
            ComponentToSound.OnPauseButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
        }
    }
}