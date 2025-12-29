using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces.Skins;
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
}