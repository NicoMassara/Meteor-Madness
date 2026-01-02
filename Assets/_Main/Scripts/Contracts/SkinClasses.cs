using System;
using UnityEngine;

namespace MeteorMadness.Contracts
{
    [Serializable]
    public class SkinData
    {
        [SerializeField] private Material material;
        
        private Material _runtimeMaterial;
        
        private Material GetRuntimeMaterial()
        {
            if (!_runtimeMaterial)
            {
                _runtimeMaterial = new Material(material);
            }
            
            return _runtimeMaterial;
        }
        
        public Material Material => material;
    }
    
    [Serializable]
    public class EarthSkinData : SkinData
    {
        [Range(0,359)]
        [SerializeField] private float rotationOffset;
        public Quaternion EarthRotationOffset => Quaternion.Euler(new Vector3(0,0,rotationOffset));
    }
    
    [Serializable]
    public class MeteorSkinData : SkinData { }
    
    [Serializable]
    public class CometSkinData : SkinData { }
    
    [Serializable]
    public class ShieldSkinData : SkinData { }
}