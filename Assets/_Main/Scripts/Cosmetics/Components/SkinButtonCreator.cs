using System;
using System.Collections.Generic;
using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class SkinButtonCreator 
    {
        private readonly Func<SkinSelectButton> _buttonSpawnerFunc;
        private readonly Dictionary<SkinType, ISkinButton> _skinButtons = new Dictionary<SkinType, ISkinButton>();
        private ISkinButton[] _buttonsArray;
        private int _skinCount;
        private SkinType _lasSelectedSkin;
        
        public event Action<SkinType> OnSkinSelected;

        public SkinButtonCreator(Func<SkinSelectButton> buttonSpawnerFunc)
        {
            _buttonSpawnerFunc = buttonSpawnerFunc;
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

        public void UpdateButtonsName()
        {
            foreach (var item in _skinButtons)
            {
                SetButtonText(item.Value, item.Key);
            }
        }

        private void CreateAllButtons()
        {
            // It needs to add 1 and then use _skinCount-1 to work, don't know, don't care but it works
            _skinCount = 1;
            
            for (int i = 1; i < (int)SkinType.DEFAULT_MAX; i++)
            {
                var skinName = ((SkinType)i).ToString();
                if (skinName.Contains("Empty", StringComparison.OrdinalIgnoreCase))
                    continue;
                
                _skinCount++;
            }
            
            _buttonsArray = new ISkinButton[_skinCount];

            for (int i = 0; i < _skinCount-1; i++) _buttonsArray[i] = _buttonSpawnerFunc();
            for (int i = 1; i < _skinCount; i++) InitializeButtonData(_buttonsArray[i-1], (SkinType)i);
        }
        
        private void InitializeButtonData(ISkinButton button, SkinType skinType)
        {
            SetButtonText(button, skinType);
            button.OnSelect += Button_OnSelectHandler;
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

        private void SetButtonText(ISkinButton button, SkinType skinType)
        {
            var information = GetSkinInformation(skinType);

            var defaultSkinName = "";

            if (information != null)
                defaultSkinName = GetLocalizedString(information.NameCode);
            else
                defaultSkinName = skinType.ToString();

            button.SetData(defaultSkinName, skinType);
        }

        private void SetInteractable(SkinType skin, bool interactable)
        {
            if(_skinButtons.TryGetValue(skin, out var button))
                button.SetInteractable(interactable);
        }

        #region Add / Remove Listener
        
        public void RemoveListenerFromAllButtons()
        {
            for (int i = 0; i < _buttonsArray.Length-1; i++)
            {
                var item = _buttonsArray[i];
                item.RemoveListener();
            }
        }

        public void AddListenerToAllButtons()
        {
            for (int i = 0; i < _buttonsArray.Length-1; i++)
            {
                var item = _buttonsArray[i];
                item.AddListener();
            }
        }
        
        #endregion
    }
}