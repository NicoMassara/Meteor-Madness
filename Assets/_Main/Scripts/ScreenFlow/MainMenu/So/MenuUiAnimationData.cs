using System;
using MeteorMadness.Animations;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Menu
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "So_Animation_UI_Menu", menuName = "Scriptable Objects/Animation Data/UI/Menu", order = 0)]
    public class MenuUiAnimationData : ScriptableObject
    {
        [Header("Menu")]
        [SerializeField] private MenuPanelFirstOpenAnimData menuFirstOpenData;
        [Space]
        [SerializeField] private MenuPanelAnimData menuOpenData;
        [Space]
        [SerializeField] private MenuPanelAnimData menuCloseData;
        [Space(2)]
        [Header("Lore")]
        [SerializeField] private BaseAnimData loreOpenData;
        [Space]
        [SerializeField] private BaseAnimData loreCloseData;
        [Space(2)]
        [Header("Tutorial")]
        [SerializeField] private BaseAnimData tutorialOpenData;
        [Space]
        [SerializeField] private BaseAnimData tutorialCloseData;
        [Space(2)]
        [Header("Credits")]
        [SerializeField] private BaseAnimData creditsOpenData;
        [Space]
        [SerializeField] private BaseAnimData creditsCloseData;
        [Header("First Game")]
        [SerializeField] private BaseAnimData firstGameOpenData;
        [Space]
        [SerializeField] private BaseAnimData firstGameCloseData;

        #region Private Classes

        [Serializable]
        public class MenuPanelFirstOpenAnimData : IManuFirstOpenData
        {
            [Header("Positions")] 
            public AnimationHelper.Direction gameTitleOffscreenPos = AnimationHelper.Direction.Up;
            public AnimationHelper.Direction leftButtonsOffscreenPos = AnimationHelper.Direction.Left;
            public AnimationHelper.Direction rightButtonsOffscreenPos = AnimationHelper.Direction.Right;
            [Space] 
            [Header("Time Values")] 
            [Range(0,1)]
            public float showTitleDelay;
            [Range(0,1)]
            public float titleMovementDuration;
            [Range(0,1)]
            public float buttonsMovementDuration;
            [Range(0,1)]
            public float showButtonsDelay;
            public float finishDelay;
            [Space] 
            [Header("Offsets")]
            public Vector2 titleOffset;
            public Vector2 leftButtonsOffset;
            public Vector2 rightButtonsOffset;
            
            public AnimationHelper.Direction GameTitleOffscreenPos => gameTitleOffscreenPos;
            public AnimationHelper.Direction LeftButtonsOffscreenPos => leftButtonsOffscreenPos;
            public AnimationHelper.Direction RightButtonsOffscreenPos => rightButtonsOffscreenPos;
            public Vector2 TitleOffset => titleOffset;
            public Vector2 LeftButtonsOffset => leftButtonsOffset;
            public Vector2 RightButtonsOffset => rightButtonsOffset;
            public float ShowTitleDelay => showTitleDelay;
            public float TitleMovementDuration => titleMovementDuration;
            public float ButtonsMovementDuration => buttonsMovementDuration;
            public float ShowButtonsDelay => showButtonsDelay;
            public float FinishDelay => finishDelay;
        }
        
        [Serializable]
        public class MenuPanelAnimData : IMenuPanelData
        {
            [Header("Positions")] 
            public AnimationHelper.Direction gameTitleOffscreenPos = AnimationHelper.Direction.Up;
            public AnimationHelper.Direction leftButtonsOffscreenPos = AnimationHelper.Direction.Left;
            public AnimationHelper.Direction rightButtonsOffscreenPos = AnimationHelper.Direction.Right; 
            [Space]
            [Header("Time Values")]
            [Range(0,1)]
            public float movementDuration = 0.25f;
            public float finishDelay = 0.25f;
            [Space]
            [Header("Offsets")]
            public Vector2 titleOffset;
            public Vector2 leftButtonsOffset;
            public Vector2 rightButtonsOffset;
            
            public AnimationHelper.Direction GameTitleOffscreenPos => gameTitleOffscreenPos;
            public AnimationHelper.Direction LeftButtonsOffscreenPos => leftButtonsOffscreenPos;
            public AnimationHelper.Direction RightButtonsOffscreenPos => rightButtonsOffscreenPos;

            public Vector2 TitleOffset => titleOffset;
            public Vector2 LeftButtonsOffset => leftButtonsOffset;
            public Vector2 RightButtonsOffset => rightButtonsOffset;
            public float FinishDelay => finishDelay;
            
            public float MovementDuration => movementDuration;
        }

        [Serializable]
        public class BaseAnimData : IBasePanelData
        {
            [Header("Positions")] 
            public AnimationHelper.Direction offscreenDirection = AnimationHelper.Direction.Left;
            [Space]
            [Header("Time Values")]
            [Range(0, 1)]
            public float movementDuration = 0.3f;
            [Space] [Header("Offsets")]
            public Vector2 offset;

            public float MovementDuration => movementDuration;
            public AnimationHelper.Direction OffscreenDirection => offscreenDirection;
            public Vector2 Offset => offset;
        }


        #endregion
        
        public IManuFirstOpenData MenuFirstOpenData => menuFirstOpenData;
        public IMenuPanelData MenuOpenData => menuOpenData;
        public IMenuPanelData MenuCloseData => menuCloseData;
        public IBasePanelData LoreOpenData => loreOpenData;
        public IBasePanelData LoreCloseData => loreCloseData;
        public IBasePanelData TutorialOpenData => tutorialOpenData;
        public IBasePanelData TutorialCloseData => tutorialCloseData;
        public IBasePanelData CreditsOpenData => creditsOpenData;
        public IBasePanelData CreditsCloseData => creditsCloseData;
        public BaseAnimData FirstGameOpenData => firstGameOpenData;
        public BaseAnimData FirstGameCloseData => firstGameCloseData;
    }

}