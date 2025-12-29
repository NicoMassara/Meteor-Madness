using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Base
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