using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Components
{
    public class AbilitySounds : SoundBehaviour<AbilityView>
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