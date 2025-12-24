using System;
using _Main.Scripts.Cosmetics.Components;

namespace _Main.Scripts.Cosmetics
{
    public interface ISkinInformation
    {
        public string NameCode { get; }
        public string DescriptionCode { get; }
        public uint UnlockPrice { get; }
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


    public interface ISkinButton
    {
        public SkinType SkinType { get; }
        public event Action<SkinType> OnSelect;
        public SkinSelectButton SetData(string skinName, SkinType skinType);
        public void SetInteractable(bool isInteractable);
        public void RemoveListener();
        public void AddListener();
    }
}