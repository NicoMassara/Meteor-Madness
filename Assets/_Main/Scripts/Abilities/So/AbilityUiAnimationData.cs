using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.Abilities.So
{
    [CreateAssetMenu(fileName = "So_Animation_UI_Ability", menuName = "Scriptable Objects/Animation Data/UI/Ability")]
    public class AbilityUiAnimationData : ScriptableObject
    {
        [System.Serializable]
        public class PanelAnimData : IPanelData
        {
            [Header("Positions")]
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.Down;
            
            [Header("Time Values")]
            [Range(0,1f)]
            public float movementDuration = 0.3f;

            public AnimationHelper.Direction OffscreenPosition => offscreenPosition;
            public float MovementDuration => movementDuration;
        }
        
        [SerializeField] private PanelAnimData panelOpenData;
        [Space]
        [SerializeField] private PanelAnimData panelCloseData;

        public IPanelData OpenData => panelOpenData;
        public IPanelData CloseData => panelCloseData;
    }

}