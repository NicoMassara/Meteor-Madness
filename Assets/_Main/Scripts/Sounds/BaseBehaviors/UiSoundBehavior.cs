using MeteorMadness.GlobalValues.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.BaseBehaviors
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