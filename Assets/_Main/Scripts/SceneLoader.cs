using UnityEngine.SceneManagement;

namespace _Main.Scripts
{
    public class SceneLoader
    {

        public static void LoadModules()
        {
            LoadScene("GameplayModule");
            LoadScene("MainMenuModule");
            LoadScene("TutorialModule");
        }

        private static void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}