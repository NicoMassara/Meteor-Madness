using System;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using UnityEngine;

namespace _Main.Scripts
{
    public class GalaxyRotator : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Transform[] galaxies;
        [Range(0f, 1f)]
        [SerializeField] private float rotationSpeed = 0.5f;
        
        private Rotator _rotator; 
        public UpdateManager.UpdateGroup UpdateGroup { get; private set; } = UpdateManager.UpdateGroup.Gameplay;

        private void Start()
        {
            _rotator = new Rotator(galaxies, rotationSpeed);
        }

        public void ManagedUpdate()
        {
            _rotator.Rotate();
        }
    }
}