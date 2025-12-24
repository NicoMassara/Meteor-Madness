using System;
using _Main.Scripts.Managers;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using _Main.Scripts.GameMode.So;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.GameMode
{
    public class GameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>,
        GameModeViewAnimation.IGameModeViewAnimation
    {
        public interface IGameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnUiClosed;
        }
        
        [SerializeField] private GameplayUiAnimationData animData;
        
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

        private class Animation_UI_Open : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel, IGameplayPanelData>
        {
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _pausePanel;

            public Animation_UI_Open(GameModeUIAnimationComponents.IGameplayPanel components,
                IGameplayPanelData animationData)
                : base(components, animationData)
            {
                _scorePanel =
                    new AnimationHelper.PanelPosition(UIComponents.ScorePanel,
                        AnimationData.ScoreOffscreenPos,
                        AnimationData.ScoreOffset);
                _pausePanel =
                    new AnimationHelper.PanelPosition(UIComponents.PauseButton,
                        AnimationData.PauseOffscreenPos,
                        AnimationData.PauseOffset);

                UIComponents.GameplayPanel.gameObject.SetActive(false);
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
                    .AppendCallback(() => UIComponents.GameplayPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scorePanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pausePanel.StartPos, AnimationData.MovementDuration));
            }
        }

        private class Animation_UI_Close : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel, IGameplayPanelData>
        {
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _pausePanel;

            public Animation_UI_Close(GameModeUIAnimationComponents.IGameplayPanel components, IGameplayPanelData animationData)
                : base(components, animationData)
            {
                _scorePanel =
                    new AnimationHelper.PanelPosition(UIComponents.ScorePanel, AnimationData.ScoreOffscreenPos, AnimationData.ScoreOffset);
                _pausePanel =
                    new AnimationHelper.PanelPosition(UIComponents.PauseButton, AnimationData.PauseOffscreenPos, AnimationData.PauseOffset);
            }
            

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scorePanel.StartPos, AnimationData.MovementDuration))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pausePanel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendInterval(AnimationData.FinishDelay)
                    .AppendCallback(() => UIComponents.GameplayPanel.gameObject.SetActive(false));
            }
        }

        private class Animation_Score_FinishAdding : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel, IScoreFinishAdding>
        {
            public Animation_Score_FinishAdding(GameModeUIAnimationComponents.IGameplayPanel components, IScoreFinishAdding animationData)
                : base(components, animationData) { }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.ScoreText.DOScale(AnimationData.BounceScale, AnimationData.BounceDuration))
                    .Append(UIComponents.ScoreText.DOScale(1, AnimationData.BounceReturnTime));
            }
        }

        private class Animation_Streak_Failed : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel, IStreakFailed>
        {
            private readonly float _horizontalPos;
            
            public Animation_Streak_Failed(GameModeUIAnimationComponents.IGameplayPanel components, IStreakFailed animationData) 
                : base(components, animationData)
            {
                _horizontalPos = UIComponents.StreakText.anchoredPosition.x;
            }

            protected override void Initialize()
            {
                UIComponents.StreakText.anchoredPosition = new Vector2(_horizontalPos, UIComponents.StreakText.anchoredPosition.y);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                        .Append(UIComponents.StreakText.DOAnchorPosX(
                            _horizontalPos + AnimationData.HorizontalOffset, AnimationData.Duration))
                        .SetEase(Ease.InOutSine)
                        .SetLoops(AnimationData.Loops, LoopType.Yoyo)
                    ;
            }
        }
        
        private class Animation_Streak_Notify : SequenceUIAnimator<GameModeUIAnimationComponents.INotifyPanel, IStreakNotify>
        {
            private readonly AnimationHelper.PanelPosition _startNotifyPanel;
            private readonly AnimationHelper.PanelPosition _endNotifyPanel;

            public Animation_Streak_Notify(GameModeUIAnimationComponents.INotifyPanel components, IStreakNotify animationData)
                : base(components, animationData)
            {
                _startNotifyPanel =
                    new AnimationHelper.PanelPosition(UIComponents.NotifyPanel, AnimationData.StartPosition, AnimationData.StartOffset);
                _endNotifyPanel =
                    new AnimationHelper.PanelPosition(UIComponents.NotifyPanel, AnimationData.EndPosition, AnimationData.EndOffset);
                UIComponents.NotifyPanel.gameObject.SetActive(false);
            }

            protected override void Initialize()
            {
                UIComponents.NotifyPanel.anchoredPosition = _startNotifyPanel.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(() => UIComponents.NotifyPanel.gameObject.SetActive(true))
                    .Append(UIComponents.NotifyPanel.DOAnchorPos(_startNotifyPanel.StartPos, AnimationData.MovementDuration))
                    .AppendInterval(AnimationData.StopDuration)
                    .Append(UIComponents.NotifyPanel.DOAnchorPos(_endNotifyPanel.OffScreenPos, AnimationData.MovementDuration))
                    .AppendCallback(() => UIComponents.NotifyPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        #region Countdown

        private class Animation_Countdown_Update : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel, ICountdownUpdate>
        {
            public Animation_Countdown_Update(GameModeUIAnimationComponents.ICountdownPanel components, ICountdownUpdate animationData)
                : base(components, animationData) { }

            protected override Sequence CreateAnimation()
            {
                var sequence = DOTween.Sequence()
                    .Append(UIComponents.CountdownText.DOScale(0, 0));

                if (!GameManager.Instance.AntiEpileptic)
                {
                    sequence
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.FadeDuration, true))
                        .Join(UIComponents.CountdownText.DOScale(1, AnimationData.BounceDuration))
                        .AppendInterval(AnimationData.FadeDuration)
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.FinalFadeDuration, false));
                }
                else
                {
                    sequence
                        .Join(UIComponents.CountdownText.DOScale(1, AnimationData.AeScaleDuration))
                        .AppendInterval(AnimationData.AeFinishDelay);
                }

                return sequence;
            }
        }

        private class Animation_Countdown_Finish : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel, ICountdownFinish>
        {
            public Animation_Countdown_Finish(GameModeUIAnimationComponents.ICountdownPanel components, ICountdownFinish animationData)
                : base(components, animationData) { }

            protected override void RestartValues()
            {
                UIComponents.CountdownPanel.gameObject.SetActive(false);
            }

            protected override Sequence CreateAnimation()
            {
                var sequence = DOTween.Sequence();

                if (!GameManager.Instance.AntiEpileptic)
                {
                    sequence
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.FadeDuration, true))
                        .Append(UIComponents.CountdownText.DOScale(AnimationData.BounceScale, AnimationData.BounceDuration))
                        .AppendInterval(AnimationData.FadeDelay)
                        .Append(TextTween.Fade(UIComponents.CountdownText, AnimationData.FinalFadeDuration, false));
                }
                else
                {
                    sequence
                        .Append(UIComponents.CountdownText.DOScale(AnimationData.AeBounceScale, AnimationData.AeScaleDuration))
                        .AppendInterval(AnimationData.AeScaleDelay)
                        .Append(UIComponents.CountdownText.DOScale(0, AnimationData.AeFinishScaleDuration));
                }

                sequence.AppendInterval(AnimationData.FinishDelay);

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
        private IUIAnimator _animationStreakFailed;
        private IUIAnimator _animationStreakNotify;

        private uint _lastStreak;
            
        #region IGameModeViewAnimation

        public event Action OnUiClosed;

        #endregion
        
        private void Awake()
        {
            _animationCountdownUpdate = new Animation_Countdown_Update(UIComponents, animData.CountdownUpdateData);
            _animationCountdownFinish = new Animation_Countdown_Finish(UIComponents, animData.CountdownFinishData);
            //
            _animationUiOpen = new Animation_UI_Open(UIComponents, animData.GameplayUiOpenData);
            _animationUiClose = new Animation_UI_Close(UIComponents, animData.GameplayUiCloseData);
            //
            _animationFinishAddingScore = new Animation_Score_FinishAdding(UIComponents, animData.ScoreFinishAddingData);
            //
            _animationStreakFailed = new Animation_Streak_Failed(UIComponents, animData.StreakFailedData);
            _animationStreakNotify = new Animation_Streak_Notify(UIComponents, animData.StreakNotifyData);
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
                
                //=== Streak ===//
                case GameModeObserverMessage.UpdateStreak:
                    HandleUpdateStreak((uint)args[0]);
                    break;
                case GameModeObserverMessage.NotifyStreak:
                    HandleNotifyStreak();
                    break;
                
                
            }
        }

        private void HandleNotifyStreak()
        {
            PlayAnimation(_animationStreakNotify);
        }

        private void HandleUpdateStreak(uint amount)
        {
            if (amount == 0 && _lastStreak > 0)
            {
                Debug.Log($"Play Animation Streak Failed");
                
                Debug.Log(_animationStreakFailed);
                PlayAnimation(_animationStreakFailed);
            }
            
            _lastStreak = amount;
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