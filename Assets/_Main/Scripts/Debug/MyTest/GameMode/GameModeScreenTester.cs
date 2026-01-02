using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Debug._Main.Scripts.Debug;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers.Save;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.MyTest.GameMode
{
#if UNITY_EDITOR   
    public class GameModeScreenTester: MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float volume;
        private float _lastVolume;
        private bool _isZoomIn;
        
        private void Awake()
        {
            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            GameScreenEventSubscriber.DisableScreen(EventBus_GameScreen_Disable);
        }

        private void Start()
        {
            SoundEvents.OnSoundManagerInitialized += LoadScene;
            
            LocalizationManager.LoadInstance();
            DataManager.LoadInstance();
            SettingsManager.LoadInstance();
            SoundManager.LoadInstance();
            GameConfigManager.LoadInstance();
            
            SoundEvents.InitializeSoundManager();

        }

        private void LoadScene()
        {
            var enumerator = TestTools.LoadScreen(
                new string[] { "GameplayModule" }, 
                0, 0, 
                null,
                GameManager.Instance.LoadGameMode);
            StartCoroutine(enumerator);
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

        public void GivePoints()
        {
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Value = 100,
                Type = ProjectileType.Meteor
                
            });
        }
        
        public void FailStreak()
        {
           ProjectileEventCaller.Collision(new CollisionData());
        }

        public void ToggleZoom()
        {
            if (_isZoomIn == false)  
            {
                CameraEventCaller.ZoomIn();
                _isZoomIn = true;
            }
            else
            {
                CameraEventCaller.ZoomOut();
                _isZoomIn = false;
            }
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
                
            if (GUILayout.Button("Give Points")) script.GivePoints();
            if (GUILayout.Button("Reload")) script.Reload();
            if (GUILayout.Button("Fail Streak")) script.FailStreak();
            if (GUILayout.Button("Toggle Zoom")) script.ToggleZoom();
                
        }
    }
    
#endif
}