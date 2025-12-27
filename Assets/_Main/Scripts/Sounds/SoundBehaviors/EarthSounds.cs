using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace MeteorMadness.Sounds.SoundBehaviors
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