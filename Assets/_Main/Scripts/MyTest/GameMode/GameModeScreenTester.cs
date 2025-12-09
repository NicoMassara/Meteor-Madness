using System.Collections;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.GameMode
{
#if UNITY_EDITOR   
    [AddComponentMenu("_Main/ModuleTester/GameModeScreenTester")]
    public class GameModeScreenTester: MonoBehaviour
    {
        private bool _canReload;
        
        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                StartCoroutine(Coroutine_LoadScreen());
            };

            GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
        }
        
        private void Start()
        {
            var localization = LocalizationManager.Instance;
            var dataManager = DataManager.Instance;
            
        }

        private IEnumerator Coroutine_LoadScreen()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameplayModule", LoadSceneMode.Additive);
                
            while (!asyncLoad.isDone)
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


        public void LoadScreen()
        {
            if (_canReload)
            {
                
            }
        }

        public void GivePoints()
        {
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Value = 1,
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
                else
                {
                    GameScreenEventCaller.DisableScreen(ScreenType.GameMode, EventRequestType.Requested);
                    _canReload = true;
                }
            }
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
                
        }
    }
    
#endif
}