using _Main.Scripts.Interfaces.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class CosmeticsUISounds : UiSoundBehavior<ICosmeticUISounds>
    {
        private void Start()
        {
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}