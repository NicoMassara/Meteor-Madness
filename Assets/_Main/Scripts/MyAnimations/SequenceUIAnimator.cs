using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Vibration;
using DG.Tweening;
using UnityEngine;

namespace _Main.Scripts.MyAnimations
{
    public abstract class SequenceUIAnimator<T, TS> : IUIAnimator
    where T : IUiAnimationComponent
    where TS : IUiAnimationData
    {
        protected readonly T UIComponents;
        protected readonly TS AnimationData;
        
        private Sequence _sequence;
        
        
        protected SequenceUIAnimator(T components, TS animationData)
        {
            UIComponents = components;
            AnimationData = animationData;
        }


        protected virtual void Initialize() { }

        protected virtual void RestartValues(){}
        
        protected abstract Sequence CreateAnimation();
        
        
        public void Play(Action onFinished)
        {
            Initialize();
            
            CreateAnimation()
                .AppendCallback(() => onFinished?.Invoke())
                .AppendCallback(RestartValues);
        }

        public void Pause() => _sequence?.Pause();
        public void Resume() => _sequence?.Play();
        public void Kill() => _sequence?.Kill();
    }
    
    public abstract class SequenceUIAnimationVibration<T, TS, TA> : SequenceUIAnimator<T, TS>
        where T : IUiAnimationComponent
        where TS : IUiAnimationData
        where TA : IAnimationVibrationComponent
    {

        protected TA VibrationComponent { get; private set; }

        protected SequenceUIAnimationVibration(T components, TS animationData, TA vibrationComponent) 
            : base(components, animationData)
        {
            VibrationComponent = vibrationComponent; 
        }
    }
    
    
    public interface IUiAnimationData {}

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
        
        public void InitializeValues() => Initialize();
    }

    public interface IUIComponentsInitializer
    {
        public void InitializeValues();
    }

    public interface IUIAnimator
    {
        public void Play(Action onFinished);

        public void Pause();
        public void Resume();

        public void Kill();
    }
    
    public interface IUiAnimationComponent {}
    public interface IAnimationVibrationComponent {}
}