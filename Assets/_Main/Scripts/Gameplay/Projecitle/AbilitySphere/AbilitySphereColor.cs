using System;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Utilities;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere
{
    public class AbilitySphereColor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sphereRenderer;
        
        private IAbilitySphereColor _abilitySphereColor;

        private void Awake()
        {
            _abilitySphereColor = GetComponent<IAbilitySphereColor>();

            _abilitySphereColor.OnAbilitySet += OnAbilitySetHandler;
        }

        private void UpdateColor(AbilityType ability)
        {
            var color = AbilityColorHelper.GetColor(ability);
            sphereRenderer.material.SetColor("_AbilityColor", color);
        }

        private void OnAbilitySetHandler(AbilityType ability)
        {
            UpdateColor(ability);
        }
    }
}