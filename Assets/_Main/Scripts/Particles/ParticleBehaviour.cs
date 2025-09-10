using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Particles
{
    public class ParticleBehaviour : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        private ParticleDataSo _data;
        private float _alpha;
        private float _scaleTimer;
        private float _fadeTimer;
        private Vector3 _moveDirection = Vector3.up;
        
        public UnityAction<ParticleBehaviour> OnRecycle;

        public void SetValues(ParticleDataSo particleData, Vector3 position, float rotation, Vector3 moveDirection)
        {
            _data = particleData;
            sprite.sprite = _data.Sprite;
            transform.position = position + _data.PositionOffset;
            transform.rotation = Quaternion.Euler(0, 0, rotation + _data.RotationOffset);
            _alpha = 1;
            this.sprite.color = new Color(1, 1, 1, 1);
            transform.localScale = _data.StartScale;
            _moveDirection = moveDirection;
            _scaleTimer = 0;
            _fadeTimer = 0;
        }

        private void Update()
        {
            _scaleTimer += Time.deltaTime;
            float t = _scaleTimer/_data.TimeToReachScale;
            transform.localScale = Vector3.Lerp(_data.StartScale, _data.TargetScale, t);
            //transform.position += (Time.deltaTime * _data.MoveSpeed) * -_moveDirection;
            
            var ratio = transform.localScale.x / _data.TargetScale.x;
            
            if (ratio >= _data.RatioTimeToStartFade)
            {
                _fadeTimer += Time.deltaTime;
                float a = _fadeTimer/_data.TimeToFade;
                _alpha = Mathf.Lerp(1, 0, a);
                sprite.color = new Color(1, 1, 1, _alpha);

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