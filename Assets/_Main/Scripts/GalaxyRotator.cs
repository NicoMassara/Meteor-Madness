using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class GalaxyRotator : MonoBehaviour
    {
        [SerializeField] private Transform[] galaxies;
        [Range(0f, 1f)]
        [SerializeField] private float rotationSpeed = 0.5f;
        
        private Rotator _rotator; 

        private void Start()
        {
            _rotator = new Rotator(galaxies, rotationSpeed);
        }

        private void Update()
        {
            _rotator.Rotate();
        }
    }
}