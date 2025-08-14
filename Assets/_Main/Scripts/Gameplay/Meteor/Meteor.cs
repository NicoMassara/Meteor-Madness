using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class Meteor : MonoBehaviour
    {
        [SerializeField] private LayerMask shieldMask;
        [SerializeField] private LayerMask earthMask;
        private float _movementSpeed;
        private Rigidbody2D _rb;
        //Reference to itself, hasHitShield
        public UnityAction<Meteor, bool> OnHit;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.transform.Translate(Vector2.right * (_movementSpeed * Time.deltaTime));
        }

        public void SetValues(float movementSpeed, Quaternion rotation, Vector2 position)
        {
            _movementSpeed = movementSpeed;
            _rb.transform.rotation = rotation;
            _rb.transform.position = position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & shieldMask) != 0)
            {
                OnHit?.Invoke(this, true);
            }
            else if (((1 << other.gameObject.layer) & earthMask) != 0)
            {
                OnHit?.Invoke(this, false);
            }
        }
    }
}