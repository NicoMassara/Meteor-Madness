using System;
using _Main.Scripts.MainMenu.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class MainMenuSounds : SoundBehaviour<MainMenuView>
    {
        [SerializeField] private SoundClassSo music;
        
        private void Start()
        {
            GetComponentToSound.OnMainMenuEnable += () =>
            {
                PlayMusic(music);
            };
        }
    }
}