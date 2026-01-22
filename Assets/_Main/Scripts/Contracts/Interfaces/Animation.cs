using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IBaseViewAnimation
    {
        public event Action OnPanelOpened;
        public event Action OnPanelClosed;
    }
}