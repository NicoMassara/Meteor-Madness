using NicolasMassara.CustomUpdateManager;
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
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Start()
        {
            _rotator = new Rotator(galaxies,Vector3.forward,rotationSpeed);
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _rotator.Rotate(deltaTime);
        }
    }
}