
using MeteorMadness.GlobalValues.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.SoundBehaviors
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