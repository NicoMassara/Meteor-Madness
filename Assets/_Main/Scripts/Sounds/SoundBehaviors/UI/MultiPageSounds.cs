
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class MultiPageSounds : UiSoundBehavior<IMultiPageUISounds>
    {
        private void Start()
        {
            ComponentToSound.OnNextButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            ComponentToSound.OnPreviousButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}