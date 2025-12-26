using System;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    [CreateAssetMenu(fileName = "SO_SkinData_Name", menuName = "Scriptable Objects/Skins/Data", order = 0)]
    public class SkinDataSo : ScriptableObject, ISkinInformation, ISkinData, ISkinDeathMessage
    {
        [Header("Type")]
        [SerializeField] private SkinType skinType;
        [Header("Information")]
        [Tooltip("Only Needs the name in Localization/Skins/xx/skinData.json")]
        [SerializeField] private string localizationCode;
        [Min(0)]
        [SerializeField] private int unlockPrice;
        [Header("Materials")]
        [SerializeField] private EarthSkinData earthData;
        [SerializeField] private ShieldSkinData shieldData;
        [SerializeField] private MeteorSkinData meteorData;
        [SerializeField] private CometSkinData cometData;

        private string BaseLocalizationCode => $"Cosmetic.SkinData.{localizationCode}";
        
        public SkinType SkinType => skinType;

        public string NameCode => $"{BaseLocalizationCode}.Name";

        public string DescriptionCode => $"{BaseLocalizationCode}.Description";
        public string DeathTitle => $"{BaseLocalizationCode}.DeathTitle";
        public int UnlockPrice => unlockPrice;

        public EarthSkinData EarthData => earthData;

        public ShieldSkinData ShieldData => shieldData;
        public MeteorSkinData MeteorData => meteorData;


        public CometSkinData CometData => cometData;
    }

    [Serializable]
    public class SkinData
    {
        [SerializeField] private Material material;
        [Range(0.1f, 10f)] [SerializeField] private float scaleOffsetX = 1f;
        [Range(0.1f, 10f)] [SerializeField] private float scaleOffsetY = 1f;
        
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
    public class MeteorSkinData : SkinData
    {

    }
    
    [Serializable]
    public class CometSkinData : SkinData
    {

    }
    
    [Serializable]
    public class ShieldSkinData : SkinData
    {

    }
    
}