using System;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMaterialController : ManagedBehavior
    {
        [SerializeField] private Material surfaceMaterial; 
        [SerializeField] private Material atmosphereMaterial;

        private void Start()
        {
            SetMaterialHealth(1f);
        }
        
        public void SetMaterialHealth(float healthAmount)
        {
            surfaceMaterial.SetFloat("_HealthAmount", healthAmount);
            atmosphereMaterial.SetFloat("_HealthAmount", healthAmount);
        }
    }
}