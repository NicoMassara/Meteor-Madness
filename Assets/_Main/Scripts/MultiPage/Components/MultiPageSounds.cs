using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.MultiPage.Components
{
    public class MultiPageSounds : SoundBehaviour<MultiPageViewUI>
    {
        [SerializeField] private SoundClassSo nextButton;
        [SerializeField] private SoundClassSo previousButton;
        
        private void Start()
        {
            GetComponentToSound.OnNextButtonPressed += () =>
            {
                PlaySound(nextButton);
            };
            
            GetComponentToSound.OnPreviousButtonPressed += () =>
            {
                PlaySound(previousButton);
            };
        }
    }
}