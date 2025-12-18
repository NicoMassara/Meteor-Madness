using System;
using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.Defeat.So
{
    [CreateAssetMenu(fileName = "So_Animation_UI_Defeat", menuName = "Scriptable Objects/Animation Data/UI/Defeat", order = 0)]
    public class DefeatUiAnimationData : ScriptableObject
    {
        #region Animation Data

        #region Panel
        [Serializable]
        public class PanelOpenAnimData : IPanelOpen
        {
            [Header("Time Values")]
            public float fadeInDelay = 0.5f;
            public float fadeIntensity = 0.5f;
            public float fadeInDuration = 1.5f;
            public float scaleDelay = 0.5f;
            public float scaleDuration = 0.75f;
            public float bounceDelay = 0.05f;

            [Range(1.01f, 2f)]
            public float bounceScale = 1.25f;

            public float bounceDuration = 0.1f;
            public float bounceReturnTime = 0.32f;
            public float finishDelay = 0.5f;

            // Interface properties
            public float FadeInDelay => fadeInDelay;
            public float FadeIntensity => fadeIntensity;
            public float FadeInDuration => fadeInDuration;
            public float ScaleDelay => scaleDelay;
            public float ScaleDuration => scaleDuration;
            public float BounceDelay => bounceDelay;
            public float BounceScale => bounceScale;
            public float BounceDuration => bounceDuration;
            public float BounceReturnTime => bounceReturnTime;
            public float FinishDelay => finishDelay;
        }
        
        
        [Serializable]
        public class PanelCloseAnimData : IPanelClose
        {
            [Header("Positions")]
            public AnimationHelper.Direction currentScoreOffscreenPosition = AnimationHelper.Direction.UpRight;
            public AnimationHelper.Direction highScoreOffscreenPosition = AnimationHelper.Direction.UpLeft;
            public AnimationHelper.Direction titleOffscreenPosition = AnimationHelper.Direction.Up;
            public AnimationHelper.Direction buttonsOffscreenPosition = AnimationHelper.Direction.Down;
            
            [Space]
            [Header("Time Values")]
            public float movementDelay = 0.5f;
            public float movementDuration = 0.25f;
            public float fadeDelay = 0.25f;
            public float backgroundFadeDuration = 0.3f;
            
            [Space]
            [Header("Offsets")]
            public Vector2 buttonsOffScreenOffset;
            public Vector2 titleOffScreenOffset;

            // Interface properties
            public float MovementDelay => movementDelay;
            public float MovementDuration => movementDuration;
            public AnimationHelper.Direction CurrentScoreOffscreenPosition => currentScoreOffscreenPosition;
            public AnimationHelper.Direction HighScoreOffscreenPosition => highScoreOffscreenPosition;
            public AnimationHelper.Direction TitleOffscreenPosition => titleOffscreenPosition;
            public AnimationHelper.Direction ButtonsOffscreenPosition => buttonsOffscreenPosition;
            public float FadeDelay => fadeDelay;
            public float BackgroundFadeDuration => backgroundFadeDuration;
            public Vector2 ButtonsOffScreenOffset => buttonsOffScreenOffset;
            public Vector2 TitleOffScreenOffset => titleOffScreenOffset;
        }
        
        #endregion
        #region Score
        
        [Serializable]
        public class CurrentScoreIncrement : ICurrentScoreIncrement
        {
            [Header("Positions")]
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.UpRight;
            [Space]
            [Header("Time Values")]
            public float moveDuration = 0.3f;
            public float finishDelay = 0.5f;

            // Interface properties
            public AnimationHelper.Direction OffscreenPosition => offscreenPosition;
            public float MoveDuration => moveDuration;
            public float FinishDelay => finishDelay;
        }

        
 
        
        [Serializable]
        public class HighScoreIncrement : IHighScoreIncrement
        {
            [Header("Positions")]
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.UpLeft;

            [Space]
            [Header("Time Values")]
            public float moveDuration = 0.3f;
            public float finishDelay = 0.5f;

            [Space] 
            [Header("New High Score Values")]
            public float scaleDuration = 0.75f;
            public float bounceDelay = 0.05f;
            public float bounceScale = 1.25f;
            public float bounceDuration = 0.1f;
            public float bounceReturnTime = 0.35f;
            public float newScoreTextDelay = 0.25f;
            public float newScoreBounceScale = 2f;
            public float newScoreBounceDuration = 0.5f;
            public float newScoreBounceReturnTime = 0.2f;

            // Interface properties
            public AnimationHelper.Direction OffscreenPosition => offscreenPosition;
            public float MoveDuration => moveDuration;
            public float FinishDelay => finishDelay;

            public float ScaleDuration => scaleDuration;
            public float BounceDelay => bounceDelay;
            public float BounceScale => bounceScale;
            public float BounceDuration => bounceDuration;
            public float BounceReturnTime => bounceReturnTime;

            public float NewScoreTextDelay => newScoreTextDelay;
            public float NewScoreBounceScale => newScoreBounceScale;
            public float NewScoreBounceDuration => newScoreBounceDuration;
            public float NewScoreBounceReturnTime => newScoreBounceReturnTime;
        }

        #endregion

        #region Buttons

        [Serializable]
        public class ButtonsOpen : IButtonsOpen
        {
            [Header("Positions")]
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.Down;
            [Space]
            [Header("Time Values")]
            public Vector2 offscreenOffset;
            [Range(0,1)]
            public float moveDuration = 0.3f;
            [Range(0,1)]
            public float finishDelay = 0.5f;

            // Interface properties
            public AnimationHelper.Direction OffscreenPosition => offscreenPosition;
            public Vector2 OffscreenOffset => offscreenOffset;
            public float MoveDuration => moveDuration;
            public float FinishDelay => finishDelay;
        }

        #endregion
        
        #endregion
        
        [Header("Main Panel")]
        [SerializeField] private PanelOpenAnimData panelOpenData;
        [Space]
        [SerializeField] private PanelCloseAnimData panelCloseData;
        [Space(2)]
        [Header("Score")]
        [SerializeField] private CurrentScoreIncrement currentScoreData;
        [Space]
        [SerializeField] private HighScoreIncrement highScoreData;
        [Space(2)]
        [Header("Buttons")]
        [SerializeField] private ButtonsOpen buttonsOpenData;

        public IPanelOpen PanelOpenData => panelOpenData;
        public IPanelClose PanelCloseData => panelCloseData;
        public ICurrentScoreIncrement CurrentScoreData => currentScoreData;
        public IHighScoreIncrement HighScoreData => highScoreData;
        public IButtonsOpen ButtonsOpenData => buttonsOpenData;
    }
}