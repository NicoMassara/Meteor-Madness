using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class EarthSounds : SoundBehaviour<IEarthSounds>
    {
        [SerializeField] private SoundClassSo collision;
        [SerializeField] private SoundClassSo death;
        [SerializeField] private SoundClassSo heal;

        private void Start()
        {
            ComponentToSound.OnCollision += () =>
            {
                PlaySound(collision);
            };
            ComponentToSound.OnHealing += () =>
            {
                PlaySound(heal);
            };
            ComponentToSound.OnDestruction += () =>
            {
                PlaySound(death);
            };
            
        }
    }
}