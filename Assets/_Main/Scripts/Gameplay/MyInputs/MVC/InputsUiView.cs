using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.MyInputs.MVC
{
    public class InputsUiView : ManagedBehavior
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
        
        private Image _currentActiveImage;

        public void InitializeImages(ITouchInputData data)
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
        
        public void SetEnablePanel(bool inputIsEnable)
        {
            imageContainer.SetActive(inputIsEnable);
        }

        public void DestroyContainer()
        {
            Destroy(imageContainer);
        }

        public void SetCurrentImage(int input)
        {
            if (input == -1)
            {
                SetActiveImage(clockwiseImage);
            }
            else if (input == 1)
            {
                SetActiveImage(counterClockwiseImage);
            }
        }
        
        public void DisableActiveImage()
        {
            if (_currentActiveImage != null)
            {
                SetImageColor(_currentActiveImage, defaultColor);
            }
            
            _currentActiveImage = null;
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

        private void SetActiveImage(Image image)
        {
            if (_currentActiveImage != null)
            {
                SetImageColor(_currentActiveImage, defaultColor);
            }
            
            if (image != null)
            {
                _currentActiveImage = image;
                SetImageColor(_currentActiveImage, activeColor);
            }
        }

        public void SetActiveBothImages()
        {
            SetImageColor(clockwiseImage, activeColor);
            SetImageColor(counterClockwiseImage, activeColor);
        }

        public void SetInactiveBothImages()
        {
            SetImageColor(clockwiseImage, defaultColor);
            SetImageColor(counterClockwiseImage, defaultColor);
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