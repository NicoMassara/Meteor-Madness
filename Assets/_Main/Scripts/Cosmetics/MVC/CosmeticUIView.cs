using System;
using _Main.Scripts.Cosmetics.Components;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;

namespace _Main.Scripts.Cosmetics.MVC
{
    public class CosmeticUIView : BaseViewUI<CosmeticUIPanelSelector, CosmeticUIComponents>, 
        ICosmeticUISounds, ICosmeticUIVibration, CosmeticUIView.ICosmeticUIView
    {
        
        public interface ICosmeticUIView
        {
            public event Action OnMainMenuButtonPressed;
            public event Action<int> OnSkinSelected;
        }
        
        private SkinButtonCreator _skinButtonCreator;
        public event Action OnMainMenuButtonPressed;
        public event Action<int> OnSkinSelected;

        private void Awake()
        {
            UIComponents.MainMenuButton.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
            
            _skinButtonCreator = new SkinButtonCreator(()=> Instantiate(UIComponents.SkinSelectButton, UIComponents.ButtonsContainer));
            _skinButtonCreator.OnSkinSelected += ButtonCreator_OnSkinSelectedHandler;
        }
        
        private void Start()
        {
            _skinButtonCreator.Initialize();   
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case CosmeticObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case CosmeticObserverMessage.Disable:
                    HandleDisable();
                    break;
            }
        }
        
        private void HandleInitialize()
        {
            _skinButtonCreator.DisableCurrentSkinButtonInteraction();
        }
        
        private void HandleDisable()
        {
            _skinButtonCreator.ClearDisableButton();
        }

        private void ButtonCreator_OnSkinSelectedHandler(SkinType skinSelected)
        {
            OnSkinSelected?.Invoke((int)skinSelected);
        }
    }
}