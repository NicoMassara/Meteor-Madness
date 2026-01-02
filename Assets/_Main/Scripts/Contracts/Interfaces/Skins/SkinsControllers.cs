using System;
using MeteorMadness.GlobalValues;
using UnityEngine;

namespace MeteorMadness.Contracts.Interfaces.Skins
{
    
    public interface IEarthSkin
    {
        public event Action<float> OnHealthChanged;
        public event Action OnShow;
        public event Action OnHide;
    }

    public interface IFlyingObjectSkin
    {
        public event Action OnSkinEnable;
    }
    
    public interface ISkinInformation
    {
        public string NameCode { get; }
        public string DescriptionCode { get; }
        public int UnlockPrice { get; }
    }
    
    public interface ISkinData
    {
        public SkinType SkinType { get; }
        public EarthSkinData EarthData { get; }
        public ShieldSkinData ShieldData { get; }
        public MeteorSkinData MeteorData { get; }
        public CometSkinData CometData { get; }
    }

    public interface ISkinDeathMessage
    {
        public string DeathTitle { get; }
    }
}