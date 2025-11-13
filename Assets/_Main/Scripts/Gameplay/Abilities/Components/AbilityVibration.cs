using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Components
{
    public class AbilityVibration : VibrationBehavior<AbilityView>
    {
        [Header("Vibration Data")]
        [SerializeField] private VibrationDataSo triggeredData;
        [SerializeField] private VibrationDataSo finishedData;
        [SerializeField] private VibrationDataSo addedData;
        
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnAbilityTriggered += () =>
            {
                Vibrate(triggeredData);
            };

            ComponentToVibrate.OnAbilityFinished += () =>
            {
                Vibrate(finishedData);
            };

            ComponentToVibrate.OnAbilityAdded += () =>
            {
                Vibrate(addedData);
            };
        }
    }
}