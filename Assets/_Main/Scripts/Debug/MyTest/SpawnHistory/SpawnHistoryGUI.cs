using _Main.Scripts.Gameplay.Projectile;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.SpawnHistory
{
    public class SpawnHistoryGUI : GUIDebug
    {
        [System.Serializable]
        private class ScreenPosition
        {
            [Range(0,1)]
            public float widthPos;
            [Range(0,1)]
            public float heightPos;
            
            public Vector2 ScreenPos =>  new Vector2(Screen.width * widthPos, Screen.height * heightPos);
        }

        [Space(5)]
        [SerializeField] private ScreenPosition guiPosition;
        
        private HistoryDebugData _historyData = new HistoryDebugData();
        
        private void Awake()
        {
            ProjectileDebugEvents.OnHistoryChanged += data =>
            {
                _historyData = data;
            };
        }
        
        protected override string[] GetLines()
        {
            var length = _historyData.Amount;
            var strings = new string[length];

            for (int i = 0; i < length; i++)
            {
                var temp1 = _historyData.Weights;

                if (temp1 != null)
                {
                    var weights = _historyData.Weights[i].Values;
                    
                    if (weights != null)
                        strings[i] = $"{i + 1} - {_historyData.History[i]}, " +
                                     $"R:{weights[0]} " +
                                     $"A:{weights[1]} " +
                                     $"D:{weights[2]} " +
                                     $"S:{weights[3]}";
                    else
                    {
                        strings[i] = $"{i + 1} - {_historyData.History[i]}";
                    }
                    
                    
                }
                else
                {
                    strings[i] = $"{i + 1} - {_historyData.History[i]}";
                }
                

            }
            
            return strings;
        }

        protected override Color[] GetLineColors()
        {
            var length = 10;
            var colors = new Color[length];

            for (int i = 0; i < length; i++)
            {
                colors[i] = Color.white;
            }
            
            return colors;
        }

        protected override bool IsDebugEnable() => true;
        protected override Vector2 GetScreenPos() => guiPosition.ScreenPos;

        protected override string GetTitle()
        {
            return "-- Spawn Type History --";
        }
    }
}