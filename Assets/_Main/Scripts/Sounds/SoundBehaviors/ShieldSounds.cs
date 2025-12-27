using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class ShieldSounds : SoundBehaviour<IShieldSounds>
    {
        [SerializeField] private SoundSourceDataSo deflect;
        [SerializeField] private SoundSourceDataSo rotate;
        [SerializeField] private SoundSourceDataSo shieldActivated;
        [Space]
        [Header("Abilities")]
        [Space]
        [Header("Start")]
        [SerializeField] private SoundSourceDataSo superShieldStart;
        [Space]
        [Header("Running")]
        [SerializeField] private SoundSourceDataSo doublePointsRunning;
        [SerializeField] private SoundSourceDataSo automaticRunning;
        [SerializeField] private SoundSourceDataSo superShieldRunning;

        private SoundManager.GeneratedId _abilityRunningSoundId;

        private void Start()
        {
            ComponentToSound.OnShieldActivated += (isActive) =>
            {
                if (isActive)
                {
                    //PlaySound(shieldActivated);
                }
            };
            
            ComponentToSound.OnRotate += () =>
            {
                PlaySound(rotate);
            };
            
            ComponentToSound.OnDeflect += () =>
            {
                PlaySound(deflect);
            };
            
            ComponentToSound.OnAbilityStarted += (ability) =>
            {
                PlaySound(GetAbilityStartSound(ability));
            };
            
            ComponentToSound.OnAbilityRunning += (ability) =>
            {
                _abilityRunningSoundId = PlaySound(GetAbilityRunningSound(ability));
            };
            
            ComponentToSound.OnAbilityFinished += () =>
            {
                StopSound(_abilityRunningSoundId);
            };
        }
        
        private SoundSourceDataSo GetAbilityStartSound(AbilityType abilityType)
        {
            return abilityType switch
            {
                AbilityType.SuperShield => superShieldStart,
                _ => superShieldStart
            };
        }
        
        private SoundSourceDataSo GetAbilityRunningSound(AbilityType abilityType)
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