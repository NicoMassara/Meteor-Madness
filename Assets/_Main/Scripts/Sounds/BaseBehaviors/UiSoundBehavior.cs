using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;

namespace _Main.Scripts.Sounds
{
    public class UiSoundBehavior<T> : SoundBehaviour<T>
        where T : ISoundComponent
    {
        protected void PlayUISound(UISoundType soundType)
        {
            SoundManager.PlayUISound(soundType, false);
        }
    }
}