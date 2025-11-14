using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldSounds : SoundBehaviour<ShieldView>
    {
        [SerializeField] private SoundClassSo deflect;
        [SerializeField] private SoundClassSo rotate;
        [SerializeField] private SoundClassSo shieldActivated;
        [Space]
        [Header("Abilities")]
        [Space]
        [Header("Start")]
        [SerializeField] private SoundClassSo superShieldStart;
        [Space]
        [Header("Running")]
        [SerializeField] private SoundClassSo doublePointsRunning;
        [SerializeField] private SoundClassSo automaticRunning;
        [SerializeField] private SoundClassSo superShieldRunning;

        private SoundId _abilityRunningSoundId;

        private void Start()
        {
            GetComponentToSound.OnShieldActivated += (isActive) =>
            {
                if (isActive)
                {
                    PlaySound(shieldActivated);
                }
            };
            
            GetComponentToSound.OnRotate += () =>
            {
                PlaySound(rotate);
            };
            
            GetComponentToSound.OnDeflect += () =>
            {
                PlaySound(deflect);
            };
            
            GetComponentToSound.OnAbilityStarted += (ability) =>
            {
                PlaySound(GetAbilityStartSound(ability));
            };
            
            GetComponentToSound.OnAbilityRunning += (ability) =>
            {
                _abilityRunningSoundId = PlaySound(GetAbilityRunningSound(ability));
            };
            
            GetComponentToSound.OnAbilityFinished += () =>
            {
                StopSound(_abilityRunningSoundId);
            };
        }
        
        private SoundClassSo GetAbilityStartSound(AbilityType abilityType)
        {
            return abilityType switch
            {
                AbilityType.SuperShield => superShieldStart,
                _ => superShieldStart
            };
        }
        
        private SoundClassSo GetAbilityRunningSound(AbilityType abilityType)
        {
            return abilityType switch
            {
                AbilityType.SuperShield => superShieldRunning,
                AbilityType.DoublePoints => doublePointsRunning,
                AbilityType.Automatic => automaticRunning,
                _ => null
            };
        }
    }
}