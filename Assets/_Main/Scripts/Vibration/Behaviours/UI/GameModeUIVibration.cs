using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.Behaviours
{
    public class GameModeUIVibration : VibrationBehavior<IGameModeUIVibration>
    {
        [SerializeField] private VibrationDataSo pointsAdded;
        
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnPauseButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnPointsAdded += () =>
            {
                Vibrate(pointsAdded);
            };
        }
#endif
    }
}