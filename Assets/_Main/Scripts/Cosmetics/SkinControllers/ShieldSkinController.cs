using System;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class ShieldSkinController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spriteContainer;
        
        private void Start()
        {
            SkinManager.Instance.OnSkinChanged += SkinManager_OnSkinChanged;
            SkinEvents.OnSaveLoaded += InitializeSkin;
        }

        private void InitializeSkin()
        {
            SkinEvents.OnSaveLoaded -= InitializeSkin;
            LoadSkin();
        }

        private void LoadSkin()
        {
            SkinManager_OnSkinChanged(SkinManager.Instance.GetCurrentSkinType());
        }
        
        private void SkinManager_OnSkinChanged(SkinType skinType)
        {
            if (SkinManager.Instance.GetHasData(skinType) == false)
            {
                Debug.LogWarning($"Skin {skinType} not found");
                return;
            }
            
            var data = SkinManager.Instance.GetShieldData(skinType);
            
            spriteRenderer.material = data.Material;
        }
    }
}