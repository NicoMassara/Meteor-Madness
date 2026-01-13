using System;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet.Test
{
    public class CometDebug : MonoBehaviour
    {
        private IDebugComet _debugComet;
        private Camera _mainCamera;
        
        // Config
        private const float Width = 320f;
        private const float LineHeight = 26f;
        private const float Padding = 12f;
        private const int TitleSize = 30;
        private const int LineSize = 24;
        
        private GUIStyle _titleStyle;
        private GUIStyle _lineStyle;


        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugComet = GetComponent<IDebugComet>();
#else
            enabled = false;
#endif
        }
        private void Start()
        {
            _mainCamera = Camera.main;
        }
        
        private void OnGUI()
        {
            if (_mainCamera == null) return;
            if (_debugComet.DebugEnable == false) return;

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = TitleSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            
            if (_lineStyle == null)
            {
                _lineStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = LineSize,
                    alignment = TextAnchor.MiddleLeft
                };
            }
            

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position);
            string[] lines =
            {
                $"Pos: {_debugComet.Position}",
                $"Speed: {_debugComet.Speed}",
                $"Ratio: {_debugComet.TravelRatio}",
                $"Dist: {_debugComet.Distance}",
                $"Scale: {_debugComet.Scale}",
            };

            float height =
                Padding * 2 +
                LineHeight * (lines.Length + 1); // +1 por el título

            float x = screenPos.x - Width * 0.5f;
            float y = Screen.height - screenPos.y - height - 10f;

            // Fondo
            GUI.Box(new Rect(x, y, Width, height), "");

            float cursorY = y + Padding;

            // Título
            GUI.Label(
                new Rect(x + Padding, cursorY, Width - Padding * 2, LineHeight),
                gameObject.name,
                _titleStyle
            );

            cursorY += LineHeight;

            // Contenido
            foreach (var line in lines)
            {
                GUI.Label(
                    new Rect(x + Padding, cursorY, Width - Padding * 2, LineHeight),
                    line,
                    _lineStyle
                );

                cursorY += LineHeight;
            }
        }
    }
}