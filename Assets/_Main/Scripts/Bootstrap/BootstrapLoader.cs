using System.Collections;
using _Main.Scripts.Localization;
using _Main.Scripts.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.Bootstrap
{
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string coreScene = "MainMenu"; // o el nombre de tu primera escena real
        [SerializeField] private string[] additiveScenes;
        [SerializeField] private float delayBeforeLoad = 0.1f;    // opcional, da tiempo al splash
        [SerializeField] private Image progressBar;
        
        private bool _hasLocalizationLoaded;
        private bool _hasLoadedData;

        private void Awake()
        {
            LocalizationEvents.OnLocalizationLoaded += () =>
            {
                _hasLocalizationLoaded = true;
            };
            
            SaveDataEvents.OnSaveInitialized += () =>
            {
                _hasLocalizationLoaded = true;
            };
            
            var localization = LocalizationManager.Instance;
            var dataManager = DataManager.Instance;
        }

        private void Start()
        {
            StartCoroutine(LoadCoreScene());
        }

        private IEnumerator LoadCoreScene()
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(coreScene, LoadSceneMode.Single);
            
            asyncLoad.allowSceneActivation = false;
            
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            asyncLoad.allowSceneActivation = true;
            
            yield return new WaitUntil(() => asyncLoad.isDone &&
                                             GetHasLoadedAssets());
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreScene));
        }

        private bool GetHasLoadedAssets()
        {
            return _hasLocalizationLoaded &&
                   _hasLoadedData;
        }
    }
}