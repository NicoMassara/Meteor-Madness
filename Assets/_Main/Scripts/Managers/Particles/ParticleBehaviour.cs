using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Particles
{
    public class ParticleBehaviour : ManagedBehavior, IUpdatable
    {
        [SerializeField] private SpriteRenderer sprite;
        private ParticleDataSo _data;
        private float _alpha;
        private float _scaleTimer;
        private float _fadeTimer;
        private Vector3 _moveDirection = Vector3.up;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
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


        public void ManagedUpdate()
        {
            var dt = CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
            
            _scaleTimer += dt;
            float t = _scaleTimer/_data.TimeToReachScale;
            transform.localScale = Vector3.Lerp(_data.StartScale, _data.TargetScale, t);
            transform.position += (dt * _data.MoveSpeed) * _moveDirection;
            
            var ratio = transform.localScale.x / _data.TargetScale.x;
            
            if (ratio >= _data.RatioTimeToStartFade)
            {
                _fadeTimer += dt;
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