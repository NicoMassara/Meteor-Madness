using System;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class DefeatUiSounds : UiSoundBehavior<IDefeatUiSounds>
    {
        private void Start()
        {
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
            
            ComponentToSound.OnRestartButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
        }
    }
}