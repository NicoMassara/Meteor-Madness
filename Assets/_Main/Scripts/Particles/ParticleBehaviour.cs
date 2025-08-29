using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Particles
{
    public class ParticleBehaviour : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        private float _timeToFade;
        private float _fadeSpeed;
        private float _fadeScale;
        private float _alpha;
        private Vector3 _moveDirection = Vector3.up;
        
        public UnityAction<ParticleBehaviour> OnRecycle;

        public void SetValues(Sprite spriteS, float timeToFade,float fadeSpeed, float fadeScale,
            Vector3 position, float rotation, float startScale, Vector3 moveDirection)
        {
            this.sprite.sprite = spriteS;
            _timeToFade = timeToFade;
            _fadeSpeed = fadeSpeed;
            _fadeScale = fadeScale;
            transform.position = position;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            _alpha = 1;
            this.sprite.color = new Color(1, 1, 1, 1);
            transform.localScale = Vector3.one * startScale;
            _moveDirection = moveDirection;
        }

        private void Update()
        {
            _timeToFade -= Time.deltaTime;
            
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _fadeScale, _fadeSpeed * Time.deltaTime);

            transform.position += (Time.deltaTime * (_fadeSpeed/4)) * -_moveDirection;
            
            if (transform.localScale.x >= 1)
            {
                if (_timeToFade <= 0)
                {
                    _alpha = Mathf.Lerp(_alpha, -0.25f, _fadeSpeed * Time.deltaTime);
                    sprite.color = new Color(1, 1, 1, _alpha);

                    if (_alpha <= 0)
                    {
                        ForceRecycle();
                    }
                }
            }
        }

        public void ForceRecycle()
        {
            OnRecycle?.Invoke(this);
        }
    }
}