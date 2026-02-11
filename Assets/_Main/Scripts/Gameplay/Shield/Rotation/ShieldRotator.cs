using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation
{
    public class ShieldRotator : IShieldRotator
    {
        private readonly IAngularRotation _inputRotation;

        public ShieldRotator(Transform objectToRotate, IAngularRotationData angularRotationData)
        {
            _inputRotation = new AngularRotation(objectToRotate, angularRotationData);
        }
        
        public void Execute(float deltaTime)
        {
            _inputRotation.Execute(deltaTime);
        }

        public IAngularRotation GetInputRotation() => _inputRotation;

    }
}   