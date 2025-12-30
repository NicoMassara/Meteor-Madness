using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Skins;
using MeteorMadness.Managers.Cosmetics;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public abstract class FlyingObjectSkinController<T> : MonoBehaviour
    where T : SkinData
    {
        [SerializeField] private Renderer spriteRenderer;

        private IFlyingObjectSkin _flyingObject;
        protected SkinManager SkinManager { get; private set; }

        private void Awake()
        {
            SkinManager = SkinManager.Instance;
            _flyingObject = GetComponent<IFlyingObjectSkin>();
        }

        private void OnEnable()
        {
            LoadSkin();
            SkinManager.OnSkinChanged += SkinManager_OnSkinChanged;
        }

        private void OnDisable()
        {
            SkinManager.OnSkinChanged -= SkinManager_OnSkinChanged;
        }

        private void LoadSkin()
        {
            SkinManager_OnSkinChanged(SkinManager.GetCurrentSkinType());
        }

        private void SkinManager_OnSkinChanged(SkinType skinType)
        {
            if (SkinManager.GetHasData(skinType) == false)
            {
                Debug.LogWarning($"Skin {skinType} not found");
                return;
            }
            spriteRenderer.material = GetSkinData(skinType).Material;
        }
        
        protected abstract T GetSkinData(SkinType skinType);
    }
}