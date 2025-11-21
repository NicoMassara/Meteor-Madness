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
            GetComponentToSound.OnAbilityAdded += () =>
            {
                PlaySound(added);
            };
            GetComponentToSound.OnAbilityTriggered += () =>
            {
                PlaySound(triggered);
            };
            GetComponentToSound.OnTimeSpeedUp += () =>
            {
                PlaySound(speedUp);
            };
            GetComponentToSound.OnTimeSlowDown += () =>
            {
                PlaySound(slowDown);
            };
        }
    }
}