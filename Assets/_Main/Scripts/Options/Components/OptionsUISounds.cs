using _Main.Scripts.Options.MVC;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.Options
{
    public class OptionsUISounds : SoundBehaviour<OptionsMenuUIView>
    {
        private void Start()
        {
            GetComponentToSound.OnAcceptButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnBackButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}