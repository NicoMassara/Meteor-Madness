using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours
{
    public class ShieldVibration : VibrationBehavior<IShieldVibration>
    {
        [SerializeField] private VibrationDataSo rotateData;
        [SerializeField] private VibrationDataSo directionChangeData;
        [SerializeField] private VibrationDataSo stopData;
        [SerializeField] private VibrationDataSo deflectData;
#if UNITY_ANDROID
        
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnRotate += () =>
            {
                Vibrate(rotateData);
            };
            
            ComponentToVibrate.OnStopped += () =>
            {
                Vibrate(stopData);
            };
            
            ComponentToVibrate.OnDeflect += () =>
            {
                Vibrate(deflectData);
            };
            
            ComponentToVibrate.OnDirectionChange += (value) =>
            {
                Vibrate(directionChangeData);
            };
        }
#endif
    }
}