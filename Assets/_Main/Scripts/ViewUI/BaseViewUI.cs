using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;
using _Main.Scripts.Utilities;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.ViewUI
{
    public abstract class BaseViewUI<T,TS> : ManagedBehavior, IObserver 
        where TS : UiComponentsData
        where T : UiComponentsSelector<TS>
    {
        [SerializeField] private T uiPanelSelector;
        
        protected TS UIComponents => uiPanelSelector.GetPanelData();
        
        public abstract void OnNotify(ulong message, params object[] args);
    }
}