using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Menu
{
    public class ItemRotator : MonoBehaviour
    {
        [Range(0,25)]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform itemTransform;
        private Rotator _rotator;

        private void Start()
        {
            _rotator = new Rotator(itemTransform,rotationSpeed);
        }

        private void Update()
        {
            _rotator.Rotate();
        }
    }
}