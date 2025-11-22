using System;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    [CreateAssetMenu(fileName = "SO_SkinData_Name", menuName = "Scriptable Objects/Skins/Data", order = 0)]
    public class SkinDataSo : ScriptableObject
    {
        [Header("Type")]
        [SerializeField] private SkinType skinType;
        [Header("Materials")]
        [SerializeField] private EarthSkinData earthData;
        [SerializeField] private MeteorSkinData meteorData;
        [SerializeField] private CometSkinData cometData;
        
        public SkinType SkinType => skinType;

        public EarthSkinData EarthData => earthData;

        public MeteorSkinData MeteorData => meteorData;

        public CometSkinData CometData => cometData;
        
    }

    [Serializable]
    public class SkinData
    {
        [SerializeField] private Material material;
        [Range(0.1f, 10f)] [SerializeField] private float scaleOffset = 1f;
        
        public Material Material => material;

        public Vector3 ScaleOffset => new Vector3(scaleOffset,scaleOffset,0);
    }

    [Serializable]
    public class EarthSkinData : SkinData
    {
        [Range(0,359)]
        [SerializeField] private float rotationOffset;
        public Vector3 EarthRotationOffset => new Vector3(0,0,rotationOffset);
    }
    
    [Serializable]
    public class MeteorSkinData : SkinData
    {

    }
    
    [Serializable]
    public class CometSkinData : SkinData
    {

    }
    
}