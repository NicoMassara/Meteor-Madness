using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.Abilities
{
    public class AbilityUIData : ManagedBehavior
    {
        [SerializeField] private AbilityUiPanelSelector uiSelector;
        private static readonly int AbilityColor = Shader.PropertyToID("_AbilityColor");
        private static readonly int AbilityColorToggle = Shader.PropertyToID("_AbilityColorToggle");

        private int _inUseCount = 0;

        private void Awake()
        {
            InitializeSprites();
            RestartValues();
        }

        private AbilityUIComponents GetAbilityUIComponents()
        {
            return uiSelector.GetPanelData();
        }

        public void AddAbility(AbilityType ability)
        {
            if (_inUseCount == 3) return;

            _inUseCount++;
            var spriteToUse = GetAbilityUIComponents().AbilitySprites[_inUseCount - 1];
            SetSpriteColor(spriteToUse, GetAbilityColor(ability));
        }

        public void RemoveAbility()
        {
            if (_inUseCount == 0) return;

            if (_inUseCount == 1)
            {
                var spriteToUse = GetAbilityUIComponents().AbilitySprites[0];
                DisableSpriteColor(spriteToUse);
            }
            else
            {
                if (_inUseCount == 1)
                {
                    var sprite1 = GetAbilityUIComponents().AbilitySprites[0];
                    DisableSpriteColor(sprite1);
                }
                else if (_inUseCount == 2)
                {
                    var sprite1 = GetAbilityUIComponents().AbilitySprites[0];
                    var sprite2 = GetAbilityUIComponents().AbilitySprites[1];
                    SetSpriteColor(sprite1, GetSpriteColor(sprite2));
                    DisableSpriteColor(sprite2);
                }
                else if (_inUseCount == 3)
                {
                    var sprite1 = GetAbilityUIComponents().AbilitySprites[0];
                    var sprite2 = GetAbilityUIComponents().AbilitySprites[1];
                    var sprite3 = GetAbilityUIComponents().AbilitySprites[2];
                    SetSpriteColor(sprite1, GetSpriteColor(sprite2));
                    SetSpriteColor(sprite2, GetSpriteColor(sprite3));
                    DisableSpriteColor(sprite3);
                }
            }

            _inUseCount--;
        }

        private Color GetAbilityColor(AbilityType ability)
        {
            return AbilityDataGetter.GetColor(ability);
        }

        public void RestartValues()
        {
            _inUseCount = 0;

            if (GetAbilityUIComponents().AbilitySprites == null) return;

            foreach (var spriteToUse in GetAbilityUIComponents().AbilitySprites)
            {
                if (spriteToUse == null) continue;
                DisableSpriteColor(spriteToUse);
            }
        }

        private void InitializeSprites()
        {
            if (GetAbilityUIComponents().AbilitySprites == null) return;

            foreach (var spriteToUse in GetAbilityUIComponents().AbilitySprites)
            {
                if (spriteToUse == null) continue;
                InitializeMaterial(spriteToUse);
            }
        }

        private Color GetSpriteColor(Image sprite)
        {
           return sprite.material.GetColor(AbilityColor);
        }

        private void SetSpriteColor(Image sprite, Color colorToSet)
        {
            sprite.material.SetFloat(AbilityColorToggle, 1);
            sprite.material.SetColor(AbilityColor, colorToSet);
        }

        private void DisableSpriteColor(Image sprite)
        {
            sprite.material.SetFloat(AbilityColorToggle, 0);
        }
        

        private void InitializeMaterial(Image sprite)
        {
            var matInstance = Instantiate(sprite.material);
            sprite.material = matInstance;
        }

    }
}