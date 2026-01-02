using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Managers;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    
#if UNITY_EDITOR
    public class GameInputTest : MonoBehaviour
    {
        [Range(1, 3)]
        [SerializeField] private float toggleDelay;
        private bool _isEnable;
        private bool _isAutoToggle;

        private bool _isActive;
        
        public bool GetIsActive() => _isActive;
        public bool GetIsEnable() => _isEnable;
        public bool GetIsAutoToggle() => _isAutoToggle;
        
        private void Start()
        {
            GameManager.Instance.InputReader.OnAbilityTriggered += (value) =>
            {
                Debug.Log($"Ability Triggered: {value}");
            };
            
            GameManager.Instance.InputReader.OnMovementDirectionChanged += (value) =>
            {
                Debug.Log($"Movement: {value}");
            };

            _isActive = true;
            
            SetEnable(true);
        }

        public void SetEnable(bool isEnable)
        {
            _isEnable = isEnable;
            InputsEventCaller.SetEnable(_isEnable);
        }

        public void EnableAutoToggle()
        {
            _isAutoToggle = true;

            if (_isEnable)
            {
                SetEnable(false);
            }

            StartCoroutine(Coroutine_AutoToggle());
        }

        public void DisableAutoToggle()
        {
            _isAutoToggle = false;
        }

        private IEnumerator Coroutine_AutoToggle()
        {
            while (_isAutoToggle)
            {
                yield return new WaitForSeconds(toggleDelay);
            
                Debug.Log("Input Enable");
                SetEnable(true);
            
                yield return new WaitForSeconds(toggleDelay);
            
                Debug.Log("Input Disable");
                SetEnable(false);
            }
            
            yield return null;
        }
    }
    
#endif
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(GameInputTest))]
    public class ComponentsNameChangeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            GameInputTest script = (GameInputTest)target;

            if(script.GetIsActive() == false) return;
            
            
            if (script.GetIsAutoToggle())
            {
                if (GUILayout.Button("Disable Auto Toggle")) script.DisableAutoToggle();
            }
            else
            {
                if (GUILayout.Button("Enable Auto Toggle")) script.EnableAutoToggle();
                
                if (script.GetIsEnable())
                {
                    if (GUILayout.Button("Disable")) script.SetEnable(false);
                }
                else
                {
                    if (GUILayout.Button("Enable")) script.SetEnable(true);
                }
                
            }
            
        }
    }
#endif
}