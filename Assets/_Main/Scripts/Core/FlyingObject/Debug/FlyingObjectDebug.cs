using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace _Main.Scripts.Core.FlyingObject.Debug
{
    public abstract class FlyingObjectDebug<T> : MonoBehaviour
        where T : IDebugFlyingObject
    {
        private Camera _mainCamera;
        protected T DebugObject { get; private set; }

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
            DebugObject = GetComponent<T>();
#else
            enabled = false;
#endif
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }
        
        protected abstract string[] GetLines();
        protected abstract Color[] GetLineColors();
        
        private void OnGUI()
        {
            if (_mainCamera == null) return;
            if (DebugObject.DebugEnable == false) return;

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
                    alignment = TextAnchor.MiddleLeft,
                };
            }
            

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position);


            var lines = GetLines();
            var linesLength = lines.Length;
            
            float height =
                Padding * 2 +
                LineHeight * (linesLength + 1); // +1 por el título

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

            for (int i = 0; i < linesLength; i++)
            {
                var line = lines[i];
                
                var color = GetLineColors()[i];
                
                _lineStyle.normal.textColor = color;

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