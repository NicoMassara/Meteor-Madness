using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts
{
    public class SceneLoader
    {

        public static void LoadModules()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                LoadScene("GameplayModule");
                LoadScene("MainMenuModule");
                LoadScene("TutorialModule");
                LoadScene("CosmeticsModule");
            }
            else
            {
                Debug.Log("Not In Core Scene Module, Modules will not load");
            }
        }

        private static void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}