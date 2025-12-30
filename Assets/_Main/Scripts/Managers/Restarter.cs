using MeteorMadness.Contracts.Events;
using UnityEngine.SceneManagement;

namespace MeteorMadness.Managers
{
    public class Restarter
    {
        public static void RestartGame()
        {
            SingletonEvents.DestroySingleton();
            SceneManager.LoadScene(0);
        }
    }
}