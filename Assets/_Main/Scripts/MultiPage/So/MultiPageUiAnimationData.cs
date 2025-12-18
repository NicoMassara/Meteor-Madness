using System;
using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{

    [CreateAssetMenu(fileName = "So_Animation_MultiPage", menuName = "Scriptable Objects/Animation Data/UI/MultiPage", order = 0)]
    public class MultiPageUiAnimationData : ScriptableObject
    {
        #region Animation Classes

        [Serializable]
        public class PanelAnimData : IPanelData
        {
            [Header("Positions")]
            [Space]
            public AnimationHelper.Direction offscreenDirection = AnimationHelper.Direction.Up;
            [Header("Time Values")]
            public float movementDuration = 0.3f;

            [Space] 
            [Header("Offsets")] 
            public Vector2 offset;

            // Interface properties
            public float MovementDuration => movementDuration;
            public AnimationHelper.Direction OffscreenPosition => offscreenDirection;

            public Vector2 Offset => offset;
        }
        

        #endregion

        [SerializeField] private PanelAnimData panelOpenData;
        [SerializeField] private PanelAnimData panelCloseData;

        #region Public Accessors

        public IPanelData PanelOpenData => panelOpenData;
        public IPanelData PanelCloseData => panelCloseData;

        #endregion
    }

}