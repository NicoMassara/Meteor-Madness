using _Main.Scripts.Interfaces.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours
{
    public class ShieldVibration : VibrationBehavior<IShieldVibration>
    {
#if UNITY_ANDROID 
        [SerializeField] private VibrationDataSo rotateData;
        [SerializeField] private VibrationDataSo directionChangeData;
        [SerializeField] private VibrationDataSo stopData;
        [SerializeField] private VibrationDataSo deflectData;
        
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