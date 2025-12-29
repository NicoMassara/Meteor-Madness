
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class PausePanelSounds : UiSoundBehavior<IPausePanelUISounds>
    {
        private void Start()
        {
            ComponentToSound.OnResumeButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            ComponentToSound.OnOptionsButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}