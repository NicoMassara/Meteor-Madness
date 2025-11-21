using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class MainMenuUISounds : UiSoundBehavior<IMainMenuUISounds>
    {
        
        private void Start()
        {
            GetComponentToSound.OnConfirmButtonClicked += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            GetComponentToSound.OnCancelButtonClicked += () =>
            {
                PlayUISound(UISoundType.Default);
            };
            GetComponentToSound.OnBackButtonClicked += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}