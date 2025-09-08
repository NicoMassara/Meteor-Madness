using UnityEngine;

namespace _Main.Scripts.Comet
{
    public abstract class FlyingObjectView<T> : MonoBehaviour
    where T : FlyingObjectMotor
    {
        [Header("Sphere Sprite")]
        [Range(0, 100f)]
        [SerializeField] private float maxRotationSpeed = 25;
        [SerializeField] private GameObject sphereObject;
        [Header("Fire Sprite")]
        [SerializeField] private GameObject fireObject;
        
        private T flyingObjectMotor;
        private Oscillator _fireScaleOscillator;
        private Oscillator _fireRotationOscillator;
        private Rotator _sphereRotator;
        private bool _hasFire;
        
        protected virtual void Awake()
        {
            flyingObjectMotor = GetComponent<T>();
        }

        protected virtual void Start()
        {
            flyingObjectMotor.OnValuesSet += OnValuesSetHandler;
            _hasFire = fireObject != null;

            if (_hasFire)
            {
                _fireScaleOscillator = new Oscillator(50, 0.12f, 1);
                _fireRotationOscillator = new Oscillator(10, 1, 90);
            }

            _sphereRotator = new Rotator(sphereObject.transform, maxRotationSpeed);
            _sphereRotator.SetSpeed(GetRotationSpeed());
        }
        
        protected virtual void Update()
        {
            _sphereRotator.Rotate();
            if (_hasFire)
            {
                fireObject.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
                fireObject.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
            }
        }

        protected virtual void OnValuesSetHandler()
        {
            _sphereRotator.SetSpeed(GetRotationSpeed());
        }

        protected float GetRotationSpeed()
        {
            return Random.Range(maxRotationSpeed * 0.75f,maxRotationSpeed * 1.25f);
        }
        
    }
}