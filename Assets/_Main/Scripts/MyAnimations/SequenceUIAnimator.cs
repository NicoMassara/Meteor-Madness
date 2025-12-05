using System;
using DG.Tweening;

namespace _Main.Scripts.MyAnimations
{
    public abstract class SequenceUIAnimator<T> : IAnimator
    {
        protected readonly T Components;

        protected SequenceUIAnimator(T components)
        {
            Components = components;
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }
            
        protected abstract void Initialize();
        protected virtual void RestartValues(){}

        protected abstract Sequence CreateFadeIn();
        protected abstract Sequence CreateFadeOut();

        public virtual void FadeIn(Action onFinished)
        {
            CreateFadeIn()
                .AppendCallback(() => onFinished?.Invoke());
        }

        public virtual void FadeOut(Action onFinished)
        {
            CreateFadeOut()
                .AppendCallback(RestartValues)
                .AppendCallback(() => onFinished?.Invoke());
        }
    }
    public interface IAnimator
    {
        public void FadeIn(Action onFinished);
        public void FadeOut(Action onFinished);
    }
    
    public interface IUiAnimationComponent {}
}