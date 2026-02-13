using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.Save;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.Cosmetics
{
    public class SkinChanger : MonoBehaviour
    {
        [SerializeField] private Renderer materialRenderer;

        private int _currentSkin = 0;

        private void Awake()
        {
            SkinManager.LoadInstance();
            
            SaveDataEvents.OnSaveInitialized += BootEvents.InitializeMainSystem;
            
            DataManager.LoadInstance();
        }

        private void Start()
        {
            SkinManager.Instance.OnSkinChanged += OnSkinChanged;
        }

        private void OnDestroy()
        {
            SaveDataEvents.OnSaveInitialized -= BootEvents.InitializeMainSystem;
            SkinManager.Instance.OnSkinChanged -= OnSkinChanged;
        }

        private void OnSkinChanged(SkinType skinType)
        {
            var data = SkinManager.Instance.GetEarthData(skinType);
            
            materialRenderer.transform.rotation = data.EarthRotationOffset;
            materialRenderer.material = data.Material;
        }


        public void ChangeSkin()
        {
            _currentSkin++;

            if (_currentSkin == 6)
            {
                _currentSkin = 1;
            }

            SkinManager.Instance.PreviewSkin(GetSkinType());
        }

        private SkinType GetSkinType()
        {
            return (SkinType)(_currentSkin);
        }

    }
    
    [CustomEditor(typeof(SkinChanger))]
    public class SkinChangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SkinChanger script = (SkinChanger)target;
            if (GUILayout.Button("Change Skin")) script.ChangeSkin();
        }
    }
}