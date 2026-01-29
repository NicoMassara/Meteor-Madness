using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts.Events;
using NicolasMassara.CustomUpdateManager;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet.Test
{
    public class CometSpawnTester : MonoBehaviour
    {
        [Range(0,1f)]
        [SerializeField] private float timeScale;
        private bool _isEnable;
        private bool _isPaused;
        internal bool IsEnable => _isEnable;
        internal bool IsPaused => _isPaused;

        private void Awake()
        {
            SoundManager.LoadInstance();
            BootEvents.OnSubSystemInitialized += Initialized;
            SoundEvents.OnSoundManagerInitialized += () =>
            {
                BootEvents.InitializeSubSystems();
            };
        }

        private void Start()
        {
            SoundEvents.InitializeSoundManager();
        }

        private void Update()
        {
            CustomTime.GlobalFixedTimeScale = timeScale;
            CustomTime.GlobalTimeScale = timeScale;
        }

        private void Initialized()
        {
            CometSpawnEventCaller.Enable();
            _isEnable = true;
        }

        public void ToggleCometSpawn()
        {
            if (_isEnable)
            {
                CometSpawnEventCaller.Disable();
                _isEnable = false;
            }
            else
            {
                CometSpawnEventCaller.Enable();
                _isEnable = true;
            }
        }

        public void PauseCometSpawn()
        {
            CometSpawnEventCaller.Enable();
            _isPaused = true;
        }

        public void ResumeCometSpawn()
        {
            CometSpawnEventCaller.Enable();
            _isPaused = false;
        }
    }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(CometSpawnTester))]
    public class AbilitySelectorTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CometSpawnTester script = (CometSpawnTester)target;
            
            if (GUILayout.Button(script.IsEnable ? "Disable" : "Enable")) script.ToggleCometSpawn();
            if (script.IsPaused)
            {
                if (GUILayout.Button("Pause")) script.PauseCometSpawn();
            }
            else
            {
                if (GUILayout.Button("Resume")) script.ResumeCometSpawn();
            }

        }
    }
    
#endif
}