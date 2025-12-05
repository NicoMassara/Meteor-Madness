using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;

namespace _Main.Scripts.Defeat
{
    public class DefeatUIView : BaseViewUI<DefeatUiPanelSelector,DefeatUIComponents>, IObserver,
        DefeatUIView.IDefeatUIView, IDefeatUiSounds
    {
        public interface IDefeatUIView
        {
            public event Action OnMainMenuButtonPressed;
            public event Action OnRestartButtonPressed;
        }
        
        public event Action OnMainMenuButtonPressed;
        public event Action OnRestartButtonPressed;
        

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case DefeatObserverMessage.EnableButtons:
                    HandleEnableButtons();
                    break;
            }
        }

        private void HandleStartDisable()
        {
            UIComponents.RestartButton.onClick.RemoveAllListeners();
            UIComponents.MainMenuButtons.onClick.RemoveAllListeners();
        }

        private void HandleEnableButtons()
        {
            UIComponents.RestartButton.onClick.AddListener(() =>
            {
                OnRestartButtonPressed?.Invoke();
            });
            
            UIComponents.MainMenuButtons.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
        }
    }
}