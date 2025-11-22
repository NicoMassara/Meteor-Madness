using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class EarthSkinController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spriteContainer;
        
        private static readonly int HealthInShader = Shader.PropertyToID("_HealthAmount");
        private IEarthSkin _earthHealth;
        private float _healthAmount = 1f;
        
        private void Awake()
        {
            _earthHealth = GetComponent<IEarthSkin>();
        }

        private void Start()
        {
            _earthHealth.OnHealthChanged += View_OnHealthChangedHandler;
            
            LoadSkin();
            SkinManager.Instance.OnSkinChanged += SkinManager_OnSkinChanged;
            
            UpdateMaterialHealth();
        }

        private void UpdateMaterialHealth()
        {
            spriteRenderer.material.SetFloat(HealthInShader, _healthAmount);
        }
        
        private void LoadSkin()
        {
            SkinManager_OnSkinChanged(SkinManager.Instance.CurrentSkinType);
        }

        private void View_OnHealthChangedHandler(float health)
        {
            _healthAmount = health;
            UpdateMaterialHealth();
        }

        private void SkinManager_OnSkinChanged(SkinType skinType)
        {
            if (SkinManager.Instance.GetHasData(skinType) == false)
            {
                Debug.LogWarning($"Skin {skinType} not found");
                return;
            }
            
            var data = SkinManager.Instance.GetEarthData(skinType);
            
            spriteRenderer.material = data.Material;
            spriteContainer.rotation = Quaternion.Euler(data.EarthRotationOffset);
            spriteRenderer.transform.localScale = data.ScaleOffset;
            
            UpdateMaterialHealth();
        }
        
    }
}