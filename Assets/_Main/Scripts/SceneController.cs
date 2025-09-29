using UnityEngine.SceneManagement;

namespace _Main.Scripts
{
    public class SceneController
    {
        
        private Scene _currentScene;

        public SceneController()
        {
            Initialize();
        }

        private void Initialize()
        {
            _currentScene = SceneManager.GetActiveScene();
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene(GameValues.GameplaySceneName);
        }

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(GameValues.MainMenuSceneName);
        }
        
        private void OnSceneLoadedHandler(Scene newScene, LoadSceneMode sceneMode)
        {
            _currentScene = newScene;
        }
    }
}