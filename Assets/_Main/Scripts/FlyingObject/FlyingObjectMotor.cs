using System;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Comet
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class FlyingObjectMotor : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] protected SoundBehavior moveSound;
        private Rigidbody2D _rb;
        private float _movementSpeed;
        private bool _isPaused;
        protected bool CanMove;
        public UnityAction OnValuesSet;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.simulated = true;
            _rb.mass = 0.0001f;
            _rb.drag = 0f;
            _rb.angularDrag = 0.05f;
            _rb.gravityScale = 0f;
        }
        
        private void Start()
        {
            GameManager.Instance.OnPaused += GM_OnPausedHandler;
        }
        
        private void FixedUpdate()
        {
            if (_isPaused == false)
            {
                if (CanMove)
                {
                    _rb.transform.Translate(Vector2.right * (_movementSpeed * Time.deltaTime));
                }
            }
        }
        
        public virtual void SetValues(float movementSpeed, Quaternion rotation, Vector2 position)
        {
            _movementSpeed = movementSpeed;
            _rb.transform.rotation = rotation;
            _rb.transform.position = position;
            CanMove = true;
            moveSound?.PlaySound(1);
            OnValuesSet?.Invoke();
        }

        protected abstract void HandleCollision(Collider2D other);

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other);
        }

        private void GM_OnPausedHandler(bool isPaused)
        {
            this._isPaused = isPaused;
        }
    }
}