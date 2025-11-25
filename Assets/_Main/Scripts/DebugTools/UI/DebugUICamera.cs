
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUICamera : MonoBehaviour
    {
        [SerializeField] private Button zoomIn;
        [SerializeField] private Button zoomOut;
        
        [Header("Temp")]
        [Range(0,3f)]
        [SerializeField] private float timeToZoom;

        public UnityAction<float> OnZoomIn;
        public UnityAction<float> OnZoomOut;

        private void Awake()
        {
            zoomIn.onClick.AddListener(() => OnZoomIn?.Invoke(timeToZoom));
            zoomOut.onClick.AddListener(() => OnZoomOut?.Invoke(timeToZoom));
        }
    }
}