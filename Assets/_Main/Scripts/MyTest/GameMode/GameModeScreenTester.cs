using System.Collections;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.MySettings;
using _Main.Scripts.Save;
using _Main.Scripts.GlobalEvents;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.GameMode
{
#if UNITY_EDITOR   
    [AddComponentMenu("_Main/ModuleTester/GameModeScreenTester")]
    public class GameModeScreenTester: MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float volume;
        private float _lastVolume;
        
        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                StartCoroutine(Coroutine_LoadScreen());
            };

            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            var localization = LocalizationManager.Instance;
            var dataManager = DataManager.Instance;
            var settings = SettingsManager.Instance;
        }

        private void Update()
        {
            if (_lastVolume != volume)
            {
                SettingsManager.Instance.SetMasterVolume(volume);
                _lastVolume = volume;
            }
        }

        public void Reload()
        {
            StartCoroutine(Coroutine_ReloadScreen());
        }
        
        public IEnumerator Coroutine_ReloadScreen()
        {
            yield return new WaitForEndOfFrame();
            
            EarthEventCaller.Death();
            
            yield return new WaitForEndOfFrame();
            
            GameManager.Instance.LoadGameMode();
        }

        private IEnumerator Coroutine_LoadScreen()
        {
            AsyncOperation asyncLoadGameplay = SceneManager.LoadSceneAsync("GameplayModule", LoadSceneMode.Additive);
                
            while (!asyncLoadGameplay.isDone)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            
            BootEvents.InitializeMainSystem();
            
            yield return new WaitForEndOfFrame();
                
            BootEvents.InitializeSubSystems();

            yield return new WaitForEndOfFrame();
                
            GameManager.Instance.LoadGameMode();

            yield return new WaitForEndOfFrame();
        }
        

        public void GivePoints()
        {
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Value = 100,
                Type = ProjectileType.Meteor
                
            });
        }

        #region Event Bus

        private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
        {
            if (input.RequestType == EventRequestType.Requested)
            {
                if (input.ScreenType == ScreenType.GameMode)
                {
                    GameScreenEventCaller.EnableScreen(ScreenType.GameMode, EventRequestType.Granted);
                }
                else if (input.ScreenType == ScreenType.Pause)
                {
                    GameManager.Instance.LoadGameMode();
                }
                else
                {
                    GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Requested);
                }
            }
        }
        
        private void EventBus_GameScreen_Disable(GameScreenEvents.DisableScreen input)
        {
            GameManager.Instance.LoadGameMode();
        }

        #endregion
    }
    
    [CustomEditor(typeof(GameModeScreenTester))]
    public class ComponentsNameChangeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            GameModeScreenTester script = (GameModeScreenTester)target;
                
            if (GUILayout.Button("Give Points"))
            {
                // Llama al método normalmente
                script.GivePoints();
            }
            
            if (GUILayout.Button("Reload"))
            {
                // Llama al método normalmente
                script.Reload();
            }
                
        }
    }
    
#endif
}