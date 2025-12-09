using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Utilities;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticUiAnimationSelector :  UiComponentsSelector<CosmeticUiAnimationComponents> { }
    
    
    [Serializable]
    public class CosmeticUiAnimationComponents : UiComponentsData,
        CosmeticUiAnimationComponents.IMainPanel
    {
        public interface IMainPanel : IUiAnimationComponent
        {
            public RectTransform MainPanel { get; }
        }
        
        [Space(2)]
        [Header("Main Panel")]
        [SerializeField] private RectTransform mainPanel;
        
        // Menu Panel
        public RectTransform MainPanel => mainPanel;
    }
}