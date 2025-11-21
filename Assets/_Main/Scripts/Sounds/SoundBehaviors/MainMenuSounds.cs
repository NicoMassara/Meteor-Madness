using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.MainMenu.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class MainMenuSounds : MusicBehavior<IMainMenuSounds>
    {
        [SerializeField] private SoundClassSo music;
        private GeneratedId _musicId;
        
        private void Start()
        {
            GetComponentToSound.OnMainMenuEnable += () =>
            {
                _musicId = PlayMusic(music, _musicId, true);
            };
        }
    }
}