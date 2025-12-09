using System;
using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface IPanelSelector<T> 
    where T : UiComponentsData
    {
        public T GetPanelData();
    }

    [Serializable]
    public abstract class UiComponentsData
    {
        private GameObject _activePanel;

        public void SetActivePanel(GameObject panelObject)
        {
            _activePanel?.SetActive(false);
            _activePanel = panelObject;
            _activePanel.SetActive(true);
        }
        public void DisableActivePanel()
        {
            _activePanel?.SetActive(false);
        }
    }
}