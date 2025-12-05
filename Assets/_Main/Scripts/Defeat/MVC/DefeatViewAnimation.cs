using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Plugins.Demigiant.DOTween;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    public class DefeatViewAnimation : BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>,
        DefeatViewAnimation.IDefeatViewAnimation, IDefeatAnimationSounds
    {
        public interface IDefeatViewAnimation : 
            BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnScoreFinished;
            public event Action OnHighScoreFinished;
            public event Action OnButtonsFinished;
        }
        
        #region Animators

        #region Main Panel

        private class Animation_Main_Open : SequenceUIAnimator<DefeatUiAnimationComponents.IMainPanel>
        {
            public Animation_Main_Open(DefeatUiAnimationComponents.IMainPanel uiComponents) 
                : base(uiComponents) { }
            
            protected override void Initialize()
            {
                // Main Panel
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                // Images
                UIComponents.BackgroundImage.gameObject.SetActive(false);
                
                // Texts
                UIComponents.Title.gameObject.SetActive(false);
                UIComponents.HighScore.gameObject.SetActive(false);
                UIComponents.Score.gameObject.SetActive(false);
                UIComponents.SubHighScore.gameObject.SetActive(false);
                
                // Buttons
                UIComponents.ButtonsPanel.gameObject.SetActive(false);
            }

            protected override Sequence CreateAnimation()
            {
                Initialize();
                
                return DOTween.Sequence()
                        .AppendInterval(0.5f)
                        .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                        .AppendCallback(() => UIComponents.BackgroundImage.gameObject.SetActive(true))
                        .Append(UIComponents.BackgroundImage.DOFade(0, 0))
                        .Append(UIComponents.BackgroundImage.DOFade(0.15f, 1.5f).SetEase(Ease.InQuad))
                        .AppendInterval(0.5F)
                        .Append(UIComponents.Title.DOScale(0, 0))
                        .AppendCallback(() => UIComponents.Title.gameObject.SetActive(true))
                        .Append(UIComponents.Title.DOScale(1, 0.75f))
                        .AppendInterval(0.05f)
                        .Append(UIComponents.Title.DOScale(1.25f, 0.1f))
                        .Append(UIComponents.Title.DOScale(1, 0.35f));
            }
        }
        private class Animation_Main_Close : SequenceUIAnimator<DefeatUiAnimationComponents.IMainPanel>
        {
            public Animation_Main_Close(DefeatUiAnimationComponents.IMainPanel uiComponents) 
                : base(uiComponents) { }
            
            private const float FadeTime = 0.3f;
            private Vector2 _panelStartPos;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                UIComponents.MainPanel.gameObject.SetActive(false);
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.MainPanel, AnimationHelper.Direction.Up);
                _panelStartPos = UIComponents.MainPanel.anchoredPosition;
            }

            protected override void RestartValues()
            {
                UIComponents.MainPanel.anchoredPosition = _panelStartPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.MainPanel.DOAnchorPos(_offscreenPos, FadeTime))
                    .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false));
            }
        }

        #endregion

        private class Animation_CurrentScore_Increment : SequenceUIAnimator<DefeatUiAnimationComponents.IScore>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private Vector2 _offscreenPos;
            private Vector2 _originalPos;
            private readonly uint _targetScore;
            private const float FadeTime = 0.3f;
            private const float IncrementTime = 2f;

            public Animation_CurrentScore_Increment(DefeatUiAnimationComponents.IScore uiComponents, uint targetScore,
                Action<TMP_Text,long> handleText) : base(uiComponents)
            {
                _targetScore = targetScore;
                _handleText = handleText;
            }

            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.Score, AnimationHelper.Direction.UpRight);
                UIComponents.Score.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                var text = UIComponents.Score.GetComponent<TMP_Text>();
                
                return DOTween.Sequence()
                        .AppendCallback(()=> UIComponents.Score.gameObject.SetActive(true))
                        .Append(UIComponents.Score.DOAnchorPos(_originalPos, FadeTime))
                        .Append(TweenUtils.AnimateScore(_handleText, text, _targetScore, TweenUtils.GetDurationLog(_targetScore)))
                        .AppendInterval(0.5f)
                    ;
            }
        }
        private class Animation_HighScore_Increment : SequenceUIAnimator<DefeatUiAnimationComponents.IHighScore>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private readonly uint _targetScore;
            private readonly bool _hasNewHighScore;
            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            private Vector2 _originalPos;

            public Animation_HighScore_Increment(DefeatUiAnimationComponents.IHighScore uiComponents,
                uint targetScore, bool hasNewHighScore ,Action<TMP_Text,long> handleText) : base(uiComponents)
            {
                _hasNewHighScore = hasNewHighScore;
                _targetScore = targetScore;
                _handleText = handleText;
            }

            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.HighScore, AnimationHelper.Direction.UpLeft);
                UIComponents.HighScore.anchoredPosition = _offscreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                var text = UIComponents.HighScore.GetComponent<TMP_Text>();

                var sequence = DOTween.Sequence()
                    .AppendCallback(() => UIComponents.HighScore.gameObject.SetActive(true))
                    .Append(UIComponents.HighScore.DOAnchorPos(_originalPos, FadeTime))
                    .Append(TweenUtils.AnimateScore(_handleText, text, (long)_targetScore,
                        TweenUtils.GetDurationLog(_targetScore)));

                if (_hasNewHighScore)
                {
                    sequence
                        .Append(UIComponents.HighScore.DOScale(1, 0.75f))
                        .AppendInterval(0.05f)
                        .Append(UIComponents.HighScore.DOScale(1.5f, 0.1f))
                        .Append(UIComponents.HighScore.DOScale(1, 0.35f))
                        .Append(UIComponents.SubHighScore.DOScale(0,0))
                        .AppendInterval(0.25f)
                        .AppendCallback(()=> UIComponents.SubHighScore.gameObject.SetActive(true))
                        .Append(UIComponents.SubHighScore.DOScale(2f, 0.5f))
                        .Append(UIComponents.SubHighScore.DOScale(1, 0.20f))
                        ;
                }
                
                sequence.AppendInterval(0.5f);
                
                return sequence;
            }
        }
        private class Animation_Buttons_Open : SequenceUIAnimator<DefeatUiAnimationComponents.IButtons>
        {
            public Animation_Buttons_Open(DefeatUiAnimationComponents.IButtons uiComponents) 
                : base(uiComponents) { }

            private const float FadeTime = 0.3f;
            private Vector2 _offscreenPos;
            
            protected override void Initialize()
            {
                _offscreenPos = AnimationHelper.GetOffscreenPos(UIComponents.ButtonsPanel, AnimationHelper.Direction.Down);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(()=> UIComponents.ButtonsPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ButtonsPanel.DOAnchorPos(_offscreenPos, FadeTime).From())
                    .AppendInterval(0.5f)
                    ;
            }
        }
        


        #endregion
        
        private IUIAnimator _animationMainOpen;
        private IUIAnimator _animationMainClose;
        //
        private IUIAnimator _animationScoreCurrent;
        private IUIAnimator _animationScoreHigh;
        //
        private IUIAnimator _animationButtonsOpen;
        
        
        private string _currentScoreLocalizedText;
        private string _highScoreLocalizedText;
        
        
        #region IDefeatViewAnimation

        public event Action OnScoreFinished;
        public event Action OnHighScoreFinished;
        public event Action OnButtonsFinished;

        #endregion
        
        
        #region IDefeatAnimationSounds

        public event Action OnPlayMusic;
        public event Action OnStopMusic;

        #endregion
        
        
        private void Start()
        {
            InitializeScoreTexts();
            
            _animationMainOpen = new Animation_Main_Open(UIComponents);
            _animationMainClose = new Animation_Main_Close(UIComponents);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Score ===
                case DefeatObserverMessage.SendScore:
                    HandleSendScore((GeneratedId)args[0]);
                    break;
                case DefeatObserverMessage.SendHighScore:
                    HandleSendHighScore((GeneratedId)args[0],(bool)args[1]);
                    break;
                
                //=== Enable / Disable ===
                case DefeatObserverMessage.Enable:
                    HandleEnable();
                    break;
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                
                //=== Buttons ===
                case DefeatObserverMessage.SendButtons:
                    HandleSendButtons();
                    break;
            }
        }
        
        #region Buttons

        private void HandleSendButtons()
        {
            var buttons = new Animation_Buttons_Open(UIComponents);
            buttons.Play(OnButtonsFinished);
        }

        #endregion
        
        #region Enable / Disable
        
        private void HandleEnable()
        {
            InitializeScoreTexts();
            //
            PlayAnimation(_animationMainOpen, () =>
            {
                TriggerOnPanelOpened();
                OnPlayMusic?.Invoke();
            });
        }
        
        private void HandleStartDisable()
        {
            PlayAnimation(_animationMainClose, () =>
            {
                TriggerOnPanelClosed();
                OnStopMusic?.Invoke();
            });
        }
        
        #endregion

        #region Score
        
        private void HandleSendScore(GeneratedId generatedId)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(generatedId, out var currentScore) == false)
            {
                Debug.LogWarning("DefeatViewAnimation::HandleSendScore: Could not find score");
                return;
            }
            
            uint finalScore = (uint)(currentScore * GetPointsMultiplier());
            
            //Debug.LogWarning($"Target Current Score: {finalScore}, Inner: {currentScore}");
            
            var sendScore = new Animation_CurrentScore_Increment(UIComponents,finalScore,HandleCurrentScoreText);
            sendScore.Play(OnScoreFinished);
            
        }

        private void HandleSendHighScore(GeneratedId generatedId, bool hasHighScore)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(generatedId, out var highScore) == false)
            {
                Debug.LogWarning("DefeatViewAnimation::HandleSendHighScore: Could not find high score");
                return;
            }
            
            uint finalScore = (uint)(highScore * GetPointsMultiplier());
            
            //Debug.LogWarning($"Target High Score: {finalScore}, Inner: {highScore}");
            
            var sendScore = new Animation_HighScore_Increment(UIComponents,finalScore,hasHighScore,HandleHighScoreText);
            sendScore.Play(OnHighScoreFinished);
        }
        

        private void HandleCurrentScoreText(TMP_Text text, long score)
        {
            text.text = $"{_currentScoreLocalizedText}:{score:D6}";
        }
        
        private void HandleHighScoreText(TMP_Text text, long score)
        {
            text.text = $"{_highScoreLocalizedText}:{score:D6}";
        }

        private void InitializeScoreTexts()
        {
            _currentScoreLocalizedText = GetLocalizedString("Gameplay.Score");
            _highScoreLocalizedText = GetLocalizedString("Gameplay.HighScore");
            
            HandleCurrentScoreText(UIComponents.Score.GetComponent<TMP_Text>(),0);
            HandleHighScoreText(UIComponents.HighScore.GetComponent<TMP_Text>(),0);
        }

        private int GetPointsMultiplier()
        {
            var instance = GameConfigManager.Instance;
            return instance ? GameConfigManager.Instance.GetGameplayData().PointsMultiplier : 500;
        }

        #endregion
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}