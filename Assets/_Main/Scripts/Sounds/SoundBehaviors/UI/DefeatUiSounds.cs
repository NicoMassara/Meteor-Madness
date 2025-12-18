using System;
using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;

namespace _Main.Scripts.Sounds.Components
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