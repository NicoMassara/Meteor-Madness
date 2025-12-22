using System;
using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using _Main.Scripts.Save;
using UnityEngine;
using _Main.Scripts.GlobalEvents;

namespace _Main.Scripts.Cosmetics
{
    public class SkinManager : SingletonBehaviour<SkinManager>
    {
        private DataManager.SkinSaveData _skinData;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private CosmeticsDebugData _debugData;
#endif
        
        // === Controllers === // 
        private SkinController _skinController;
        private LockedSkinController _lockedSkinController;
        private CoinsController _coinsController;

        private SkinType _currentSkinPreview;
        
        public event Action<SkinType> OnSkinChanged;
        
        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new CosmeticsDebugData();
#endif
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

            if (_lockedSkinController.GetIsLocked((int)storedSkin) == false && 
                _skinController.TrySetCurrentSkinType(storedSkin))
            {
                OnSkinChanged?.Invoke(storedSkin);
            }
            else
            {
                OnSkinChanged?.Invoke(SkinType.Default);
            }
            
            SkinEvents.TriggerOnSaveLoaded();
        }


        public void PreviewSkin(SkinType skinType)
        {
            _currentSkinPreview = skinType;
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
            
            _currentSkinPreview = SkinType.None;

            data.SkinIndex = (int)_skinController.GetCurrentSkinType();
            data.UnlockedSkins = _lockedSkinController.GetUnlockedSkins();
            data.SkinCoins = _coinsController.GetCoins();
            
            DataManager.Instance.SaveGameData(data, DataManager.SaveDataType.Skin);
        }
        

        #region Data Getters
        
        // === Skin Controller ===// 
        public SkinType GetCurrentSkinType() => _skinController.GetCurrentSkinType();
        public bool GetHasData(SkinType skinType) => _skinController.GetHasData(skinType);
        public EarthSkinData GetEarthData(SkinType skinType) => _skinController.GetEarthData(skinType);
        public MeteorSkinData GetMeteorData(SkinType skinType) => _skinController.GetMeteorData(skinType);
        public CometSkinData GetCometData(SkinType skinType) => _skinController.GetCometData(skinType);
        public ShieldSkinData GetShieldData(SkinType skinType) => _skinController.GetShieldData(skinType);

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