using System;
using _Main.Scripts.Cosmetics.Components;
using _Main.Scripts.MyAnimations;

namespace _Main.Scripts.Cosmetics
{
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


    public interface ISkinButton
    {
        public SkinType SkinType { get; }
        public event Action<SkinType> OnSelect;
        public SkinSelectButton SetData(string skinName, SkinType skinType);
        public void SetInteractable(bool isInteractable);
        public void RemoveListener();
        public void AddListener();
    }
    
    // Animations 

    #region Animations
    
    public interface IPanelData : IUiAnimationData
    {
        public AnimationHelper.Direction OffScreenPos { get; }
        public float MovementDuration { get; }
    }

    public interface IPanelCloseData : IUiAnimationData
    {
        public AnimationHelper.Direction OffScreenPos { get; }
        public float MovementDuration { get; }
    }
    
    #endregion
}