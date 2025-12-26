using System;
using System.Collections.Generic;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Interfaces.Skins;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.Localization;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class SkinButtonCreator 
    {
        private readonly bool _createEmptyButtons;
        private readonly Func<SkinSelectButton> _buttonSpawnerFunc;
        private readonly Dictionary<SkinType, ISkinButton> _skinButtons = new Dictionary<SkinType, ISkinButton>();
        private ISkinButton[] _buttonsArray;
        private SkinType _lasSelectedSkin;
        
        public event Action<SkinType> OnSkinSelected;

        public SkinButtonCreator(Func<SkinSelectButton> buttonSpawnerFunc)
        {
            _buttonSpawnerFunc = buttonSpawnerFunc;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _createEmptyButtons = false;
#endif
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


        private interface ISortable
        {
            public int GetSortValue();
        }
        private class PrizeData : ISortable
        {
            public int Prize { get; set; }
            public int SkinIndex { get; set; }
            public int GetSortValue() => Prize;
        }

        private void CreateAllButtons()
        {
            // It needs to add 1 and then use _skinCount-1 to work, don't know, don't care but it works
            var skinCount = 1;
            
            for (int i = 1; i < (int)SkinType.DEFAULT_MAX-1; i++)
            {
                var skinName = ((SkinType)i).ToString();
                if (_createEmptyButtons == false)
                    if (skinName.Contains("Empty", StringComparison.OrdinalIgnoreCase))
                        continue;
                
                skinCount++;
            }
            
            var prizeArray = new PrizeData[skinCount];
            _buttonsArray = new ISkinButton[skinCount];

            for (int i = 0; i < skinCount; i++)
            {
                var item = new PrizeData
                {
                    Prize = GetSkinPrice((SkinType)i+1),
                    SkinIndex = i+1
                };
                
                prizeArray[i] = item;
            }
            
            Array.Sort(prizeArray, (a, b) => a.GetSortValue().CompareTo(b.GetSortValue()));
            
            for (int i = 0; i < prizeArray.Length - 1; i++)
            {
                _buttonsArray[i] = _buttonSpawnerFunc();
                var skinType = (SkinType)prizeArray[i].SkinIndex;
                InitializeButtonData(_buttonsArray[i], skinType);
            }
        }
        
        private void InitializeButtonData(ISkinButton button, SkinType skinType)
        {
            SetButtonText(button, skinType);
            button.OnSelect += Button_OnSelectHandler;
            _skinButtons.Add(skinType, button);
        }

        private int GetSkinPrice(SkinType type)
        {
            var info = SkinManager.Instance.GetSkinInformationByType(type);
            return info?.UnlockPrice ?? int.MaxValue;
        }

        private ISkinInformation GetSkinInformation(SkinType type)
        {
            return SkinManager.Instance.GetSkinInformationByType(type);
        }

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