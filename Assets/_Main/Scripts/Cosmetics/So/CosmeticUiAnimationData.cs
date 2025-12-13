using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    using UnityEngine;
    using System;

    [CreateAssetMenu(fileName = "So_Animation_UI_Cosmetics", menuName = "Scriptable Objects/Animation Data/UI/Cosmetics", order = 0)]
    public class CosmeticUiAnimationData : ScriptableObject
    {
        #region Panel

        [Serializable]
        public class PanelAnimData : IPanelData
        {
            [Header("Positions")]
            public AnimationHelper.Direction offScreenPos = AnimationHelper.Direction.Left;
            [Space]
            [Header("Time Values")]
            [Range(0,1)]
            public float movementDuration = 0.5f;

            // Interface properties
            public AnimationHelper.Direction OffScreenPos => offScreenPos;
            public float MovementDuration => movementDuration;
        }
        
        [SerializeField] private PanelAnimData panelOpenData;
        [Space]
        [SerializeField] private PanelAnimData panelCloseData;

        #endregion
        
        public IPanelData PanelOpenData => panelOpenData;
        public IPanelData PanelCloseData => panelCloseData;
    }
}
