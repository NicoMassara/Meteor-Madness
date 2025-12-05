using System.Collections;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.MyTest.Defeat
{
#if UNITY_EDITOR   
        [AddComponentMenu("_Main/ModuleTester/DefeatScreenTester")]
        public class DefeatScreenTester : MonoBehaviour
        {
            [Range(0,1000)]
            [SerializeField] private uint scoreAmount;
            [Range(0,1000)]
            [SerializeField] private uint highScore;
            
            private bool _canReload;
            
            private void Awake()
            {
                LocalizationEvents.OnLocalizationLoaded += () =>
                {
                    StartCoroutine(LoadDefeatScreen());
                };

                GameScreenEventSubscriber.EnableScreen(EventBus_GameScreen_Enable);
            }
            
            private void Start()
            {
                var localization = LocalizationManager.Instance;
                var dataManager = DataManager.Instance;
            }
            
            private void SetScoreValue()
            {
                GameManager.Instance.CurrentScoreSecuredId = 
                    SecureValueManager.RegisterValue(scoreAmount);

                SecureValueManager.ModifyValue(GameManager.Instance.GetHighScoreSecuredId(), highScore);
            }
            
            private IEnumerator LoadDefeatScreen()
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DefeatModule", LoadSceneMode.Additive);
                
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                
                yield return new WaitForEndOfFrame();
                
                BootEvents.InitializeMainSystem();

                yield return new WaitForSeconds(1);
                
                BootEvents.InitializeSubSystems();

                yield return new WaitForEndOfFrame();
                
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForEndOfFrame();
                
                EarthEventCaller.DestructionFinished();
            }

            private IEnumerator ReloadDefeatScreen()
            {
                SetScoreValue();
                
                yield return new WaitForEndOfFrame();
                
                GameManager.Instance.LoadDefeatScreen();

                yield return new WaitForEndOfFrame();
                
                EarthEventCaller.DestructionFinished();
            }

            public void LoadScreen()
            {
                if(_canReload)
                    StartCoroutine(ReloadDefeatScreen());
            }

            #region Event Bus

            private void EventBus_GameScreen_Enable(GameScreenEvents.EnableScreen input)
            {
                if (input.RequestType == EventRequestType.Requested)
                {
                    if (input.ScreenType == ScreenType.Defeat)
                    {
                        GameScreenEventCaller.EnableScreen(ScreenType.Defeat, EventRequestType.Granted);
                    }
                    else
                    {
                        GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Requested);
                        EarthEventCaller.RestartFinished();
                        _canReload = true;
                    }
                }
            }

            #endregion
        }
        
        [CustomEditor(typeof(DefeatScreenTester))]
        public class ComponentsNameChangeEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                // Dibuja el inspector normal
                DrawDefaultInspector();

                // Agrega el botón
                DefeatScreenTester script = (DefeatScreenTester)target;
                
                if (GUILayout.Button("Load Screen"))
                {
                    // Llama al método normalmente
                    script.LoadScreen();
                }
                
            }
        }
#endif
}
