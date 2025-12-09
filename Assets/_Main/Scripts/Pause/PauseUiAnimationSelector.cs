using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseUiAnimationSelector : UiComponentsSelector<PauseUIAnimationComponents> { }
    
    [Serializable]
    public class PauseUIAnimationComponents : UiComponentsData,
        PauseUIAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
        }
        
        [Header("Countdown")]
        [SerializeField] private RectTransform mainPanel;

        public RectTransform MainPanel => mainPanel;
    }
}