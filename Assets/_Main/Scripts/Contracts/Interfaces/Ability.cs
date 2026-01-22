using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IAbilityUIView
    {
        public event Action OnTriggerButtonPressed;
    }
    
    public interface IAbilityViewAnimation : IBaseViewAnimation
    {
        public event Action OnDataInitialized;
    }

    public interface IAbilityButtonUpdater
    {
        public event Action OnAbilityAdded;
        public event Action OnAbilityTriggered;
        public event Action OnAbilityFinished;
    }

}