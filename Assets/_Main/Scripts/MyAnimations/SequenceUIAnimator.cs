using System;
using _Main.Scripts.Interfaces;
using DG.Tweening;

namespace _Main.Scripts.MyAnimations
{
    public abstract class SequenceUIAnimator<T> : IUIAnimator
    where T : IUiAnimationComponent
    {
        protected readonly T UIComponents;
        
        protected SequenceUIAnimator(T components)
        {
            UIComponents = components;
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }


        protected virtual void Initialize() { }

        protected virtual void RestartValues(){}
        
        protected abstract Sequence CreateAnimation();
        
        
        public void Play(Action onFinished)
        {
            CreateAnimation()
                .AppendCallback(() => onFinished?.Invoke())
                .AppendCallback(RestartValues);
        }
    }

    /// <summary>
    /// This is used to set Initial Positions and Values to UI Components or restart them
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UIComponentsInitializer<T> : IUIComponentsInitializer     
        where T : IUiAnimationComponent
    {
        protected readonly T UIComponents;
        
        // ReSharper disable once VirtualMemberCallInConstructor
        protected UIComponentsInitializer(T components)
        {
            UIComponents = components;
            Initialize();
        }

        protected abstract void Initialize();
        
        public void RestartValues() => Initialize();
    }

    public interface IUIComponentsInitializer
    {
        public void RestartValues();
    }

    public interface IUIAnimator
    {
        public void Play(Action onFinished);
    }
    
    public interface IUiAnimationComponent {}
}