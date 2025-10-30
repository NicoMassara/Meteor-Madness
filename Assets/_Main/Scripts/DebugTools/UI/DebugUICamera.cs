using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUICamera : MonoBehaviour
    {
        [SerializeField] private Button zoomIn;
        [SerializeField] private Button zoomOut;

        public UnityAction OnZoomIn;
        public UnityAction OnZoomOut;

        private void Awake()
        {
            zoomIn.onClick.AddListener(() => OnZoomIn?.Invoke());
            zoomOut.onClick.AddListener(() => OnZoomOut?.Invoke());
        }
    }
}