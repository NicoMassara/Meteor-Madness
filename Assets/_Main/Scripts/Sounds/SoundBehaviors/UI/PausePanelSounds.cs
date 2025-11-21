using _Main.Scripts.Interfaces.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class PausePanelSounds : UiSoundBehavior<IPausePanelUISounds>
    {
        private void Start()
        {
            GetComponentToSound.OnResumeButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnOptionsButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}