using System;
using _Main.Scripts.Cosmetics.Components;
using MeteorMadness.Animations;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues;
using MeteorMadness.ScreenFlow.Base;

namespace _Main.Scripts.Cosmetics
{
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