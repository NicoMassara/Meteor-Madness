using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.AbilityUI
{
#if UNITY_EDITOR   
    
    [AddComponentMenu("_Main/ModuleTester/Ability UI Tester")]
    public class AbilityScreenTester : MonoBehaviour
    {
        private IEnumerator _currentCoroutine;

        public bool IsLooping { get; private set; }
        public bool IsEnabled { get; private set; }

        private void Start()
        {
            StartCoroutine(Coroutine_LoadCoreScene());
        }

        public void ToggleEnable()
        {
            if (IsEnabled)
            {
                AbilitiesEventCaller.DisableUI();
            }
            else
            {
                AbilitiesEventCaller.EnableUI();
            }
            
            IsEnabled = !IsEnabled;
        }
        
        public void ToggleLoop()
        {
            if (IsLooping == false)
            {
                _currentCoroutine = Coroutine_OpenUI();
                StartCoroutine(_currentCoroutine);
                IsLooping = true;
            }
            else
            {
                StopCoroutine(_currentCoroutine);
                IsLooping = false;
            }
        }

        private IEnumerator Coroutine_LoadCoreScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CoreModule", LoadSceneMode.Additive);
                
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
                
            yield return new WaitForEndOfFrame();
                
            BootEvents.InitializeMainSystem();

            yield return new WaitForSeconds(1);
                
            BootEvents.InitializeSubSystems();

            yield return new WaitForEndOfFrame();
            
            BootEvents.TriggerOnGameLoaded();
            
            yield return new WaitForEndOfFrame();
            
            CameraEventCaller.ZoomIn();
            
            yield return new WaitForEndOfFrame();
            
            AbilitiesEventCaller.Enable();
            AbilitiesEventCaller.DisableUI();
        }

        private IEnumerator Coroutine_OpenUI()
        {
            yield return new WaitForSeconds(1);
            
            AbilitiesEventCaller.EnableUI();
            
            yield return new WaitForEndOfFrame();
            
            _currentCoroutine = Coroutine_CloseUI();
            StartCoroutine(_currentCoroutine);
            
        }
        
        private IEnumerator Coroutine_CloseUI()
        {
            yield return new WaitForSeconds(1);
            
            AbilitiesEventCaller.DisableUI();
            
            yield return new WaitForEndOfFrame();
            
            _currentCoroutine = Coroutine_OpenUI();
            StartCoroutine(_currentCoroutine);
        }
    }
    
    [CustomEditor(typeof(AbilityScreenTester))]
    public class ComponentsNameChangeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            AbilityScreenTester script = (AbilityScreenTester)target;

            if (script.IsLooping == false)
            {
                if (GUILayout.Button(script.IsEnabled ? "Disable" : "Enable"))
                {
                    // Llama al método normalmente
                    script.ToggleEnable();
                }
            }
            
            if (GUILayout.Button(script.IsLooping ? "Stop Looping" : "Start Loop"))
            {
                // Llama al método normalmente
                script.ToggleLoop();
            }
            
                
        }
    }
    
#endif
}