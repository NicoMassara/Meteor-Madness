using _Main.Scripts.Interfaces.Sounds;

namespace _Main.Scripts.Sounds.Components
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