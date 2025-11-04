using System;
using UnityEngine;
using UnityEngine.UIElements;

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
        [Header("Main Panel")] 
        public GameObject MainPanel;
    }
}