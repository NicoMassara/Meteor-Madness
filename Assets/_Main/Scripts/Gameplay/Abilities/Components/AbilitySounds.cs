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