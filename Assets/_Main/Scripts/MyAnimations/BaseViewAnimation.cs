using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Observer;
using _Main.Scripts.Utilities;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyAnimations
{
    public abstract class BaseViewAnimation<T,TS> : ManagedBehavior, IObserver
    where TS : UiComponentsData
    where T : UiComponentsSelector<TS>
    {
        public interface IBaseViewAnimation
        {
            public event Action OnPanelOpened;
            public event Action OnPanelClosed;
        }
        
        [SerializeField] private T uiPanelSelector;
        private IUIAnimator _currentAnimator;
        
        protected TS UIComponents => uiPanelSelector.GetPanelData();
        
        public event Action OnPanelOpened;
        public event Action OnPanelClosed;
        
        public abstract void OnNotify(ulong message, params object[] args);

        #region Actions

        protected void TriggerOnPanelOpened()
        {
            OnPanelOpened?.Invoke();
        }

        protected void TriggerOnPanelClosed()
        {
            OnPanelClosed?.Invoke();
        }
        
        protected void PlayAnimation(IUIAnimator animator, Action onFinished = null)
        {
            animator.Play(onFinished);
        }

        #endregion
    }
}