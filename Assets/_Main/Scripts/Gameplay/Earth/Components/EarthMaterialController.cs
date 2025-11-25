using System;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMaterialController : ManagedBehavior
    {
        private static readonly int HealthAmount = Shader.PropertyToID("_HealthAmount");
        [SerializeField] private Material surfaceMaterial; 

        private void Start()
        {
            SetMaterialHealth(1f);
        }
        
        public void SetMaterialHealth(float healthAmount)
        {
            surfaceMaterial.SetFloat(HealthAmount, healthAmount);
        }
    }
}