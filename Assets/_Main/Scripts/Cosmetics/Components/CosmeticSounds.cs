using System;
using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class CosmeticSounds : SoundBehaviour<CosmeticView>
    {
        [SerializeField] private SoundClassSo backgroundMusic;

        private void Start()
        {
            GetComponentToSound.OnCosmeticEnable += () =>
            {

            };
        }
    }
}