using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class AbilitySounds : SoundBehaviour<IAbilitySounds>
    {
        [Header("Ability Sounds")]
        [SerializeField] private SoundSourceDataSo added;
        [SerializeField] private SoundSourceDataSo triggered;
        [Header("Time Sounds")]
        [SerializeField] private SoundSourceDataSo speedUp;
        [SerializeField] private SoundSourceDataSo slowDown;
        
        private void Start()
        {
            ComponentToSound.OnAbilityAdded += () =>
            {
                PlaySound(added);
            };
            ComponentToSound.OnAbilityTriggered += () =>
            {
                PlaySound(triggered);
            };
            ComponentToSound.OnTimeSpeedUp += () =>
            {
                PlaySound(speedUp);
            };
            ComponentToSound.OnTimeSlowDown += () =>
            {
                PlaySound(slowDown);
            };
        }
    }
}