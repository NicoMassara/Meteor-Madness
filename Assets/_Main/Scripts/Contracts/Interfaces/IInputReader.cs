using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IInputReader
    {
        public event Action<float> OnMoved;
        public event Action<float> OnMagnitudeChanged;
        public event Action OnStopInput;
        public event Action OnPrepareToMove;
    }

    public interface IInputUI
    {
        public event Action<float> OnMoved;
        public event Action<float> OnMagnitudeChanged;
        public event Action OnStopInput;
        public event Action OnEnable;
        public event Action OnDisable;
    }
}