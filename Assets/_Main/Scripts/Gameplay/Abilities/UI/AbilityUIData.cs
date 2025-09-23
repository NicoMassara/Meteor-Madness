using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.Abilities
{
    public class AbilityUIData : ManagedBehavior
    {
        [SerializeField] private Image[] abilitySprite;
        
        private readonly Color _disabledColor = new Color(1, 1, 1, 0.25f);
        private int _inUseCount = 0;

        private void Awake()
        {
            for (int i = 0; i < abilitySprite.Length; i++)
            {
                var spriteToUse = abilitySprite[i];
                spriteToUse.color = GetAbilityColor(AbilityType.None);
            }
        }

        public void AddAbility(AbilityType ability)
        {
            if(_inUseCount == 3) return;
            
            _inUseCount++;
            var spriteToUse = abilitySprite[_inUseCount-1];
            spriteToUse.color = GetAbilityColor(ability);
        }

        public void RemoveAbility()
        {
            if(_inUseCount == 0) return;
            
            if (_inUseCount == 1)
            {
                var spriteToUse = abilitySprite[0];
                spriteToUse.color = GetAbilityColor(AbilityType.None);
            }
            else
            {
                var sprite1 = abilitySprite[0];
                var sprite2 = abilitySprite[1];
                var sprite3 = abilitySprite[2];

                sprite1.color = sprite2.color;
                sprite2.color = sprite3.color;
                sprite3.color = GetAbilityColor(AbilityType.None);
            }

            _inUseCount--;
        }

        private Color GetAbilityColor(AbilityType ability)
        {
            var newColor = ability switch
            {
                AbilityType.None => _disabledColor,
                AbilityType.SuperShield => Color.cyan,
                AbilityType.SlowMotion => Color.yellow,
                AbilityType.Health => Color.red,
                _ => Color.clear
            };
            
            return newColor;
        }
    }
}