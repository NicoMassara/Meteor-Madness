using System;
using MeteorMadness.Contracts;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.UI.FloatingText
{
    public class FloatingTextBehaviour : ManagedBehavior,IUpdatable,IFloatingText
    {
        //[SerializeField] private TextMeshPro meshText;
        [Range(1, 5)] 
        [SerializeField] private float movementSpeed = 2;
        [Range(0.1f,3)]
        [SerializeField] private float fadeTime = 1f;
        [Range(0.1f,3)]
        [SerializeField] private float fadeDelay = 1f;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;
        public event Action<IFloatingText> OnRecycle;
        private bool _canMove;
        private bool _canFade;

        private float _fadeTimer;
        private float _startFadeTimer;
        private float _currentAlpha;
        private IFloatingText floatingTextImplementation;

        public void ExecuteUpdate(float deltaTime)
        {
            
            if (_canMove)
            {
                HandleMovement(deltaTime);
            }
            
            if (_canFade)
            {
                HandleFade(deltaTime);
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
            
            /*var textColor = meshText.color;
            textColor.a = _currentAlpha;
            meshText.color = textColor;*/

            if (_currentAlpha <= 0)
            {
                Recycle();
            }
        }
        
        public void SetValues(FloatingTextValues values)
        {
            transform.position = values.Position + values.Offset;
            /*meshText.text = values.Text;
            meshText.color = values.Color;*/
            
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
}