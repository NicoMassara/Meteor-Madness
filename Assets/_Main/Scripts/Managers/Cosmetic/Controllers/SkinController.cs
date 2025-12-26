using System.Collections.Generic;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.GlobalValues.Interfaces.Skins;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    public class SkinController
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
                
                        //Debug.LogWarning($"{skinType} Skin Data duplicated found in Resources/{Path} was not loaded, check the SkinType and changed to load it!");
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
        
        private readonly AssetsLoader _assetsLoader;
        private GeneratedId _secureId;
        
        public SkinController()
        {
            _assetsLoader = new AssetsLoader();
        }

        public void Initialize(DataManager.SkinSaveData data)
        {
            _secureId = SecureValueManager.RegisterValue(data.SkinIndex);
        }

        #region Public Methods

        public ISkinInformation GetSkinInformationByType(SkinType skinType) 
            => _assetsLoader.Contains(skinType) ? _assetsLoader.GetData(skinType) : null;

        public ISkinDeathMessage GetSkinDeathMessageByType(SkinType skinType) 
            => _assetsLoader.Contains(skinType) ? _assetsLoader.GetData(skinType) : null;


        public SkinType GetCurrentSkinType()
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out int skinIndex) == false)
            {
                Debug.LogWarning("Skin save data not found");
                return SkinType.Default;
            }
            
            return (SkinType)skinIndex;
        }

        public bool TrySetCurrentSkinType(SkinType skinType)
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out int skinIndex) == false)
            {
                Debug.LogWarning("Skin save data not found");
                return false;
            }
            
            skinIndex = (int)skinType;
            SecureValueManager.ModifyValue(_secureId, skinIndex);
            return true;
        }

        public bool GetHasData(SkinType skinType)
        {
            return _assetsLoader.Contains(skinType);
        }

        public IEarthSkinData GetEarthData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).EarthData;
        }
        
        public IMeteorSkinData GetMeteorData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).MeteorData;
        }
        
        public ICometSkinData GetCometData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).CometData;
        }
        
        public IShieldSkinData GetShieldData(SkinType skinType)
        {
            if(GetHasData(skinType) == false) return null;
            
            return _assetsLoader.GetData(skinType).ShieldData;
        }
        
        #endregion
    }
    
}