using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class RotateUiButtons : ManagedBehavior
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject imageContainer;
        [SerializeField] private RectTransform clockwiseImage;
        [SerializeField] private RectTransform counterClockwiseImage;
        [SerializeField] private Canvas canvas;

        private void Start()
        {
            var data = GameConfigManager.Instance.GetGameplayData().TouchInputData;
            
            if(data == null) return;

            if (clockwiseImage == null)
            {
                Debug.Log("clockwiseImage is null");
                return;
            }
            
            var clockWiseCenter = GetCenterBetweenPoints(data.GetRightZoneBounds().y, data.GetRightZoneBounds().x);
            var counterClockCenter = GetCenterBetweenPoints(data.GetLeftZoneBounds().y, data.GetLeftZoneBounds().x);
            
            SetPositionInCanvas(clockwiseImage, new Vector2(clockWiseCenter, data.GetBottomBound()));
            SetPositionInCanvas(counterClockwiseImage, new Vector2(counterClockCenter, data.GetBottomBound()));
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

        public void SetEnablePanel(bool isEnable)
        {
            imageContainer.gameObject.SetActive(isEnable);
        }
    }
}