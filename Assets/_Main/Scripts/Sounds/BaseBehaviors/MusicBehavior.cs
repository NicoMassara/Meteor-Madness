using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;


namespace _Main.Scripts.Sounds
{
    public class MusicBehavior<T> : SoundBehaviour<T>
        where T : ISoundComponent
    {
        protected SoundManager.GeneratedId PlayMusic(ISoundSourceData soundClass, SoundManager.GeneratedId musicId, bool isIsolated = false)
        {
            if (IsIdValid(musicId))
            {
                return musicId;
            }
            
            return SoundManager.PlayMusic(soundClass,null,isIsolated,musicId);
        }
        
        protected void StopAllMusic()
        {
            SoundManager.StopMusic();
        }
    }
}