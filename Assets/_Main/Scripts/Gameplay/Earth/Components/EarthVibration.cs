using System;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthVibration : VibrationBehavior<EarthView>
    {
        [SerializeField] private VibrationDataSo collisionData;
        
        private void Start()
        {
            ComponentToVibrate.OnCollision += () =>
            {
                Vibrate(collisionData);
            };
        }
    }
}