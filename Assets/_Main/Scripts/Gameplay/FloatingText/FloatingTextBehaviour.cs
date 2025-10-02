using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public class FloatingTextBehaviour : ManagedBehavior,IUpdatable,IFloatingText
    {
        [SerializeField] private TextMeshPro meshText;
        [Range(1, 5)] 
        [SerializeField] private float movementSpeed = 2;
        [Range(0.1f,3)]
        [SerializeField] private float fadeTime = 1f;
        [Range(0.1f,3)]
        [SerializeField] private float fadeDelay = 1f;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public event Action<IFloatingText> OnRecycle;
        private bool _canMove;
        private bool _canFade;

        private float _fadeTimer;
        private float _startFadeTimer;
        private float _currentAlpha;
        
        public void ManagedUpdate()
        {
            var dt = CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
            
            if (_canMove)
            {
                HandleMovement(dt);
            }
            
            if (_canFade)
            {
                HandleFade(dt);
            }
        }

        private void HandleMovement(float deltaTime)
        {
            var finalSpeed = deltaTime * movementSpeed;
            transform.position += Vector3.up * finalSpeed ;
        }

        private void HandleFade(float dt)
        {
            _startFadeTimer += dt;
            
            if (_startFadeTimer < fadeDelay) return;

            _fadeTimer += dt;
            float a = _fadeTimer/fadeTime;
            _currentAlpha = Mathf.Lerp(1, 0, a);
            
            var textColor = meshText.color;
            textColor.a = _currentAlpha;
            meshText.color = textColor;

            if (_currentAlpha <= 0)
            {
                Recycle();
            }
        }

        public void SetValues(FloatingTextValues values)
        {
            transform.position = values.Position;
            meshText.text = values.Text;
            meshText.color = values.Color;
            
            _canMove = values.DoesMove;
            _canFade = values.DoesFade;
        }

        private void ResetValues()
        {
            _startFadeTimer = 0;
            _currentAlpha = 1;
            _fadeTimer = 0;
            _canMove = false;
            _canFade = false;
        }

        public void Recycle()
        {
            ResetValues();
            OnRecycle?.Invoke(this);
        }
    }

    public class FloatingTextValues
    {
        public Vector2 Position;
        public Color Color;
        public bool DoesMove;
        public bool DoesFade;
        public string Text;
    }
}