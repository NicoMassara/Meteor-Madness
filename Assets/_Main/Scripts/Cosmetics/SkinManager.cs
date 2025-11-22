using System;
using System.Collections.Generic;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyTools;
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
            
            public event Action OnLoaded;
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
                
                    Debug.Log($"{skinType} Skin loaded from Resources/{Path}");
#endif
                
                    _dataDic.Add(skinType, data);
                }
                
                _hasLoaded = true;
                OnLoaded?.Invoke();
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

        public SkinType CurrentSkinType { get; private set; }

        public event Action<SkinType> OnSkinChanged;
        
        private void Awake()
        {
            _assetsLoader = new AssetsLoader();
        }

        private void Start()
        {
            SelectSkin(SkinType.Default);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var value = (SkinType)ValueCarousel.GetValue((int)CurrentSkinType, 2);
                
                SelectSkin(value);
            }
        }

        private void SelectSkin(SkinType skinType)
        {
            if(CurrentSkinType == skinType) return;
            
            CurrentSkinType = skinType;
            OnSkinChanged?.Invoke(skinType);
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
        
        #endregion
        
    }

    public enum SkinType
    {
        Default,
        Pizza
    }
}