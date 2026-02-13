using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Gameplay.Projectile.SO;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Test
{
    public class SlotSelectorTester : MonoBehaviour
    {
        [SerializeField] private ProjectileSpawnDataSo spawnData;
        private SpawnTypeSelector _spawnSelector;
        private int _currentLevel;

        private void Awake()
        {
            RandomService.Initialize();
        }

        private void Start()
        {
            _spawnSelector = new SpawnTypeSelector();
        }

        public void IncreaseLevel()
        {
            _currentLevel++;
            _currentLevel = Math.Clamp(_currentLevel,0, GameParameters.GameplayValues.SpawnLevelAmount);
        }

        public void DecreaseLevel()
        {
            _currentLevel--;
            _currentLevel = Math.Clamp(_currentLevel,0, GameParameters.GameplayValues.SpawnLevelAmount);
        }

        public void CreateBatchData()
        {
            var amount = 10_000;
            var arrayData = new SpawnType[amount];
            
            for (int i = 0; i < amount; i++)
            {
                var data = _spawnSelector.GetSpawnType(spawnData.GetDataByIndex(_currentLevel).SpawnTypeRange);
                arrayData[i] = data;
            }

            var count = new int[5];

            var lastTemp = SpawnType.None;
            var tempCount = 0;
            
            for (int i = 0; i < amount; i++)
            {
                var item = arrayData[i];

                switch (item)
                {
                    case SpawnType.Random:
                        count[0]++;
                        break;
                    case SpawnType.Ascendent:
                        count[1]++;
                        break;
                    case SpawnType.Descendent:
                        count[2]++;
                        break;
                    case SpawnType.SamePosition:
                        count[3]++;
                        break;
                }

                if (lastTemp == item)
                {
                    tempCount++;

                    if (tempCount == 3)
                    {
                        count[4]++;
                    }
                }
                else
                {
                    tempCount = 0;
                }

                lastTemp = item;
            }
            
            Debug.Log($"-- Final Values -- \n" +
                $"Random:     {count[0]} \n" + 
                $"Ascendent:  {count[1]} \n" + 
                $"Descendent: {count[2]} \n" + 
                $"Same:       {count[3]} \n" +
                $"3 in a Row:       {count[4]} \n");
        }

        public void Restart()
        {
            _currentLevel = 0;
            _spawnSelector.RestartData();
        }
    }

#if UNITY_EDITOR

    
    [CustomEditor(typeof(SlotSelectorTester))]
    public class SkinChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SlotSelectorTester script = (SlotSelectorTester)target;
            GUILayout.Space(10);
            GUILayout.Label("Spawner", EditorStyles.boldLabel);
            if (GUILayout.Button("Create Batch Data")) script.CreateBatchData();
            if (GUILayout.Button("Restart")) script.Restart();
            GUILayout.Space(10);
            GUILayout.Label("Level", EditorStyles.boldLabel);
            if (GUILayout.Button("Increase")) script.IncreaseLevel();
            if (GUILayout.Button("Decrease")) script.DecreaseLevel();
        }
    }
    
#endif
}