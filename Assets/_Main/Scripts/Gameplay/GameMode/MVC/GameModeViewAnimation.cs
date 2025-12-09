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

        #region Countdown
        
        private class Animation_Countdown_Update : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel>
        {
            public Animation_Countdown_Update(GameModeUIAnimationComponents.ICountdownPanel uiComponents) : base(uiComponents) { }

            protected override Sequence CreateAnimation()
            {
                var sequence = DOTween.Sequence()
                    .Append(UIComponents.CountdownText.DOScale(0, 0));
                
                if (GameManager.Instance.AntiEpileptic == false)
                {
                    sequence
                        .Append(TextTween.Fade(UIComponents.CountdownText, 0.4f, true))
                        .Join(UIComponents.CountdownText.DOScale(1, 0.4f))
                        .AppendInterval(0.15f)
                        .Append(TextTween.Fade(UIComponents.CountdownText, 0.45f, false))
                        ;
                }
                else
                {
                    sequence
                        .Join(UIComponents.CountdownText.DOScale(1, 0.4f))
                        .AppendInterval(0.15f)
                        ;
                }
                
                return sequence;
            }
        }
        private class Animation_Countdown_Finish : SequenceUIAnimator<GameModeUIAnimationComponents.ICountdownPanel>
        {
            public Animation_Countdown_Finish(GameModeUIAnimationComponents.ICountdownPanel uiComponents) : base(uiComponents) { }

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
                        .Append(TextTween.Fade(UIComponents.CountdownText, 0.05f, true))
                        .Append(UIComponents.CountdownText.DOScale(1.5f, 0.10f))
                        .AppendInterval(0.6f)
                        .Append(TextTween.Fade(UIComponents.CountdownText, 0.25f, false));
                }
                else
                {
                    sequence
                        .Append(UIComponents.CountdownText.DOScale(1.5f, 0.4f))
                        .AppendInterval(0.6f)
                        .Append(UIComponents.CountdownText.DOScale(0, 0.25f));
                }
                
                return sequence;
            }
        }
        
        #endregion

        #region Gameplay

        private class Animation_UI_Open : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel>
        {
            private readonly Vector2 _scoreOffscreenPos;
            private readonly Vector2 _scoreStartPos;
            //
            private readonly Vector2 _pauseOffscreenPos;
            private readonly Vector2 _pauseStartPos;
            
            public Animation_UI_Open(GameModeUIAnimationComponents.IGameplayPanel components) 
                : base(components)
            {
                _scoreStartPos = UIComponents.ScorePanel.anchoredPosition;
                _scoreOffscreenPos = AnimationHelper.
                    GetOffscreenPos(UIComponents.ScorePanel, AnimationHelper.Direction.UpLeft);
                //
                _pauseStartPos = UIComponents.ScorePanel.anchoredPosition;
                _pauseOffscreenPos = AnimationHelper.
                    GetOffscreenPos(UIComponents.PauseButton, AnimationHelper.Direction.UpRight);
            }

            protected override void Initialize()
            {
                UIComponents.ScorePanel.anchoredPosition = _scoreOffscreenPos;
                UIComponents.PauseButton.anchoredPosition = _pauseOffscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                Initialize();
                
                return DOTween.Sequence()
                    .AppendCallback(()=>UIComponents.GameplayPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scoreStartPos, 0.25f))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pauseStartPos, 0.25f));
            }
        }
        private class Animation_UI_Close : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel>
        {
            private readonly Vector2 _scoreOffscreenPos;
            //
            private readonly Vector2 _pauseOffscreenPos;
            
            public Animation_UI_Close(GameModeUIAnimationComponents.IGameplayPanel components) 
                : base(components)
            {
                _scoreOffscreenPos = AnimationHelper
                    .GetOffscreenPos(UIComponents.ScorePanel, AnimationHelper.Direction.UpLeft);
                //
                _pauseOffscreenPos = AnimationHelper
                    .GetOffscreenPos(UIComponents.PauseButton, AnimationHelper.Direction.UpRight);
            }
            
            protected override Sequence CreateAnimation()
            {
                
                return DOTween.Sequence()
                    .Append(UIComponents.ScorePanel.DOAnchorPos(_scoreOffscreenPos, 0.5f))
                    .Join(UIComponents.PauseButton.DOAnchorPos(_pauseOffscreenPos, 0.5f))
                    .AppendCallback(()=> UIComponents.GameplayPanel.gameObject.SetActive(false));
            }
        }
        private class Animation_Points_FinishAdding : SequenceUIAnimator<GameModeUIAnimationComponents.IGameplayPanel>
        {
            public Animation_Points_FinishAdding(GameModeUIAnimationComponents.IGameplayPanel components) 
                : base(components)
            {
            }
            
            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.ScorePanel.DOScale(1.25f, 0.25f))
                    .Append(UIComponents.ScorePanel.DOScale(1, 0.1f));
            }
        }

        #endregion

        #endregion
        
        private IUIAnimator _animationCountdownUpdate;
        private IUIAnimator _animationCountdownFinish;
        private IUIAnimator _animationUiOpen;
        private IUIAnimator _animationUiClose;
        private IUIAnimator _animationFinishAddingPoints;

        #region IGameModeViewAnimation

        public event Action OnUiClosed;

        #endregion
        
        private void Awake()
        {
            _animationCountdownUpdate = new Animation_Countdown_Update(UIComponents);
            _animationCountdownFinish = new Animation_Countdown_Finish(UIComponents);
            //
            _animationUiOpen = new Animation_UI_Open(UIComponents);
            _animationUiClose = new Animation_UI_Close(UIComponents);
            //
            _animationFinishAddingPoints = new Animation_Points_FinishAdding(UIComponents);
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
            PlayAnimation(_animationFinishAddingPoints);
        }


        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
            
        }

        #endregion
    }
}