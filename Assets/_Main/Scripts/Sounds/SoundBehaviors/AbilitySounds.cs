using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class AbilitySounds : SoundBehaviour<IAbilitySounds>
    {
        [Header("Ability Sounds")]
        [SerializeField] private SoundClassSo added;
        [SerializeField] private SoundClassSo triggered;
        [Header("Time Sounds")]
        [SerializeField] private SoundClassSo speedUp;
        [SerializeField] private SoundClassSo slowDown;
        
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