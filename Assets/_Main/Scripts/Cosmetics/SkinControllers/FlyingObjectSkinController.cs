using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public abstract class FlyingObjectSkinController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spriteContainer;

        private IFlyingObjectSkin _flyingObject;
        protected SkinManager SkinManager { get; private set; }

        private void Awake()
        {
            SkinManager = SkinManager.Instance;
            
            _flyingObject = GetComponent<IFlyingObjectSkin>();
        }

        private void Start()
        {
            //LoadSkin();
            
            _flyingObject.OnSkinEnable += LoadSkin;
            
            SkinManager.OnSkinChanged += SkinManager_OnSkinChanged;
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
            
            var data = GetSkinData(skinType);
            
            spriteRenderer.material = data.Material;
            spriteRenderer.transform.localScale = data.ScaleOffset;
        }
        
        protected abstract SkinData GetSkinData(SkinType skinType);
    }
}