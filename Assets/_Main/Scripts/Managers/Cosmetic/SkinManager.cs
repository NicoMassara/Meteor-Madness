using System;
using System.Collections.Generic;
using _Main.Scripts.Cosmetics;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.GlobalValues.Interfaces.Skins;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace MeteorMadness.Managers.Cosmetics
{
    public class SkinManager : SingletonBehaviour<SkinManager>
    {
        private DataManager.SkinSaveData _skinData;
        
        // === Controllers === // 
        private SkinController _skinController;
        private LockedSkinController _lockedSkinController;
        private CoinsController _coinsController;

        private SkinType _currentSkinPreview;
        
        public event Action<SkinType> OnSkinChanged;
        
        private void Awake()
        {
            _skinController = new SkinController();
            _lockedSkinController = new LockedSkinController();
            _coinsController = new CoinsController();
            
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }
        
        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //
            var data = DataManager.Instance.GetData<DataManager.SkinSaveData>(DataManager.SaveDataType.Skin);
            if (data == null)
            {
                Debug.LogWarning("Skin save data not found");
                return;
            }
            
            _skinController.Initialize(data);
            _lockedSkinController.Initialize(data);
            _coinsController.Initialize(data);

            SetInitialSkin();
            
            //
            BootEvents.MainSystemInitialized();
        }

        private void SetInitialSkin()
        {
            var storedSkin = _skinController.GetCurrentSkinType();

            var deathTitle = "";

            if (_lockedSkinController.GetIsLocked((int)storedSkin) == false && 
                _skinController.TrySetCurrentSkinType(storedSkin))
            {
                OnSkinChanged?.Invoke(storedSkin);
                deathTitle = _skinController.GetSkinDeathMessageByType(storedSkin).DeathTitle;
            }
            else
            {
                OnSkinChanged?.Invoke(SkinType.Default);
                deathTitle = _skinController.GetSkinDeathMessageByType(SkinType.Default).DeathTitle;
            }
            
            GameManager.Instance.DeathTitle = deathTitle;
            
            
            SkinEvents.TriggerOnSaveLoaded();
        }


        public void PreviewSkin(SkinType skinType)
        {
            _currentSkinPreview = skinType;
            
            if (_lockedSkinController.GetIsLocked((int)skinType) == false && 
                _skinController.GetCurrentSkinType() != _currentSkinPreview &&
                _skinController.TrySetCurrentSkinType(skinType))
            {
                Debug.Log($"{skinType} skin selected!");
            }
            
            OnSkinChanged?.Invoke(skinType);
        }
        
        public void SaveSelected()
        {
            if (_currentSkinPreview != SkinType.None)
            {
                var isLocked = _lockedSkinController.GetIsLocked((int)_currentSkinPreview);

                if (isLocked == false)
                {
                    if (_skinController.TrySetCurrentSkinType(_currentSkinPreview))
                    {
                        OnSkinChanged?.Invoke(_skinController.GetCurrentSkinType());
                    }
                }
                else
                {
                    Debug.Log($"{_currentSkinPreview} skin is locked, reverting to {_skinController.GetCurrentSkinType()} skin!");
                    
                    OnSkinChanged?.Invoke(_skinController.GetCurrentSkinType());
                }
            }
            
            if(_lockedSkinController.GetIsLocked((int)_skinController.GetCurrentSkinType()))
            {
                if (_skinController.TrySetCurrentSkinType(SkinType.Default))
                {
                    OnSkinChanged?.Invoke(_skinController.GetCurrentSkinType());
                }
            }

            var data = DataManager.Instance.GetData<DataManager.SkinSaveData>(DataManager.SaveDataType.Skin);

            
            var title = _skinController.GetSkinDeathMessageByType(_skinController.GetCurrentSkinType()).DeathTitle;
            GameManager.Instance.DeathTitle = title;
            
            _currentSkinPreview = SkinType.None;

            data.SkinIndex = (int)_skinController.GetCurrentSkinType();
            data.UnlockedSkins = _lockedSkinController.GetUnlockedSkins();
            data.SkinCoins = _coinsController.GetCoins();
            
            DataManager.Instance.SaveGameData(data, DataManager.SaveDataType.Skin);
        }
        

        #region Data Getters
        
        public SkinType GetPreviewedSkin() => _currentSkinPreview;

        public void UnlockPreviewSkin()
        {
            _skinController.TrySetCurrentSkinType(_currentSkinPreview);
            _currentSkinPreview = SkinType.None;
        }

        // === Skin Controller ===// 
        public SkinType GetCurrentSkinType() => _skinController.GetCurrentSkinType();
        public bool GetHasData(SkinType skinType) => _skinController.GetHasData(skinType);
        public ISkinInformation GetSkinInformationByType(SkinType skinType) => _skinController.GetSkinInformationByType(skinType);
        public ISkinDeathMessage GetSkinDeathMessageByType(SkinType skinType) => _skinController.GetSkinDeathMessageByType(skinType);
        
        // MODIFY - This functions should return the current skin data
        public IEarthSkinData GetEarthData(SkinType skinType) => _skinController.GetEarthData(skinType);
        public IMeteorSkinData GetMeteorData(SkinType skinType) => _skinController.GetMeteorData(skinType);
        public ICometSkinData GetCometData(SkinType skinType) => _skinController.GetCometData(skinType);
        public IShieldSkinData GetShieldData(SkinType skinType) => _skinController.GetShieldData(skinType);

        // === Locked Skin Controller ===// 
        public List<int> GetUnlockedSkins() => _lockedSkinController.GetUnlockedSkins();
        public void UnlockSkin(int skinIndex) => _lockedSkinController.UnlockSkin(skinIndex);
        public bool GetIsLocked(int skinIndex) => _lockedSkinController.GetIsLocked(skinIndex);
        
        // === Coins Controller ===// 
        public bool TryAddCoins(uint score) => _coinsController.TryAddCoins(score);
        public uint GetCoins() => _coinsController.GetCoins();
        public bool GetContainsEnoughCoins(uint coinsAmount) => _coinsController.GetContainsEnoughCoins(coinsAmount);
        public bool TryRemoveCoins(uint coinsToRemove) => _coinsController.TryRemoveCoins(coinsToRemove);
        public void SaveStoredCoins() => _coinsController.SaveStoredCoins();

        #endregion

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        public void ForceSkin(SkinType type)
        {
            if (_skinController.TrySetCurrentSkinType(type))
            {
                OnSkinChanged?.Invoke(_skinController.GetCurrentSkinType());
            }
        }
        
#endif
    }
}