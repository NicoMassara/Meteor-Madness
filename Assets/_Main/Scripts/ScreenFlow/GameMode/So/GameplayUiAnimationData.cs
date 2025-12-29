using System;
using MeteorMadness.Animations;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.GameMode
{
    [CreateAssetMenu(fileName = "So_Animation_UI_Gameplay", menuName = "Scriptable Objects/Animation Data/UI/Gameplay", order = 0)]
    public class GameplayUiAnimationData : ScriptableObject
    {
        #region Gameplay UI

        [Serializable]
        public class PanelData : IGameplayPanelData
        {
            [Header("Positions")]
            public AnimationHelper.Direction scoreOffscreenPos = AnimationHelper.Direction.UpLeft;
            public AnimationHelper.Direction pauseOffscreenPos = AnimationHelper.Direction.UpRight;
            [Space]
            [Header("Time Values")]
            [Range(0,1f)]
            public float movementDuration = 0.25f;
            [Range(0,1f)]
            public float finishDelay = 0.5f;
            [Space]
            [Header("Offsets")]
            public Vector2 scoreOffset;
            public Vector2 pauseOffset;

            // Interface properties
            public AnimationHelper.Direction ScoreOffscreenPos => scoreOffscreenPos;
            public AnimationHelper.Direction PauseOffscreenPos => pauseOffscreenPos;
            public Vector2 ScoreOffset => scoreOffset;
            public Vector2 PauseOffset => pauseOffset;
            public float MovementDuration => movementDuration;
            public float FinishDelay => finishDelay;
        }

        [Serializable]
        public class ScoreFinishAdding : IScoreFinishAdding
        {
            [Header("Time Values")]
            public float bounceScale = 1.25f;
            public float bounceDuration = 0.25f;
            public float bounceReturnTime = 0.1f;
            
            // Interface properties
            public float BounceScale => bounceScale;
            public float BounceDuration => bounceDuration;
            public float BounceReturnTime => bounceReturnTime;
        }

        [Serializable]
        public class StreakFailed : IStreakFailed
        {
            [Header("Time Values")]
            public float duration = 0.1f;
            [Space] 
            [Header("Offsets")]
            public float horizontalOffset = 20f;
            [Space] 
            [Header("Misc Values")]
            [SerializeField] int loops = 10;

            public float Duration => duration;
            public float HorizontalOffset => horizontalOffset;
            public int Loops => loops;
        }

        [Serializable]
        private class StreakNotify : IStreakNotify
        {
            [Header("Positions")]
            [SerializeField] private AnimationHelper.Direction startPosition;
            [SerializeField] private AnimationHelper.Direction endPosition;

            [Space()]
            [Header("Time Values")]
            [SerializeField] private float movementDuration;
            [SerializeField] private float stopDuration;

            [Space()]
            [Header("Offsets")]
            [SerializeField] private Vector2 startOffset;
            [SerializeField] private Vector2 endOffset;

            public AnimationHelper.Direction StartPosition => startPosition;
            public AnimationHelper.Direction EndPosition => endPosition;
            public float MovementDuration => movementDuration;
            public float StopDuration => stopDuration;
            public Vector2 StartOffset => startOffset;
            public Vector2 EndOffset => endOffset;
        }



        #endregion

        #region Countdown

        [Serializable]
        public class CountdownUpdate : ICountdownUpdate
        {
            [Header("Time Values")]
            public float fadeDuration = 0.4f;
            public float bounceDuration = 0.4f;
            public float fadeDelay = 0.15f;
            public float finalFadeDuration = 0.45f;
            [Header("Anti Epileptic - Time Values")]
            public float aeScaleDuration = 0.4f;
            public float aeFinishDelay = 0.45f;

            // Interface properties
            public float FadeDuration => fadeDuration;
            public float BounceDuration => bounceDuration;
            public float FadeDelay => fadeDelay;
            public float FinalFadeDuration => finalFadeDuration;
            public float AeScaleDuration => aeScaleDuration;
            public float AeFinishDelay => aeFinishDelay;
        }



        [Serializable]
        public class CountdownFinish : ICountdownFinish
        {
            [Header("Time Values")]
            public float fadeDuration = 0.05f;
            public float bounceDuration = 0.10f;
            public float bounceScale = 1.5f;
            public float fadeDelay = 0.6f;
            public float finalFadeDuration = 0.25f;
            [Space]
            [Header("Anti Epileptic - Time Values")]
            public float aeScaleDuration = 0.4f;
            public float aeBounceScale = 1.5f;
            public float aeScaleDelay = 0.45f;
            public float aeFinishScaleDuration = 0.45f;
            [Space]
            [Header("Global Values")]
            public float finishDelay = 0.5f;

            // Interface properties
            public float FadeDuration => fadeDuration;
            public float BounceDuration => bounceDuration;
            public float BounceScale => bounceScale;
            public float FadeDelay => fadeDelay;
            public float FinalFadeDuration => finalFadeDuration;
            public float AeScaleDuration => aeScaleDuration;
            public float AeBounceScale => aeBounceScale;
            public float AeScaleDelay => aeScaleDelay;
            public float AeFinishScaleDuration => aeFinishScaleDuration;
            public float FinishDelay => finishDelay;
        }

        #endregion
        
        [Header("Countdown")]
        [SerializeField] private CountdownUpdate countdownUpdateData;
        [Space]
        [SerializeField] private CountdownFinish countdownFinishData;
        [Space(2)]
        [Header("Gameplay")]
        [SerializeField] private PanelData gameplayUiOpenData;
        [Space]
        [SerializeField] private PanelData gameplayUiCloseData;
        [Space(2)]
        [Header("Score")]
        [SerializeField] private ScoreFinishAdding scoreFinishAddingData;
        [Header("Streak")]
        [SerializeField] private StreakFailed streakFailedData;
        [SerializeField] private StreakNotify streakNotifyData;

        // Public interface getters
        public IGameplayPanelData GameplayUiOpenData => gameplayUiOpenData;
        public IGameplayPanelData GameplayUiCloseData => gameplayUiCloseData;
        public IScoreFinishAdding ScoreFinishAddingData => scoreFinishAddingData;
        public ICountdownUpdate CountdownUpdateData => countdownUpdateData;
        public ICountdownFinish CountdownFinishData => countdownFinishData;
        public IStreakFailed StreakFailedData => streakFailedData;
        public IStreakNotify StreakNotifyData => streakNotifyData;
    }

}