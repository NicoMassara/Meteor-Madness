using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Skins;
using MeteorMadness.Managers.Cosmetics;
using UnityEngine;


namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class EarthSkinController : MonoBehaviour
    {
        [SerializeField] private Renderer spriteRenderer;
        
        private static readonly int HealthInShader = Shader.PropertyToID("_HealthAmount");
        private static readonly int OpacityInShader = Shader.PropertyToID("_Opacity");
        private IEarthSkin _earthHealth;
        private float _healthAmount = 1f;
        private float _opacity = 1f;
        
        private void Awake()
        {
            spriteRenderer.gameObject.SetActive(false);
            _earthHealth = GetComponent<IEarthSkin>();
            
            //Hack
            BootEvents.OnGameLoaded += OnGameLoadedHandler;
        }

        private void OnGameLoadedHandler()
        {
            BootEvents.OnGameLoaded -= OnGameLoadedHandler;
            //
            spriteRenderer.gameObject.SetActive(true);
        }

        private void Start()
        {
            _earthHealth.OnHealthChanged += View_OnHealthChangedHandler;
            _earthHealth.OnHide += OnHideHandler;
            _earthHealth.OnShow += OnShowHandler;
            
            SkinManager.Instance.OnSkinChanged += SkinManager_OnSkinChanged;
            SkinEvents.OnSaveLoaded += InitializeSkin;
        }
        
        private void InitializeSkin()
        {
            SkinEvents.OnSaveLoaded -= InitializeSkin;
            LoadSkin();
            UpdateMaterialHealth();
        }

        private void UpdateMaterialHealth()
        {
            spriteRenderer.material.SetFloat(HealthInShader, _healthAmount);
        }
        
        private void UpdateMaterialOpacity()
        {
            spriteRenderer.material.SetFloat(OpacityInShader, _opacity);
        }
        
        private void LoadSkin()
        {
            SkinManager_OnSkinChanged(SkinManager.Instance.GetCurrentSkinType());
        }
        
        private void OnHideHandler()
        {
            _opacity = 0;
            UpdateMaterialOpacity();
        }
        
        private void OnShowHandler()
        {
            _opacity = 1;
            UpdateMaterialOpacity();
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
            
            spriteRenderer.transform.rotation = data.EarthRotationOffset;
            spriteRenderer.material = SkinManager.Instance.GetEarthData(skinType).Material;
            
            UpdateMaterialHealth();
        }
    }
}