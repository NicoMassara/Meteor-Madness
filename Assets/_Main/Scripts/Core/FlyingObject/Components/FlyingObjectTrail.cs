using System;
using MeteorMadness.GlobalValues.Tools;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Core.FlyingObject.Components
{
    public class FlyingObjectTrail : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Transform trailTransform;
        [SerializeField] private FlyingObjectTrailDataSo dataSo;
        private Oscillator _fireScaleOscillator;
        private Oscillator _fireRotationOscillator;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTarget;

        private void Start()
        {
            var scaleData = dataSo.TrailScaleData;
            _fireScaleOscillator = new Oscillator(scaleData.speed, scaleData.amplitude, scaleData.offset);
            var rotData = dataSo.TrailRotateData;
            _fireRotationOscillator = new Oscillator(rotData.speed, rotData.amplitude, rotData.offset);
        }


        public void ExecuteUpdate(float deltaTime)
        {
            if(trailTransform == null) return;
            
            trailTransform.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
            trailTransform.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
        }
    }
}