using System;
using _Main.Scripts.Shaker;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorView : MonoBehaviour
    {
        [Header("Meteor")] 
        [Range(0, 100)] 
        [SerializeField] private float maxRotationSpeed = 25;
        [SerializeField] private GameObject meteorObject;
        [Header("Fire")]
        [SerializeField] private GameObject fireObject;
        
        private MeteorMotor _motor;
        private Oscillator _fireScaleOscillator;
        private Oscillator _fireRotationOscillator;
        private Rotator _meteorRotator;
        

        private void Awake()
        {
            _motor = GetComponent<MeteorMotor>();
        }

        private void Start()
        {
            _motor.OnValuesSet += OnValuesSetHandler;
            _fireScaleOscillator = new Oscillator(50, 0.12f, 1);
            _fireRotationOscillator = new Oscillator(10, 1, 90);
            _meteorRotator = new Rotator(meteorObject.transform, maxRotationSpeed);
            _meteorRotator.SetSpeed(GetRotationSpeed());
        }

        private void Update()
        {
            _meteorRotator.Rotate();
            fireObject.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
            fireObject.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
        }

        private void OnValuesSetHandler()
        {
            _meteorRotator.SetSpeed(GetRotationSpeed());
        }

        private float GetRotationSpeed()
        {
            return Random.Range(maxRotationSpeed * 0.75f,maxRotationSpeed * 1.25f);
        }
    }
}