using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.Behaviours
{
    public class GameModeVibration : VibrationBehavior<IGameModeVibration>
    {
        [SerializeField] private VibrationDataSo countdownFinish;
        [SerializeField] private VibrationDataSo countdownUpdate;
        
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnCountDownFinished += () =>
            {
                Vibrate(countdownFinish);
            };
            
            ComponentToVibrate.OnCountdownUpdated += (time) =>
            {
                Vibrate(countdownUpdate);
            };
        }
#endif
    }
}