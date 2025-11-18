
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class ItemRotator : ManagedBehavior, IUpdatable
    {
        [Range(0,25)]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform itemTransform;
        private Rotator _rotator;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Start()
        {
            _rotator = new Rotator(itemTransform,Vector3.forward, rotationSpeed);
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            _rotator.Rotate(deltaTime);
        }
    }
}