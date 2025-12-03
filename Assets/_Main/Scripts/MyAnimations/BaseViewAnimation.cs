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
        [SerializeField] private T uiPanelSelector;
        private IAnimator _currentAnimator;
        
        protected TS UIComponents => uiPanelSelector.GetPanelData();
        
        public event Action OnPanelOpened;
        public event Action OnPanelClosed;
        
        public abstract void OnNotify(ulong message, params object[] args);

        #region Actions

        protected void SetAnimator(IAnimator animator)
        {
            if (_currentAnimator != null)
            {
                _currentAnimator?.FadeOut(() =>
                {
                    OnPanelClosed?.Invoke();
                    
                    animator?.FadeIn(() =>
                    {
                        OnPanelOpened?.Invoke();
                    });
                });
            }
            else
            {
                animator?.FadeIn(() =>
                {
                    OnPanelOpened?.Invoke();
                });
            }

            _currentAnimator = animator;
        }

        protected void ClearAnimator()
        {
            SetAnimator(null);
        }

        #endregion
    }
}