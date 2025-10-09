using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using _Main.Scripts.MyTools;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main.Scripts.FyingObject
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingObjectView<T, TS, TVS> : ManagedBehavior, IObserver, IUpdatable, IFixedUpdatable, IPoolable<TS>
    where T : FlyingObjectMotor<TVS>
    where TS : FlyingObjectView<T, TS, TVS>
    where TVS : FlyingObjectValues
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
        
        private Rigidbody2D _rigidbody2D;
        private Oscillator _fireScaleOscillator;
        private Oscillator _fireRotationOscillator;
        private Rotator _sphereRotator;
        private bool _hasFire;
        private bool _canMove;
        private float _movementSpeed;
        
        public UnityAction<Vector2> OnPositionChanged;
        public UnityAction<TVS> OnValuesChanged;
        public UnityAction<Collider2D> OnCollisionDetected;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public UpdateGroup SelfFixedUpdateGroup { get; } = UpdateGroup.Gameplay;
        public event Action<TS> OnRecycle;
        
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

            _sphereRotator = new Rotator(sphereObject.transform,Vector3.forward, maxRotationSpeed);
            _sphereRotator.SetSpeed(GetRotationSpeed());
        }
        
        public virtual void ManagedUpdate()
        {
            _sphereRotator?.Rotate(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            
            if (_hasFire)
            {
                fireObject.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
                fireObject.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
            }
        }

        public virtual void ManagedFixedUpdate()
        {
            if (_canMove)
            {
                var dt = CustomTime.GetFixedDeltaTimeByChannel(SelfFixedUpdateGroup);
                _rigidbody2D.transform.Translate(Vector2.right * (_movementSpeed * dt));
                OnPositionChanged?.Invoke(_rigidbody2D.position);
            }
        }

        public virtual void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case FlyingObjectObserverMessage.SetValues:
                    HandleSetValues((float)args[0], (Quaternion)args[1], (Vector2)args[2],(bool)args[3]);
                    break;
                case FlyingObjectObserverMessage.HandleCollision:
                    HandleCollision((bool)args[0], (Vector2)args[1], (Vector2)args[2],(bool)args[3]);
                    break;
            }
        }

        public virtual void SetValues(TVS data)
        {
            OnValuesChanged?.Invoke(data);
        }

        protected virtual void HandleCollision(bool canMove, Vector2 position, Vector2 direction, bool doesShowParticles)
        {
            _canMove = canMove;
            if (doesShowParticles)
            {
                GameManager.Instance.EventManager.Publish
                (
                    new ParticleEvents.Spawn
                    {
                        ParticleData = collisionParticle,
                        Position = position,
                        MoveDirection = direction
                    }
                );
            }

            moveSound?.StopSound();
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
            OnCollisionDetected?.Invoke(other);
        }

        public void Recycle()
        {
            moveSound?.StopSound();
            OnRecycle?.Invoke((TS)this);
        }
    }
}