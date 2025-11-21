using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts
{
    public class ModulesLoader : MonoBehaviour
    { 
        private string[] _moduleNames;
        private const int SceneOffset = 2;
        
        
        private void Start()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            
            _moduleNames = new string[sceneCount-SceneOffset];

            for (int i = SceneOffset; i < sceneCount; i++)
            {
                // Esto obtiene la ruta de la escena en Build Settings
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                // Extraemos solo el nombre de la escena (sin carpeta ni extensión)
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                _moduleNames[i-SceneOffset] = sceneName;
                
            }
            
            StartCoroutine(LoadModules());
            
        }

        private IEnumerator LoadModules()
        {
            foreach (var moduleName in _moduleNames)
            {
                var asyncOperation = SceneManager.LoadSceneAsync(moduleName, LoadSceneMode.Additive);
                
                yield return new WaitUntil(() => asyncOperation.isDone);
            }
            ModuleLoaderEvents.TriggerOnModulesLoaded();
            Destroy(gameObject);
            yield return null;
        }
        
    }
}