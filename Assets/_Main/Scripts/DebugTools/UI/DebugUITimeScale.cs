using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUITimeScale : MonoBehaviour
    {
        [Header("Increase/Decrease Buttons")]
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;
        [Header("Fixed Buttons")]
        [SerializeField] private Button pauseScaleButton;
        [SerializeField] private Button minScaleButton;
        [SerializeField] private Button halfScaleButton;
        [SerializeField] private Button maxScaleButton;
        [Header("Text")]
        [SerializeField] private TMP_Text scaleText;

        private int _currentScale = 10;
        private event Action<float> OnScaleUpdate;
        public event Action<float> OnChangeScale;
        
        private void Awake()
        {
            increaseButton.onClick.AddListener(() =>
            {
                _currentScale++;
                _currentScale = Mathf.Clamp(_currentScale, 0, 10);
                OnScaleUpdate?.Invoke(_currentScale);
            });
            
            decreaseButton.onClick.AddListener(() =>
            {
                _currentScale--;
                _currentScale = Mathf.Clamp(_currentScale, 0, 10);
                OnScaleUpdate?.Invoke(_currentScale);
            });
            
            pauseScaleButton.onClick.AddListener(() =>
            {
                _currentScale = 0;
                OnScaleUpdate?.Invoke(_currentScale);
            });
            
            minScaleButton.onClick.AddListener(() =>
            {
                _currentScale = 1;
                OnScaleUpdate?.Invoke(_currentScale);
            });
            
            halfScaleButton.onClick.AddListener(() =>
            {
                _currentScale = 5;
                OnScaleUpdate?.Invoke(_currentScale);
            });
            
            maxScaleButton.onClick.AddListener(() =>
            {
                _currentScale = 10;
                OnScaleUpdate?.Invoke(_currentScale);
            });

            OnScaleUpdate += OnScaleUpdateHandler;
        }

        private void Start()
        {
            _currentScale = 10;
            scaleText.text = _currentScale.ToString();
        }

        private void OnScaleUpdateHandler(float scale)
        {
            _currentScale = (int)scale;
            
            if (scale > 0)
            {
                scale /= 10;
            }
            
            scaleText.text = _currentScale.ToString();
            
            OnChangeScale?.Invoke(scale);
        }
    }
}