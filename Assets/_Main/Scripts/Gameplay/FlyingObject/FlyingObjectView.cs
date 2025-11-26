using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Sounds;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.FlyingObject
{
    public class FlyingObjectMovement : ManagedComponent, IFixedUpdatable
    {
        private Rigidbody2D _rigidbody;
        private event Action<Vector2> _onPositionChanged;
        public float MovementSpeed { get; set; }
        public bool CanMove { get; set; }
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        public void InitializeValues(Rigidbody2D rigidbody, Action<Vector2> onPositionChanged)
        {
            Initialize();
            _rigidbody = rigidbody;
            _onPositionChanged += onPositionChanged;
        }
        
        public void ExecuteFixedUpdate(float fixedDeltaTime)
        {
            if (CanMove)
            {
                _rigidbody.transform.Translate(Vector2.right * (MovementSpeed * fixedDeltaTime));
                _onPositionChanged?.Invoke(_rigidbody.position);
            }
        }
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingObjectView<T, TS, TVS> : ManagedBehavior, IProjectileSounds, 
        IObserver, IUpdatable, IPoolable<TS>, IFlyingObjectSkin
    where T : FlyingObjectMotor<TVS>
    where TS : FlyingObjectView<T, TS, TVS>
    where TVS : FlyingObjectValues
    {
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
        protected FlyingObjectMovement Movement { get; private set; }
        
        public event Action<Vector2> OnPositionChanged;
        public event Action<TVS> OnValuesChanged;
        public event Action<Collider2D> OnCollisionDetected;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        public event Action<TS> OnRecycle;
        
        public event Action OnStart;
        public event Action OnStop;
        public event Action OnSkinEnable;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2D.simulated = true;
            _rigidbody2D.mass = 0.0001f;
            _rigidbody2D.drag = 0f;
            _rigidbody2D.angularDrag = 0.05f;
            _rigidbody2D.gravityScale = 0f;
            
            Movement = new FlyingObjectMovement();
        }

        private void Start()
        {
            Movement.InitializeValues(_rigidbody2D,OnPositionChanged);
            _hasFire = fireObject != null;

            if (_hasFire)
            {
                _fireScaleOscillator = new Oscillator(50, 0.12f, 1);
                _fireRotationOscillator = new Oscillator(10, 1, 90);
            }

            _sphereRotator = new Rotator(sphereObject.transform,Vector3.forward, maxRotationSpeed);
            _sphereRotator.SetSpeed(GetRotationSpeed());
            
        }
        
        public virtual void ExecuteUpdate(float deltaTime)
        {
            _sphereRotator?.Rotate(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
            
            if (_hasFire)
            {
                fireObject.transform.localScale = new Vector3(_fireScaleOscillator.OscillateSin(), 1, 1);
                fireObject.transform.localRotation = Quaternion.Euler(0,0, _fireRotationOscillator.OscillateCos());
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
            OnStart?.Invoke();
            OnSkinEnable?.Invoke();
        }

        protected virtual void HandleCollision(bool canMove, Vector2 position, Vector2 direction, bool doesShowParticles)
        {
            Movement.CanMove = canMove;
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
        }

        private void HandleSetValues(float movementSpeed, Quaternion rotation, Vector2 position, bool canMove)
        {
            Movement.MovementSpeed = movementSpeed;
            _rigidbody2D.transform.rotation = rotation;
            _rigidbody2D.transform.position = position;
            Movement.CanMove = canMove;
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
            OnRecycle?.Invoke((TS)this);
            OnStop?.Invoke();
        }
    }
}