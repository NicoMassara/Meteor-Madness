using MeteorMadness.Animations;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Settings
{
    [CreateAssetMenu(fileName = "So_Animation_UI_Settings", menuName = "Scriptable Objects/Animation Data/UI/Settings")]
    public class SettingsUiAnimationData : ScriptableObject
    {
        [System.Serializable]
        public class PanelOpenData : IPanelData
        {
            [Header("Positions")]
            public AnimationHelper.Direction panelOffscreenPos = AnimationHelper.Direction.Right;
            [Space]
            [Header("Time Values")]
            [Range(0,1f)]
            public float movementDuration = 0.3f;
            [Space]
            [Header("Offsets")]
            public Vector2 offset = Vector2.zero;

            public AnimationHelper.Direction OffscreenPos => panelOffscreenPos;
            public float MovementDuration => movementDuration;
            public Vector2 Offset => offset;
        }
        
        [SerializeField] private PanelOpenData panelOpenData;
        [Space]
        [SerializeField] private PanelOpenData panelCloseData;

        public IPanelData OpenData => panelOpenData;
        public IPanelData CloseData => panelCloseData;
    }

}