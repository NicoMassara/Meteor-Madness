using System;
using DG.Tweening;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Base
{
    public abstract class BaseViewAnimation<T,TS> : ManagedBehavior, IObserver
    where TS : UiComponentsData
    where T : UiComponentsSelector<TS>
    {
        [SerializeField] private T uiPanelSelector;
        private IUiAnimation _currentAnimation;
        
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
        
        
        protected void PlayAnimation(IUiAnimation animator, Action onFinished = null, bool doesOverride = false)
        {
            if (doesOverride)
            {
                _currentAnimation?.Kill();
            }

            _currentAnimation = animator;
            
            _currentAnimation.Play(() =>
            {
                onFinished?.Invoke();
                ClearAnimation();
            });
        }

        private void ClearAnimation()
        {
            _currentAnimation = null;
        }

        #endregion
    }
}