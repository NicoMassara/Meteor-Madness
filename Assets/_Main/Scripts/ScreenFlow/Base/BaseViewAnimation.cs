using System;
using MeteorMadness.GlobalValues.Tools.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Base
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
            _currentAnimator = animator;
            
            _currentAnimator.Play(() =>
            {
                onFinished?.Invoke();
                ClearAnimation();
            });
        }

        private void ClearAnimation()
        {
            _currentAnimator = null;
        }

        #endregion

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                _currentAnimator?.Resume();
            }
            else
            {
                _currentAnimator?.Pause();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus == false)
            {
                _currentAnimator?.Resume();
            }
            else
            {
                _currentAnimator?.Pause();
            }
        }
    }
}