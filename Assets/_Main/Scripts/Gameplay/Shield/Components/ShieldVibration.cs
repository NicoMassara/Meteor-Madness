using System;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldVibration : VibrationBehavior<ShieldView>
    {
#if UNITY_ANDROID 
        [SerializeField] private VibrationDataSo rotateData;
        [SerializeField] private VibrationDataSo deflectData;
        
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnRotate += () =>
            {
                Vibrate(rotateData);
            };
            
            ComponentToVibrate.OnDeflect += () =>
            {
                Vibrate(deflectData);
            };
        }
#endif
    }
}