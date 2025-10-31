using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities
{
    public class AbilityUIData : ManagedBehavior
    {
        [SerializeField] private AbilityUiPanelSelector uiSelector;
        
        private readonly Color _disabledColor = new Color(1, 1, 1, 0.25f);
        private int _inUseCount = 0;

        private void Awake()
        {
            RestartValues();
        }

        private AbilityUIComponents GetAbilityUIComponents()
        {
            return uiSelector.GetPanelData();
        }

        public void AddAbility(AbilityType ability)
        {
            if(_inUseCount == 3) return;
            
            _inUseCount++;
            var spriteToUse = GetAbilityUIComponents().AbilitySprites[_inUseCount-1];
            spriteToUse.color = GetAbilityColor(ability);
        }

        public void RemoveAbility()
        {
            if(_inUseCount == 0) return;
            
            if (_inUseCount == 1)
            {
                var spriteToUse = GetAbilityUIComponents().AbilitySprites[0];
                spriteToUse.color = GetAbilityColor(AbilityType.None);
            }
            else
            {
                var sprite1 = GetAbilityUIComponents().AbilitySprites[0];
                var sprite2 = GetAbilityUIComponents().AbilitySprites[1];
                var sprite3 = GetAbilityUIComponents().AbilitySprites[2];

                sprite1.color = sprite2.color;
                sprite2.color = sprite3.color;
                sprite3.color = GetAbilityColor(AbilityType.None);
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
            
            if(GetAbilityUIComponents().AbilitySprites == null) return;
            
            for (int i = 0; i < GetAbilityUIComponents().AbilitySprites.Length; i++)
            {
                var spriteToUse = GetAbilityUIComponents().AbilitySprites[i];
                if(spriteToUse == null) continue;
                spriteToUse.color = GetAbilityColor(AbilityType.None);
            }
        }
    }
}