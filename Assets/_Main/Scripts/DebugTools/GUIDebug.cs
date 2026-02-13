using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug
{
    public abstract class GUIDebug : MonoBehaviour
    {
        private Camera _mainCamera;

        [System.Serializable]
        private class GuiConfig
        {
            [Min(1)]
            public float Width = 320f;
            [Min(1)]
            public float LineHeight = 26f;
            [Min(1)]
            public float Padding = 12f;
            [Min(1)]
            public int TitleSize = 30;
            [Min(1)]
            public int LineSize = 24;
        }

        [SerializeField] private GuiConfig guiConfig;
        
        private GUIStyle _titleStyle;
        private GUIStyle _lineStyle;
        private bool _hasStarted;
        
        
        private void Start()
        {
            _mainCamera = Camera.main;
            _hasStarted = true;
        }
        
        protected abstract string[] GetLines();
        protected abstract Color[] GetLineColors();
        protected abstract bool IsDebugEnable();
        protected abstract Vector2 GetScreenPos();
        protected abstract string GetTitle();
        
        private void OnGUI()
        {
            if(_hasStarted == false) return; 
            if (_mainCamera == null) return;
            if (IsDebugEnable() == false) return;

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = guiConfig.TitleSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            
            if (_lineStyle == null)
            {
                _lineStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = guiConfig.LineSize,
                    alignment = TextAnchor.MiddleLeft,
                };
            }
            

            Vector3 screenPos = GetScreenPos();

            var lines = GetLines();
            var linesLength = lines.Length;
            
            float height =
                guiConfig.Padding * 2 +
                guiConfig.LineHeight * (linesLength + 1); // +1 por el título

            float x = screenPos.x - guiConfig.Width * 0.5f;
            float y = Screen.height - screenPos.y - height - 10f;

            // Fondo
            GUI.Box(new Rect(x, y, guiConfig.Width, height), "");

            float cursorY = y + guiConfig.Padding;

            // Título
            GUI.Label(
                new Rect(x + guiConfig.Padding, cursorY, guiConfig.Width - guiConfig.Padding * 2, guiConfig.LineHeight),
                GetTitle(),
                _titleStyle
            );

            cursorY += guiConfig.LineHeight;

            // Contenido

            for (int i = 0; i < linesLength; i++)
            {
                var line = lines[i];
                
                var color = GetLineColors()[i];
                
                _lineStyle.normal.textColor = color;

                GUI.Label(
                    new Rect(x + guiConfig.Padding, cursorY, guiConfig.Width - guiConfig.Padding * 2, guiConfig.LineHeight),
                    line,
                    _lineStyle
                );
                
                cursorY += guiConfig.LineHeight;
            }
        }
        
    }
}