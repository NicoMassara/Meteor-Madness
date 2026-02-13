
using MeteorMadness.Debug._Main.Scripts.Debug._Main.Scripts.Contracts;
using UnityEngine;

namespace _Main.Scripts.DebugTools
{
    public abstract class ComponentGuiDebug : MonoBehaviour
    {
        protected Camera MainCamera { get; private set; }
        protected abstract int DebugLines { get; }
        private bool _hasStarted;
        protected abstract string  DebugName { get; }

        #region GUI Config
        
        [System.Serializable]
        private class DebugGUIConfig
        {
            public Vector2 PositionOffset = new Vector2(0, 150);
            [Min(1)]
            public float Width = 320f;
            [Min(1)]
            public float LineHeight = 26f;
            [Min(1)]
            public int TitleSize = 30;
            [Min(1)]
            public int LineSize = 24;
        }

        [SerializeField] private DebugGUIConfig guiConfig;
        
        private GUIStyle _titleStyle;
        private GUIStyle _lineStyle;
        private Texture2D _whiteTex;
        
        #endregion

        #region Debug Data

        protected DebugGUIData[] DebugDataArray { get; private set; }
        
        protected class DebugGUIData
        {
            public string DebugText { get; private set;}
            public Color DebugColor { get; private set;}

            public bool DoesShow { get; private set; } = true;

            public DebugGUIData SetText(string text)
            {
                DebugText = text;
                return this;
            }

            public DebugGUIData SetColor(Color color)
            {
                DebugColor = color;
                return this;
            }

            public DebugGUIData SetShowCondition(bool condition)
            {
                DoesShow = condition;
                return this;
            }
        }
        
        #endregion

        protected virtual void Start()
        {
            MainCamera = Camera.main;
            _hasStarted = true;
            
            InitializeDebugData(DebugLines);
        }
        
        private void InitializeDebugData(int debugLines)
        {
            DebugDataArray = new DebugGUIData[debugLines];

            for (int i = 0; i < debugLines; i++)
            {
                DebugDataArray[i] = new DebugGUIData();
            }
        }
        
        #region Protected Methods

        protected abstract DebugGUIData[] GetDebugData();
        protected abstract bool GetCanShowGUI();
        protected abstract Vector3 GetGUIPosition();
        protected virtual void DrawDebugText()
        {
            if (MainCamera == null) return;
            if (DebugDataArray == null) return;

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = guiConfig.TitleSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            else
            {
                _titleStyle.fontSize = guiConfig.TitleSize;
            }
            
            if (_lineStyle == null)
            {
                _lineStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = guiConfig.LineSize,
                    alignment = TextAnchor.MiddleLeft,
                };
            }
            else
            {
                _lineStyle.fontSize = guiConfig.LineSize;
            }
            
            if (_whiteTex == null)
            {
                _whiteTex = new Texture2D(1, 1);
                _whiteTex.SetPixel(0, 0, Color.white);
                _whiteTex.Apply();
            }
            

            Vector3 screenPos = GetGUIPosition() + (Vector3)guiConfig.PositionOffset;


            var debugData = GetDebugData();
            var debugLength = debugData.Length;
            var canShowCount = 0;

            for (int i = 0; i < debugLength; i++)
            {
                var debugItem = DebugDataArray[i];
                if (debugItem.DoesShow)
                {
                    canShowCount++;
                }
            }

            const float padding = 20;
            
            float height =
                padding * 2 +
                guiConfig.LineHeight * (canShowCount + 1); // +1 por el título

            float x = screenPos.x - guiConfig.Width * 0.5f;
            float y = Screen.height - screenPos.y - height - 10f;

            // Fondo
            GUI.Box(new Rect(x, y, guiConfig.Width, height), "");

            float cursorY = y + padding;

            // Título
            GUI.Label(
                new Rect(x + padding, cursorY, guiConfig.Width - padding * 2, guiConfig.LineHeight),
                DebugName,
                _titleStyle
            );

            const float titlePadding = 20;
            cursorY += titlePadding;
            
            _lineStyle.normal.textColor = Color.white;
            _lineStyle.alignment = TextAnchor.MiddleCenter;
            
            const float separatorHeight = 2f;
            const float separatorMargin = 6f;
            cursorY += separatorMargin * 2 + separatorHeight;
            
            GUI.DrawTexture(
                new Rect(
                    x + padding,
                    cursorY + separatorMargin,
                    guiConfig.Width - padding * 2,
                    separatorHeight
                ),
                _whiteTex
            );
            
            cursorY += separatorMargin * 2 + separatorHeight;

            // Contenido
            
            _lineStyle.alignment = TextAnchor.MiddleLeft;

            for (int i = 0; i < debugLength; i++)
            {
                var debugItem = DebugDataArray[i];
                
                if(debugItem.DoesShow == false) continue;
                
                _lineStyle.normal.textColor = debugItem.DebugColor;

                GUI.Label(
                    new Rect(x + padding, cursorY, guiConfig.Width - padding * 2, guiConfig.LineHeight),
                    debugItem.DebugText,
                    _lineStyle
                );
                
                cursorY += guiConfig.LineHeight;
            }
        }
        protected abstract bool GetIsDebugEnable();

        #endregion

        protected virtual void OnGUI()
        {
            if(_hasStarted == false) return;
            if (GetIsDebugEnable() == false) return;
            if (GetCanShowGUI() == false) return;
            
            DrawDebugText();
        }
    }
}

