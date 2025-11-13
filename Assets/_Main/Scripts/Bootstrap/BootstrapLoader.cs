using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Bootstrap
{
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad = "MainMenu"; // o el nombre de tu primera escena real
        [SerializeField] private float delayBeforeLoad = 0.1f;    // opcional, da tiempo al splash

        private IEnumerator Start()
        {
            // Espera un frame o unos ms para dejar que aparezca el splash/logo
            yield return new WaitForSeconds(delayBeforeLoad);

            // Ahora carga la escena principal en segundo plano
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

            // Opcional: mostrar barra de carga, logo, etc.
            while (!asyncLoad.isDone)
                yield return null;
        }
    }
}