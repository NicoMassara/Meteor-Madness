using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Particles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParticleBehaviour : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private float _timeToFade;
        private float _fadeSpeed;
        private float _fadeScale;
        private float _alpha;
        
        public UnityAction<ParticleBehaviour> OnRecycle;
        
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void SetValues(Sprite sprite, float timeToFade,float fadeSpeed, float fadeScale, Vector3 position, float rotation)
        {
            _sprite.sprite = sprite;
            _timeToFade = timeToFade;
            _fadeSpeed = fadeSpeed;
            _fadeScale = fadeScale;
            transform.position = position;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            _alpha = 1;
            _sprite.color = new Color(1, 1, 1, 1);
        }

        private void Update()
        {
            _timeToFade -= Time.deltaTime;
            
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _fadeScale, _fadeSpeed * Time.deltaTime);

            if (_timeToFade <= 0)
            {
                _alpha = Mathf.Lerp(_alpha, -0.25f, _fadeSpeed * Time.deltaTime);
                _sprite.color = new Color(1, 1, 1, _alpha);

                if (_alpha <= 0)
                {
                    ForceRecycle();
                }
            }
        }

        public void ForceRecycle()
        {
            OnRecycle?.Invoke(this);
        }
    }
}