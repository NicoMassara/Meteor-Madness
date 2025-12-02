using System;
using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyTools;
using _Main.Scripts.Save;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    public class SkinManager : SingletonBehaviour<SkinManager>
    {
        #region Tools
        private class AssetsLoader
        {
            private readonly Dictionary<SkinType, SkinDataSo> _dataDic = new Dictionary<SkinType, SkinDataSo>();
            private const string Path = "ScriptableObjects/Skins";
            private bool _hasLoaded = false;
            
            public AssetsLoader()
            {
                Initialize();
            }

            private void Initialize()
            {
                if(_hasLoaded) return;
                
                var loaded = Resources.LoadAll<SkinDataSo>(Path);

                foreach (var data in loaded)
                {
                    var skinType = data.SkinType;

                    if (_dataDic.ContainsKey(skinType))
                    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                
                        Debug.LogWarning($"{skinType} Skin Data duplicated found in Resources/{Path} was not loaded, check the SkinType and changed to load it!");
#endif
                        continue;
                    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                
                    //Debug.Log($"{skinType} Skin loaded from Resources/{Path}");
#endif
                
                    _dataDic.Add(skinType, data);
                }
                
                _hasLoaded = true;
                SkinEvents.TriggerOnAssetsLoaded();
            }

            public bool Contains(SkinType soundType)
            {
                return _dataDic.ContainsKey(soundType);
            }

            public SkinDataSo GetData(SkinType skinType)
            {
                return _dataDic[skinType];
            }
        }

        #endregion
        
        private AssetsLoader _assetsLoader;
        private DataManager.SkinSaveData _skinData;
        private CosmeticsDebugData _debugData;
        
        public event Action<SkinType> OnSkinChanged;
        
        private void Awake()
        {
            _assetsLoader = new AssetsLoader();
            _debugData = new CosmeticsDebugData();
            
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }
        
        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //
            _skinData = DataManager.Instance.GetData<DataManager.SkinSaveData>(DataManager.SaveDataType.Skin);
            if (_skinData == null)
            {
                Debug.LogWarning("Skin save data not found");
                return;
            }

            SelectSkin((SkinType)_skinData.SkinIndex);
            
            
            BootEvents.TriggerOnMainSystemInitialized();
            SkinEvents.TriggerOnSaveLoaded();
        }
        
        public void SelectSkin(SkinType skinType)
        {
            if(GetCurrentSkinType() == skinType) return;
            
            _skinData.SkinIndex = (int)skinType;
            
            _debugData.CurrentSkin = GetCurrentSkinType();
            Debug.Log($"Skin {GetCurrentSkinType()} is selected");
            OnSkinChanged?.Invoke(skinType);
        }
        
        public void SaveSelected()
        {
            DataManager.Instance.SaveGameData(_skinData, DataManager.SaveDataType.Skin);
        }
        
        public SkinType GetCurrentSkinType()
        {
            if (_skinData == null)
            {
                Debug.LogWarning("Skin save data not found");
                return SkinType.Default;
            }
            
            return (SkinType)_skinData.SkinIndex;
        }

        #region Data Getters
        
        public bool GetHasData(SkinType skinType)
        {
            return _assetsLoader.Contains(skinType);
        }

        public EarthSkinData GetEarthData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).EarthData;
        }
        
        public MeteorSkinData GetMeteorData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).MeteorData;
        }
        
        public CometSkinData GetCometData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).CometData;
        }
        
        public ShieldSkinData GetShieldData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).ShieldData;
        }
        
        #endregion
    }

    public enum SkinType
    {
        Default,
        Pizza
    }
}