
using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.MyInputs.MVC
{
    public class InputsUiView : ManagedBehavior, IObserver
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject imageContainer;
        [SerializeField] private Image clockwiseImage;
        [SerializeField] private Image counterClockwiseImage;
        [SerializeField] private Canvas canvas;
        [Header("Colors")] 
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color activeColor = Color.gray;
        [SerializeField] private float transparency = 0.5f;

        private void Awake()
        {
#if !UNITY_ANDROID || !UNITY_IOS
            
            HandleSetEnableUI(false);
            
            SetEnableImage(clockwiseImage, false);
            SetEnableImage(counterClockwiseImage, false);
#endif
        }

        public void OnNotify(ulong message, params object[] args)
        {
#if UNITY_ANDROID
            

            switch (message)
            {
                case InputsUIObserverMessage.SetEnableClock:
                    HandleEnableClock((bool)args[0]);
                    break;
                case InputsUIObserverMessage.SetEnableCounterClock:
                    HandleEnableCounterClock((bool)args[0]);
                    break;
                case InputsUIObserverMessage.SetEnableUI:
                    HandleSetEnableUI((bool)args[0]);
                    break;
                case InputsUIObserverMessage.Initialize:
                    HandleInitialize((ITouchInputData)args[0]);
                    break;
                case InputsUIObserverMessage.Destroy:
                    HandleDestroy();
                    break;
            }
#endif
        }
#if UNITY_ANDROID

        #region Observer Handlers
        
        private void HandleEnableClock(bool isActive)
        {
            SetEnableImage(clockwiseImage, isActive);
        }

        private void HandleEnableCounterClock(bool isActive)
        {
            SetEnableImage(counterClockwiseImage, isActive);
        }
        
        private void HandleSetEnableUI(bool isEnable)
        {
            imageContainer.SetActive(isEnable);
        }
        private void HandleInitialize(ITouchInputData touchInputData)
        {
            InitializeImages(touchInputData);
        }
        
        private void HandleDestroy()
        {
            Destroy(imageContainer);
        }

        #endregion

        private void InitializeImages(ITouchInputData data)
        {
            if(data == null) return;

            if (clockwiseImage == null)
            {
                Debug.Log("clockwiseImage is null");
                return;
            }
            
            var clockWiseCenter = GetCenterBetweenPoints(data.GetRightZoneBounds().y, data.GetRightZoneBounds().x);
            var counterClockCenter = GetCenterBetweenPoints(data.GetLeftZoneBounds().y, data.GetLeftZoneBounds().x);
            
            SetPositionInCanvas(clockwiseImage.GetComponent<RectTransform>(), new Vector2(clockWiseCenter, data.GetBottomBound()));
            SetPositionInCanvas(counterClockwiseImage.GetComponent<RectTransform>(), new Vector2(counterClockCenter, data.GetBottomBound()));
        }

        private void SetPositionInCanvas(RectTransform uiElement, Vector2 screenPosition)
        {
            Vector2 localPoint;

            // Convertimos la screen position a posición local dentro del canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvas.worldCamera, // null si es Screen Space - Overlay
                out localPoint
            );
            
            // Aplicamos la posición al UI element
            localPoint = new Vector2(localPoint.x, localPoint.y + (uiElement.rect.height/2));
            uiElement.anchoredPosition = localPoint;
        }
        
        private float GetCenterBetweenPoints(float pointA, float pointB)
        {
            return (pointA + pointB) * 0.5f;
        }
#endif

        private void SetEnableImage(Image image, bool isActive)
        {
            if (image == null) return;
            
            var targetColor = isActive ? activeColor : defaultColor;
            SetImageColor(image, targetColor);
        }
        
        private void SetImageColor(Image image, Color color)
        {
            image.color = new Color(color.r, color.g, color.b, GetTransparency());
        }

        private float GetTransparency()
        {
            return Mathf.Lerp(0,255, transparency);
        }
    }
}