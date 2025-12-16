using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class EarthSounds : SoundBehaviour<IEarthSounds>
    {
        [SerializeField] private SoundSourceDataSo collision;
        [SerializeField] private SoundSourceDataSo death;
        [SerializeField] private SoundSourceDataSo heal;

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