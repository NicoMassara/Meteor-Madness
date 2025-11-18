using System;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldTrail : ManagedBehavior
    {
        [SerializeField] private Material trailMaterial;
        [SerializeField] private Color defaultColor;

        private void Awake()
        {
            SetTrailColor(defaultColor);
        }

        public void SetTrailColor(Color color)
        {
            trailMaterial.SetColor("_TrailColor", color);
        }

        public void SetDefault()
        {
            SetTrailColor(defaultColor);
        }
    }
}