using System;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldVibration : VibrationBehavior<ShieldView>
    {
        [SerializeField] private VibrationDataSo rotateData;
        [SerializeField] private VibrationDataSo deflectData;
        
        private void Start()
        {
            ComponentToVibrate.OnRotate += () =>
            {
                Vibrate(rotateData);
            };
            
            ComponentToVibrate.OnDeflect += () =>
            {
                Vibrate(deflectData);
            };
        }
    }
}