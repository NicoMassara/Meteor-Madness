using System;
using DG.Tweening;
using NicolasMassara.CustomUpdateManager;

namespace MeteorMadness.ScreenFlow.Base
{
    public abstract class SequenceUIAnimation<T, TS> : IUiAnimation
    where T : IUiAnimationComponent
    where TS : IUiAnimationData
    {
        protected readonly T UIComponents;
        protected readonly TS AnimationData;
        
        private Sequence _sequence;
        
        protected SequenceUIAnimation(T components, TS animationData)
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
            
            _sequence = CreateAnimation()
                .AppendCallback(() => onFinished?.Invoke())
                .AppendCallback(RestartValues);
        }

        public void Pause() => _sequence?.Pause();
        public void Resume() => _sequence?.Play();
        public void Kill(bool waitForComplete = false) => _sequence?.Kill(waitForComplete);
    }
    
    public abstract class SequenceUIAnimationVibration<T, TS, TA> : SequenceUIAnimation<T, TS>
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

    public interface IUiAnimation
    {
        public void Play(Action onFinished);

        public void Pause();
        public void Resume();

        public void Kill(bool waitForComplete = false);
    }
    
    public interface IUiAnimationComponent {}
    public interface IAnimationVibrationComponent {}
}