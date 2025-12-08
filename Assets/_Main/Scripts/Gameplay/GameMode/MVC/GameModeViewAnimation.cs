using System;
using _Main.Scripts.Managers;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>,
        GameModeViewAnimation.IGameModeViewAnimation
    {
        public interface IGameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnUiClosed;
        }
        
        #region Animation Data

        #region Gameplay UI
        
        [Serializable]
        private class GameplayUiOpenData : UiAnimationData
        {
            [Header("Position")]
            public AnimationHelper.Direction scoreOffscreenPos = AnimationHelper.Direction.UpLeft;
            public AnimationHelper.Direction pauseOffscreenPos = AnimationHelper.Direction.UpRight;
            [Header("Offset")]
            public Vector2 scoreOffset;
            public Vector2 pauseOffset;
            [Space]
            [Header("Time Values")] 
            public float movementDuration = 0.25f;

        }
        
        [SerializeField] private GameplayUiOpenData gameplayUiOpenData;
        
        [Serializable]
        private class GameplayUiCloseData : UiAnimationData
        {
            [Header("Position")]
            public AnimationHelper.Direction scoreOffscreenPos = AnimationHelper.Direction.UpLeft;
            public AnimationHelper.Direction pauseOffscreenPos = AnimationHelper.Direction.UpRight;
            [Header("Offset")]
            public Vector2 scoreOffset;
            public Vector2 pauseOffset;
            [Space]
            [Header("Time Values")] 
            public float movementDuration = 0.25f;
            public float finishDelay = 0.5f;
        }
        
        [SerializeField] private GameplayUiCloseData gameplayUiCloseData;
        
        [Serializable]
        private class ScoreFinishAdding : UiAnimationData
        {
            public float bounceScale = 1.25f;
            public float bounceDuration = 0.25f;
            public float bounceReturnTime = 0.1f;
        }
        
        [SerializeField] private ScoreFinishAdding scoreFinishAddingData;
        
        #endregion

        #region Countdown

        [Serializable]
        private class CountdownUpdate : UiAnimationData
        {
            [Header("Time Values")]
            public float fadeDuration = 0.4f;
            public float bounceDuration = 0.4f;
            public float fadeDelay = 0.15f;
            public float finalFadeDuration = 0.45f;
            [Header("Anti Epileptic - Time Values")]
            public float aeScaleDuration = 0.4f;
            public float aeFinishDelay = 0.45f;

        }
        
        [SerializeField] private CountdownUpdate countdownUpdateData;
        
        [Serializable]
        private class CountdownFinish : UiAnimationData
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
            
        }
        
        [SerializeField] private CountdownFinish countdownFinishData;

        #endregion
        
        #endregion
        
        #region Animations

        private static class TextTween
        {
            public static Tweener Fade(RectTransform text, float duration, bool doesFadeIn = true)
            {
                return Fade(text.GetComponent<TMP_Text>(), duration, doesFadeIn);
            }

            public static Tweener Fade(TMP_Text text, float duration, bool doesFadeIn = true)
            {
                text.gameObject.SetActive(true);

                float startAlpha = doesFadeIn ? 0f : text.color.a;
                float endAlpha = doesFadeIn ? 1f : 0f;

                if (doesFadeIn)
                {
                    Color c = text.color;
                    c.a = 0f;
                    text.color = c;
                }

                return text.DOFade(endAlpha, duration);
            }
        }
        
        #region Gameplay

        private class Animation_UI_Open : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel,GameplayUiOpenData>
        {
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _pausePanel;

            public Animation_UI_Open(GameModeUIAnimationComponents.IGameplayPanel components,
                GameplayUiOpenData animationData)
                : base(components, animationData)
            {
                _scorePanel =
                    new AnimationHelper.PanelPosition(UIComponents.ScorePanel,
                        AnimationData.scoreOffscreenPos, 
                        AnimationData.scoreOffset);
                _pausePanel =
                    new AnimationHelper.PanelPosition(UIComponents.PauseButton, 
                        AnimationData.pauseOffscreenPos,
                        AnimationData.pauseOffset);
            }

            protected override void Initialize()
            {
                UIComponents.ScorePanel.anchoredPosition = _scorePanel.OffScreenPos;
                UIComponents.PauseButton.anchoredPosition = _pausePanel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                Initialize();
                
                return DOTween.Sequence()
                    .AppendCallback(()=>UIComponents.GameplayPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scorePanel.StartPos, AnimationData.movementDuration))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pausePanel.StartPos, AnimationData.movementDuration));
            }
        }
        private class Animation_UI_Close : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel,GameplayUiCloseData>
        {
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _pausePanel;


            public Animation_UI_Close(GameModeUIAnimationComponents.IGameplayPanel components, GameplayUiCloseData animationData)
                : base(components, animationData)
            {
                _scorePanel =
                    new AnimationHelper.PanelPosition(UIComponents.ScorePanel, AnimationData.scoreOffscreenPos, AnimationData.scoreOffset);
                _pausePanel =
                    new AnimationHelper.PanelPosition(UIComponents.PauseButton, AnimationData.pauseOffscreenPos, AnimationData.pauseOffset);
            }
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scorePanel.OffScreenPos, AnimationData.movementDuration))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pausePanel.OffScreenPos, AnimationData.movementDuration))
                    .AppendInterval(AnimationData.finishDelay)
                    .AppendCallback(()=> UIComponents.GameplayPanel.gameObject.SetActive(false));
            }
        }
        private class Animation_Score_FinishAdding : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel,ScoreFinishAdding>
        {
            public Animation_Score_FinishAdding(GameModeUIAnimationComponents.IGameplayPanel components, ScoreFinishAdding animationData)
                : base(components, animationData) { }
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.ScorePanel.DOScale(AnimationData.bounceScale, AnimationData.bounceDuration))
                    .Append(UIComponents.ScorePanel.DOScale(1, AnimationData.bounceReturnTime));
            }
        }

        #endregion
        
        #region Countdown
        
        private class Animation_Countdown_Update : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel,CountdownUpdate>
        {
            public Animation_Countdown_Update(GameModeUIAnimationComponents.ICountdownPanel components, CountdownUpdate animationData) 
                : base(components, animationData) { }

            protected override Sequence CreateAnimation()
            {
                var sequence = DOTween.Sequence()
                    .Append(UIComponents.CountdownText.DOScale(0, 0));
                
                if (GameManager.Instance.AntiEpileptic == false)
                {
                    sequence
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.fadeDuration, true))
                        .Join(UIComponents.CountdownText.DOScale(1, AnimationData.bounceDuration))
                        .AppendInterval(AnimationData.fadeDuration)
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.finalFadeDuration, false))
                        ;
                }
                else
                {
                    sequence
                        .Join(UIComponents.CountdownText.DOScale(1, AnimationData.aeScaleDuration))
                        .AppendInterval(AnimationData.aeFinishDelay)
                        ;
                }
                
                return sequence;
            }
        }
        private class Animation_Countdown_Finish : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel,CountdownFinish>
        {
            public Animation_Countdown_Finish(GameModeUIAnimationComponents.ICountdownPanel components, CountdownFinish animationData)
                : base(components, animationData) { }
            
            protected override void RestartValues()
            {
                UIComponents.CountdownPanel.gameObject.SetActive(false);
            }

            protected override Sequence CreateAnimation()
            {
                var sequence = DOTween.Sequence();
                
                if (GameManager.Instance.AntiEpileptic == false)
                {
                    sequence
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.fadeDuration, true))
                        .Append(UIComponents.CountdownText.DOScale(AnimationData.bounceScale, AnimationData.bounceDuration))
                        .AppendInterval(AnimationData.fadeDelay)
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.finalFadeDuration, false));
                }
                else
                {
                    sequence
                        .Append(UIComponents.CountdownText.DOScale(AnimationData.aeBounceScale, AnimationData.aeScaleDuration))
                        .AppendInterval(AnimationData.aeScaleDelay)
                        .Append(UIComponents.CountdownText.DOScale(0, AnimationData.aeFinishScaleDuration));
                }

                sequence
                    .AppendInterval(AnimationData.finishDelay);
                
                return sequence;
            }
        }
        
        #endregion

        #endregion
        
        private IUIAnimator _animationCountdownUpdate;
        private IUIAnimator _animationCountdownFinish;
        private IUIAnimator _animationUiOpen;
        private IUIAnimator _animationUiClose;
        private IUIAnimator _animationFinishAddingScore;

        #region IGameModeViewAnimation

        public event Action OnUiClosed;

        #endregion
        
        private void Awake()
        {
            _animationCountdownUpdate = new Animation_Countdown_Update(UIComponents, countdownUpdateData);
            _animationCountdownFinish = new Animation_Countdown_Finish(UIComponents, countdownFinishData);
            //
            _animationUiOpen = new Animation_UI_Open(UIComponents, gameplayUiOpenData);
            _animationUiClose = new Animation_UI_Close(UIComponents, gameplayUiCloseData);
            //
            _animationFinishAddingScore = new Animation_Score_FinishAdding(UIComponents, scoreFinishAddingData);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Countdown ===//
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown((int)args[0]);
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleUpdateCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.FinishCountdown:
                    HandleFinishCountdown();
                    break;
                
                //=== Gameplay ===//
                case GameModeObserverMessage.EnableGameplayUI:
                    HandleEnableGameplayUI();
                    break;
                case GameModeObserverMessage.DisableGameplayUI:
                    HandleDisableGameplayUI();
                    break;
                case GameModeObserverMessage.FinishAddingPoints:
                    HandleFinishAddingPoints();
                    break;
                
                //=== Internal Level ===//
                case GameModeObserverMessage.UpdateGameLevel:
                    HandleUpdateGameLevel((int)args[0]);
                    break;
            }
        }

        #region Countdown

        private void HandleStartCountdown(int delay)
        {
            UIComponents.CountdownPanel.gameObject.SetActive(true);
        }
        
        private void HandleUpdateCountdown(float time)
        {
            if (time > 1)
            {
                PlayAnimation(_animationCountdownUpdate);
            }
            else if(time > 0)
            {
                PlayAnimation(_animationCountdownFinish);
            }
        }
        
        private void HandleFinishCountdown()
        {

        }

        #endregion
        
        #region Gameplay UI

        private void HandleEnableGameplayUI()
        {
            PlayAnimation(_animationUiOpen);
        }
        
        private void HandleDisableGameplayUI()
        {
            PlayAnimation(_animationUiClose,OnUiClosed);
        }
        
        private void HandleFinishAddingPoints()
        {
            PlayAnimation(_animationFinishAddingScore);
        }


        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
            
        }

        #endregion
    }
}