using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class MusicBehavior<T> : SoundBehaviour<T>
        where T : ISoundComponent
    {
        protected GeneratedId PlayMusic(SoundClassSo soundClass, GeneratedId musicId, bool isIsolated = false)
        {
            if (IsIdValid(musicId))
            {
                return musicId;
            }
            
            return SoundManager.PlayMusic(soundClass,isIsolated,musicId);
        }
        
        protected void StopAllMusic()
        {
            SoundManager.StopAllMusic();
        }
    }
}