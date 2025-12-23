using System;
using System.Collections.Generic;
using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class SkinButtonCreator 
    {
        private readonly Func<SkinSelectButton> _buttonSpawnerFunc;
        private readonly int _skinCount;
        private readonly Dictionary<SkinType, ISkinButton> _skinButtons = new Dictionary<SkinType, ISkinButton>();
        private SkinType _lasSelectedSkin;
        
        public event Action<SkinType> OnSkinSelected;

        public SkinButtonCreator(Func<SkinSelectButton> buttonSpawnerFunc)
        {
            _buttonSpawnerFunc = buttonSpawnerFunc;
            // Had to add One more than the real amount, don't know why
            _skinCount = 2 + 1;
            //_skinCount = (int)SkinType.DEFAULT_MAX;
        }

        public void Initialize()
        {
            CreateAllButtons();
        }

        public void DisableCurrentSkinButtonInteraction()
        {
            _lasSelectedSkin = SkinManager.Instance.GetCurrentSkinType();
            SetInteractable(_lasSelectedSkin, false);
        }
        
        public void ClearDisableButton()
        {
            SetInteractable(_lasSelectedSkin, true);
            SetInteractable(SkinManager.Instance.GetCurrentSkinType(), true);
            _lasSelectedSkin = SkinType.None;
        }

        private void CreateAllButtons()
        {
            var buttonsArray = new ISkinButton[_skinCount];
            
            for (int i = 0; i < _skinCount-1; i++) buttonsArray[i] = _buttonSpawnerFunc();
            for (int i = 1; i < _skinCount; i++) SetButtonText(buttonsArray[i-1], (SkinType)i);
        }
        
        private void SetButtonText(ISkinButton button, SkinType skinType)
        {
            var information = GetSkinInformation(skinType);

            var defaultSkinName = "";

            if (information != null)
                defaultSkinName = GetLocalizedString(information.NameCode);
            else
                defaultSkinName = skinType.ToString();

            button.SetData(defaultSkinName, skinType).OnSelect += Button_OnSelectHandler;
            
            _skinButtons.Add(skinType, button);
        }

        private ISkinInformation GetSkinInformation(SkinType type) => SkinManager.Instance.GetSkinInformationByType(type);
        private string GetLocalizedString(string key) => LocalizationManager.Instance.GetText(key);
        
        private void Button_OnSelectHandler(SkinType skinSelected)
        {
            OnSkinSelected?.Invoke(skinSelected);

            if (skinSelected != SkinType.None)
            {
                SetInteractable(_lasSelectedSkin, true);
                SetInteractable(skinSelected, false);
                _lasSelectedSkin = skinSelected;
            }
        }

        private void SetInteractable(SkinType skin, bool interactable)
        {
            if(_skinButtons.TryGetValue(skin, out var button))
                button.SetInteractable(interactable);
        }
    }
}