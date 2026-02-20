using System;
using MeteorMadness.GlobalValues.Tools.Observer;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities
{
    public interface IAbilityView : IObserver
    {
        public event Action OnAbilityFinished;
    }
}