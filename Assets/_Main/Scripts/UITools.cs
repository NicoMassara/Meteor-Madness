using UnityEngine;

namespace _Main.Scripts
{
    public class UITools
    {
        public static void SetActivePanel(GameObject newPanel, GameObject currentPanel)
        {
            currentPanel?.SetActive(false);
            currentPanel = newPanel;
            currentPanel?.SetActive(true);
        }
    }
}