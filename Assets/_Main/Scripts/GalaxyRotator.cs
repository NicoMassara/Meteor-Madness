using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts
{
    public class GalaxyRotator : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Transform[] galaxies;
        [Range(0f, 1f)]
        [SerializeField] private float rotationSpeed = 0.5f;
        
        private Rotator _rotator; 
        public UpdateGroup SelfUpdateGroup { get; private set; } = UpdateGroup.Gameplay;

        private void Start()
        {
            _rotator = new Rotator(galaxies,Vector3.forward,rotationSpeed);
        }

        public void ManagedUpdate()
        {
            _rotator.Rotate(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }
    }
}