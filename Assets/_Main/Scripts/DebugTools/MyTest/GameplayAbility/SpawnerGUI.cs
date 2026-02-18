using System;
using _Main.Scripts.Gameplay.Projectile;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.GameplayAbility
{
    public class SpawnerGUI : GUIDebug
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

        private BatchDebugData _defaultBatchData = new BatchDebugData();
        private float _batchTime;

        private void Awake()
        {
            ProjectileDebugEvents.OnBatchCreated += data =>
            {
                _batchTime = Time.realtimeSinceStartup;
                _defaultBatchData = data;
            };
        }

        protected override string[] GetLines()
        {
            return new string[]
            {
                $"Time: {_batchTime:F2}",
                $"Level: {_defaultBatchData.Level+1}",
                $"Amount: {_defaultBatchData.Amount}",
                $"Start Slot: {_defaultBatchData.StartSlot}",
                $"Offset: {_defaultBatchData.Offset}",
                $"Inner Dist: {_defaultBatchData.InnerDist:F2}",
                $"Next Dist: {_defaultBatchData.NextDist:F2}",
                $"Last Slot: {_defaultBatchData.LastSlot}", 
                $"Type: {_defaultBatchData.SpawnType}"
            };
        }

        protected override Color[] GetLineColors()
        {
            return new Color[]
            {
                Color.white,
                Color.white,
                Color.white,
                Color.white,
                Color.white,
                Color.white,
                Color.white, 
                Color.white,
                Color.white,
            };
        }

        protected override bool IsDebugEnable() => true;
        protected override Vector2 GetScreenPos() => guiPosition.ScreenPos;
        protected override string GetTitle()
        {
            return "-- Batch Created --";
        }
    }
}