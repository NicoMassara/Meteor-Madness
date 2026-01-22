using System;
using MeteorMadness.Animations;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel.So
{
    [CreateAssetMenu(fileName = "So_Animation_UI_InputPanel", menuName = "Scriptable Objects/Animation Data/UI/Input Panel", order = 0)]
    public class InputPanelAnimationData : MonoBehaviour
    {
        #region Animation Data

        [Serializable]
        public class PanelAnimData : IPanelData
        {
            [Header("Positions")]
            public AnimationHelper.Direction offscreenDirection = AnimationHelper.Direction.Up;
            [Space]
            [Header("Time Values")]
            [Range(0,1f)]
            public float movementDuration = 0.3f;
            [Header("Offsets")]
            [Space]
            public Vector2 offset;

            // Interface implementation
            public float MovementDuration => movementDuration;
            public AnimationHelper.Direction OffscreenPosition => offscreenDirection;
            
            public Vector2 Offset => offset;
        }
        

        #endregion
        
        [SerializeField] private PanelAnimData panelOpenData;
        [Space]
        [SerializeField] private PanelAnimData panelCloseData;

        public IPanelData PanelOpenData => panelOpenData;
        public IPanelData PanelCloseData => panelCloseData;
    }
}