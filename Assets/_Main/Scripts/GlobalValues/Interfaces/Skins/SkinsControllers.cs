using System;
using UnityEngine;

namespace MeteorMadness.GlobalValues.Interfaces.Skins
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
        public IEarthSkinData EarthData { get; }
        public IShieldSkinData ShieldData { get; }
        public IMeteorSkinData MeteorData { get; }
        public ICometSkinData CometData { get; }
    }

    public interface IBaseSkinData
    {
        public Material Material { get; }

        public Vector3 ScaleOffset { get; }
    }

    public interface IEarthSkinData : IBaseSkinData
    {
        public Vector3 EarthRotationOffset { get; }
    }
    public interface IShieldSkinData : IBaseSkinData { }
    public interface IMeteorSkinData : IBaseSkinData { }
    public interface ICometSkinData : IBaseSkinData { }

    public interface ISkinDeathMessage
    {
        public string DeathTitle { get; }
    }
}