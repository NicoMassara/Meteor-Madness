using System;
using UnityEngine;

namespace MeteorMadness.Contracts
{
    [Serializable]
    public class SkinData
    {
        [SerializeField] private Material material;
        [Range(0.1f, 10f)] [SerializeField] private float scaleOffsetX = 1f;
        [Range(0.1f, 10f)] [SerializeField] private float scaleOffsetY = 1f;
        
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

        public Vector3 ScaleOffset => new Vector3(scaleOffsetX,scaleOffsetY,0);
    }
    
    [Serializable]
    public class EarthSkinData : SkinData
    {
        [Range(0,359)]
        [SerializeField] private float rotationOffset;
        public Vector3 EarthRotationOffset => new Vector3(0,0,rotationOffset);
    }
    
    [Serializable]
    public class MeteorSkinData : SkinData { }
    
    [Serializable]
    public class CometSkinData : SkinData { }
    
    [Serializable]
    public class ShieldSkinData : SkinData { }
}