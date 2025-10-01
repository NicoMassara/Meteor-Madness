using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
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
        private void Start()
        {
            _rotator = new Rotator(itemTransform,Vector3.forward, rotationSpeed);
        }
        
        public void ManagedUpdate()
        {
            _rotator.Rotate(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }
    }
}