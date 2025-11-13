using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthSounds : SoundBehaviour<EarthView>
    {
        [SerializeField] private SoundClassSo collision;
        [SerializeField] private SoundClassSo death;
        [SerializeField] private SoundClassSo heal;

        private void Start()
        {
            GetComponentToSound.OnCollision += () =>
            {
                PlaySound(collision);
            };
            GetComponentToSound.OnHealed += () =>
            {
                PlaySound(heal);
            };
            GetComponentToSound.OnDestruction += () =>
            {
                PlaySound(death);
            };
            
        }
    }
}