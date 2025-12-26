using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics.Components
{
    public class ButtonsScrollRect : MonoBehaviour
    {
        [Range(0.01f, 1f)]
        [SerializeField] private float scrollStep = 0.05f;
        private ScrollRect _scrollRect;

        private const float CanScrollDelay = 0.5f;
        private float _enableTime;
        private float _lastValue;
        private float _accumulated;

        public event Action<int> OnScrolled;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                Debug.LogError("Scroll Rect is null!");
            }
        }

        private void OnEnable()
        {
            _enableTime = Time.realtimeSinceStartup;
            _scrollRect.onValueChanged.AddListener(OnValueChangedHandler);
        }

        private void OnDisable()
        {
            _enableTime = 0;
            _scrollRect.onValueChanged.RemoveListener(OnValueChangedHandler);
        }

        private void OnValueChangedHandler(Vector2 input)
        {
            if(GetIsScrollDisabled()) return;
            
            _accumulated += Mathf.Abs(input.y - _lastValue);
            _lastValue = input.y;
            bool doesScroll = _accumulated >= scrollStep;
            
            if (doesScroll)
            {
                var signedValue = Math.Sign(input.y);
                OnScrolled?.Invoke(signedValue);
                _accumulated = 0;
            }
        }

        private bool GetIsScrollDisabled()
        {
            return ((Time.realtimeSinceStartup - _enableTime) > CanScrollDelay) == false;
        }
    }
}