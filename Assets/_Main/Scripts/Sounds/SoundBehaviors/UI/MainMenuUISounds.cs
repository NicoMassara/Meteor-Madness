using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class MainMenuUISounds : UiSoundBehavior<IMainMenuUISounds>
    {
        
        private void Start()
        {
            ComponentToSound.OnConfirmButtonClicked += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            ComponentToSound.OnCancelButtonClicked += () =>
            {
                PlayUISound(UISoundType.Default);
            };
            ComponentToSound.OnBackButtonClicked += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}