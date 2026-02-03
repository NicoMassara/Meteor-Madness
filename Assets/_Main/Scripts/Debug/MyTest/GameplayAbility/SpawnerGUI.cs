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

        private BatchDebugData _batchData = new BatchDebugData();
        private float _batchTime;

        private void Awake()
        {
            ProjectileDebugEvents.OnBatchCreated += data =>
            {
                _batchTime = Time.realtimeSinceStartup;
                _batchData = data;
            };
        }

        protected override string[] GetLines()
        {
            return new string[]
            {
                $"Time: {_batchTime:F2}",
                $"Level: {_batchData.Level+1}",
                $"Amount: {_batchData.Amount}",
                $"Start Slot: {_batchData.StartSlot}",
                $"Offset: {_batchData.Offset}",
                $"Inner Dist: {_batchData.InnerDist:F2}",
                $"Next Dist: {_batchData.NextDist:F2}",
                $"Last Slot: {_batchData.LastSlot}", 
                $"Type: {_batchData.SpawnType}"
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