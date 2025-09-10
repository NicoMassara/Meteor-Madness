using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace _Main.Scripts.FyingObject
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingObjectView<T, TS> : MonoBehaviour, IObserver
    where T : FlyingObjectMotor
    where TS : FlyingObjectView<T, TS>
    {
        [Header("Sounds")]
        [SerializeField] protected SoundBehavior moveSound;
        [Header("Sphere Sprite")]
        [Range(0, 100f)]
        [SerializeField] private float maxRotationSpeed = 25;
        [SerializeField] private GameObject sphereObject;
        [Header("Fire Sprite")]
        [SerializeField] private GameObject fireObject;
        [Header("Particles")]
        [SerializeField] private ParticleDataSo collisionParticle;
        
        private FlyingObjectController<T> _controller;
        private Rigidbody2D _rigidbody2D;
        private Oscillator _fireScaleOscillator;
        private Oscillator _fireRotationOscillator;
        private Rotator _sphereRotator;
        private bool _hasFire;
        private bool _canMove;
        private float _movementSpeed;

        public UnityAction<TS> OnRecycle;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2D.simulated = true;
            _rigidbody2D.mass = 0.0001f;
            _rigidbody2D.drag = 0f;
            _rigidbody2D.angularDrag = 0.05f;
            _rigidbody2D.gravityScale = 0f;
        }

        private void Start()
        {
            _hasFire = fireObject != null;

            if (_hasFire)
            {
                _fireScaleOscillator = new Oscillator(50, 0.12f, 1);
                _fireRotationOscillator = new Oscillator(10, 1, 90);
            }

            _sphereRotator = new Rotator(sphereObject.transform, maxRotationSpeed);
            _sphereRotator.SetSpeed(GetRotationSpeed());
        }

        private void Update()
        {
            _sphereRotator.Rotate();
            
            if (_hasFire)
            {
                fireObject.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
                fireObject.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
            }
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                _rigidbody2D.transform.Translate(Vector2.right * (_movementSpeed * Time.deltaTime));
            }
        }

        public virtual void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case FlyingObjectObserverMessage.SetValues:
                    HandleSetValues((float)args[0], (Quaternion)args[1], (Vector2)args[2], (bool)args[3]);
                    break;
                case FlyingObjectObserverMessage.HandleCollision:
                    HandleCollision((bool)args[0], (Vector2)args[1], (bool)args[2]);
                    break;
            }
        }

        public void SetValues(float movementSpeed, Quaternion rotation, Vector2 position)
        {
            _controller.SetValues(movementSpeed,rotation,position);
        }

        protected virtual void HandleCollision(bool canMove, Vector2 position, bool doesShowParticles)
        {
            _canMove = canMove;
            if (false)
            {
                GameManager.Instance.EventManager.Publish
                (
                    new SpawnParticle
                    {
                        ParticleData = collisionParticle,
                        Position = position
                    }
                );
            }

            moveSound?.StopSound();
        }
        
        public void SetController(FlyingObjectController<T> controller)
        {
            _controller = controller;
        }

        private void HandleSetValues(float movementSpeed, Quaternion rotation, Vector2 position, bool canMove)
        {
            _movementSpeed = movementSpeed;
            _rigidbody2D.transform.rotation = rotation;
            _rigidbody2D.transform.position = position;
            _canMove = canMove;
            moveSound?.PlaySound(1);
        }
        
        protected float GetRotationSpeed()
        {
            return Random.Range(maxRotationSpeed * 0.75f,maxRotationSpeed * 1.25f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _controller.HandleTriggerEnter2D(other);
        }

        public void ForceRecycle()
        {
            moveSound?.StopSound();
            OnRecycle?.Invoke((TS)this);
        }
    }
}