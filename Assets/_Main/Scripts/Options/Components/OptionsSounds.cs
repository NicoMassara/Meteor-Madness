using System;
using _Main.Scripts.Options.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Options
{
    public class OptionsSounds : SoundBehaviour<OptionsMenuView>
    {
        [SerializeField] private SoundClassSo music; 
        
        private void Start()
        {
            GetComponentToSound.OnOptionsMenuEnable += () =>
            {
                PlaySound(music);
            };
            
            GetComponentToSound.OnOptionsMenuDisable += () =>
            {
                StopMusic();
            };
        }
    }
}