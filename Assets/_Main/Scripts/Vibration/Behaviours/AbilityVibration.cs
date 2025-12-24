using _Main.Scripts.Interfaces.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours
{
    public class AbilityVibration : VibrationBehavior<IAbilityVibration>
    {
        [Header("Vibration Data")]
        [SerializeField] private VibrationDataSo triggeredData;
        [SerializeField] private VibrationDataSo finishedData;
        [SerializeField] private VibrationDataSo addedData;
        
#if UNITY_ANDROID
        
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
#endif
    }
}