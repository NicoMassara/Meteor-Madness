using System;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class Meteor : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private SoundBehavior moveSound;
        [Header("Layers Mask")]
        [SerializeField] private LayerMask shieldMask;
        [SerializeField] private LayerMask earthMask;
        private Rigidbody2D _rb;
        private CircleCollider2D _circleCollider;
        private float _movementSpeed;
        private bool _isPaused;
        private bool _canMove;
        private bool _isDestroyed;

        private readonly Timer _recycleTimer = new Timer();
        //Reference to itself, hasHitShield
        public UnityAction<Meteor, bool> OnHit;
        public UnityAction<Meteor> OnRecycle;
        public UnityAction OnValuesSet;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        private void Start()
        {
            GameManager.Instance.OnPaused += GM_OnPausedHandler;
            _recycleTimer.OnEnd += Timer_OnEndHandler;
        }

        private void Update()
        {
            if (_isDestroyed)
            {
                _recycleTimer.Run();
            }
        }

        private void FixedUpdate()
        {
            if (_isPaused == false)
            {
                if (_canMove)
                {
                    _rb.transform.Translate(Vector2.right * (_movementSpeed * Time.deltaTime));
                }
            }
        }

        public void SetValues(float movementSpeed, Quaternion rotation, Vector2 position)
        {
            _movementSpeed = movementSpeed;
            _rb.transform.rotation = rotation;
            _rb.transform.position = position;
            _canMove = true;
            _isDestroyed = false;
            moveSound.PlaySound(1);
            OnValuesSet?.Invoke();
        }
        
        private void GM_OnPausedHandler(bool isPaused)
        {
            _isPaused = isPaused;
        }
        
        private void Timer_OnEndHandler()
        {
            _circleCollider.isTrigger = false;
            _recycleTimer.Reset();
            OnRecycle?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & shieldMask) != 0)
            {
                if (_isDestroyed == false)
                {
                    OnHit?.Invoke(this, true);
                    _canMove = false;
                    _circleCollider.isTrigger = true;
                    _isDestroyed = true;
                    _recycleTimer.Set(GameValues.MeteorRecycleTime);
                    moveSound.StopSound();
                }
            }
            else if (((1 << other.gameObject.layer) & earthMask) != 0)
            {
                OnHit?.Invoke(this, false);
                _canMove = false;
                moveSound.StopSound();
                OnRecycle?.Invoke(this);
            }
        }

        public void ForceRecycle()
        {
            _canMove = false;
            moveSound.StopSound();
            OnRecycle?.Invoke(this);
        }
    }
}