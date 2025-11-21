using _Main.Scripts.Interfaces.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class MultiPageSounds : UiSoundBehavior<IMultiPageUISounds>
    {
        private void Start()
        {
            GetComponentToSound.OnNextButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnPreviousButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}