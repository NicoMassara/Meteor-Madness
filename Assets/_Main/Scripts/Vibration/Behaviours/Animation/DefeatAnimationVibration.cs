using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.BaseBehaviours
{
    public class DefeatAnimationVibration : VibrationBehavior<IDefeatAnimationVibration>
    {
        [SerializeField] private VibrationDataSo textAppearData;
        [SerializeField] private VibrationDataSo titleBounceData;
        [SerializeField] private VibrationDataSo closePanelData;
        
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnVibration += ExecuteVibrationByType;
        }

        private void ExecuteVibrationByType(DefeatAnimationVibrationType vibrationType)
        {
            Vibrate(SelectVibration(vibrationType));
        }

        private VibrationDataSo SelectVibration(DefeatAnimationVibrationType vibrationType)
        {
            return vibrationType switch
            {
                DefeatAnimationVibrationType.TextAppear => textAppearData,
                DefeatAnimationVibrationType.TitleBounce => titleBounceData,
                DefeatAnimationVibrationType.ClosePanel => closePanelData,
                _ => throw new ArgumentOutOfRangeException(nameof(vibrationType), vibrationType, null)
            };
        }
#endif
    }
}