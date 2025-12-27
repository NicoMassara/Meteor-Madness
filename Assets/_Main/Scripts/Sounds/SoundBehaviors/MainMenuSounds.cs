using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class MainMenuSounds : MusicBehavior<IMainMenuSounds>
    {
        [SerializeField] private SoundSourceDataSo music;
        private SoundManager.GeneratedId _musicId;
        
        private void Start()
        {
            ComponentToSound.OnMainMenuEnable += () =>
            {
                _musicId = PlayMusic(music, _musicId, true);
            };
        }
    }
}