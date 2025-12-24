using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Defeat.So;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.GameConfig;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using DG.Tweening;
using Plugins.Demigiant.DOTween;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    public class DefeatViewAnimation : BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>,
        DefeatViewAnimation.IDefeatViewAnimation, IDefeatAnimationSounds, IDefeatAnimationVibration, DefeatViewAnimation.IVibrationCaller
    {
        public interface IDefeatViewAnimation : 
            BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnScoreFinished;
            public event Action OnHighScoreFinished;
            public event Action OnCoinsFinished;
            public event Action OnButtonsFinished;
        }

        private interface IVibrationCaller : IAnimationVibrationComponent
        {
            public void TriggerScoreMoved();
            public void TriggerHighScoreMoved();
            public void TriggerNewHighScore();
            public void TriggerCoinsMoved();
            public void TriggerTitleMoved();
        }

        [SerializeField] private DefeatUiAnimationData animationData; 
        
        #region Animators

        private class AnimationInitializer : UIComponentsInitializer<DefeatUiAnimationComponents.IMainPanel>
        {
            public AnimationInitializer(DefeatUiAnimationComponents.IMainPanel components)
                : base(components)
            {
                
            }

            protected override void Initialize()
            {
                // Main Panel
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                // Images
                UIComponents.BackgroundImage.gameObject.SetActive(false);
                
                // Texts
                UIComponents.Title.gameObject.SetActive(false);
                UIComponents.HighScorePanel.gameObject.SetActive(false);
                UIComponents.HighScoreText.gameObject.SetActive(false);
                UIComponents.Score.gameObject.SetActive(false);
                UIComponents.SubHighScoreText.gameObject.SetActive(false);
                
                // Buttons
                UIComponents.ButtonsPanel.gameObject.SetActive(false);
                
                // Coins
                UIComponents.CoinsPanel.gameObject.SetActive(false);
            }
        }
        
        #region Main Panel

        private class Animation_Main_Open : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IMainPanel,IPanelOpen, IVibrationCaller>
        {
            public Animation_Main_Open(DefeatUiAnimationComponents.IMainPanel components, IPanelOpen animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent) { }

            protected override void Initialize()
            {
                // Main Panel
                UIComponents.MainPanel.gameObject.SetActive(false);
                
                // Images
                UIComponents.BackgroundImage.gameObject.SetActive(false);
                
                // Texts
                UIComponents.Title.gameObject.SetActive(false);
                UIComponents.HighScorePanel.gameObject.SetActive(false);
                UIComponents.HighScoreText.gameObject.SetActive(false);
                UIComponents.Score.gameObject.SetActive(false);
                UIComponents.SubHighScoreText.gameObject.SetActive(false);
                
                // Buttons
                UIComponents.ButtonsPanel.gameObject.SetActive(false);
                
            }

            protected override Sequence CreateAnimation()
            {
                Initialize();

                return DOTween.Sequence()
                        .AppendInterval(AnimationData.FadeInDelay)
                        .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                        .AppendCallback(() => UIComponents.BackgroundImage.gameObject.SetActive(true))
                        .Append(UIComponents.BackgroundImage.DOFade(0, 0))
                        .Append(UIComponents.BackgroundImage.DOFade(AnimationData.FadeIntensity, AnimationData.FadeInDuration).SetEase(Ease.InQuad))
                        .AppendCallback(() => VibrationComponent.TriggerTitleMoved())
                        .AppendInterval(AnimationData.ScaleDelay)
                        .Append(UIComponents.Title.DOScale(0, 0))
                        .AppendCallback(() => UIComponents.Title.gameObject.SetActive(true))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.ScaleDuration))
                        .AppendInterval(AnimationData.BounceDelay)
                        .Append(UIComponents.Title.DOScale(AnimationData.BounceScale, AnimationData.BounceDuration))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.BounceReturnTime))
                        .AppendCallback(() => VibrationComponent.TriggerTitleMoved())
                        .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }
        private class Animation_Main_Close : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IMainPanel,IPanelClose, IVibrationCaller>
        {
            
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _highScorePanel;
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _buttonsPanel;
            private readonly AnimationHelper.PanelPosition _coinsPanel;
            
            public Animation_Main_Close(DefeatUiAnimationComponents.IMainPanel components, IPanelClose animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _scorePanel = new AnimationHelper.PanelPosition(UIComponents.Score,
                    AnimationData.CurrentScoreOffscreenPosition);
                
                _highScorePanel = new AnimationHelper.PanelPosition(UIComponents.HighScoreText, 
                    AnimationData.HighScoreOffscreenPosition);
                
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.Title,
                    AnimationData.TitleOffscreenPosition, AnimationData.TitleOffScreenOffset);
                
                _buttonsPanel = new AnimationHelper.PanelPosition(UIComponents.ButtonsPanel, 
                    AnimationData.ButtonsOffscreenPosition, AnimationData.ButtonsOffScreenOffset);
                
                _coinsPanel = new AnimationHelper.PanelPosition(UIComponents.CoinsPanel, 
                    AnimationData.CoinsOffscreenPosition, AnimationData.CoinsOffScreenOffset);
            }

            protected override void RestartValues()
            {
                UIComponents.Title.anchoredPosition = _titlePanel.StartPos;
                UIComponents.ButtonsPanel.anchoredPosition = _buttonsPanel.StartPos;
                UIComponents.CoinsPanel.anchoredPosition = _coinsPanel.StartPos;
            }

            protected override Sequence CreateAnimation()
            {
                var movementDuration = AnimationData.MovementDuration;
                
                return DOTween.Sequence()
                        .AppendInterval(AnimationData.MovementDelay)
                        .AppendCallback(() => VibrationComponent.TriggerTitleMoved())
                        .Append(UIComponents.Score.DOAnchorPos(_scorePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.HighScorePanel.DOAnchorPos(_highScorePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.Title.DOAnchorPos(_titlePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.ButtonsPanel.DOAnchorPos(_buttonsPanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.CoinsPanel.DOAnchorPos(_coinsPanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.BackgroundImage.DOFade(0, AnimationData.BackgroundFadeDuration))
                        .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false))
                    ;
            }
        }

        #endregion

        #region Score
        
        private class Animation_CurrentScore_Increment : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IScore,ICurrentScoreIncrement, IVibrationCaller>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private uint _targetScore;
            private readonly AnimationHelper.PanelPosition _panelPosition;

            public Animation_CurrentScore_Increment(
                DefeatUiAnimationComponents.IScore components, 
                ICurrentScoreIncrement animationData, 
                Action<TMP_Text,long> handleText, 
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _handleText = handleText;
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.Score,
                    AnimationData.OffscreenPosition);
            }

            public void SetTargetScore(uint targetScore)
            {
                _targetScore = targetScore;
            }

            protected override void Initialize()
            {
                UIComponents.Score.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                var text = UIComponents.Score.GetComponent<TMP_Text>();
                
                return DOTween.Sequence()
                        .AppendCallback(()=> UIComponents.Score.gameObject.SetActive(true))
                        .Append(UIComponents.Score.DOAnchorPos(_panelPosition.StartPos, AnimationData.MoveDuration))
                        .AppendCallback(() => VibrationComponent.TriggerScoreMoved())
                        .Append(TweenUtils.AnimateScore(_handleText, text, _targetScore, TweenUtils.GetDurationLog(_targetScore)))
                        .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }
        private class Animation_HighScore_Increment : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IHighScore,IHighScoreIncrement,IVibrationCaller>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private uint _targetScore;
            private bool _hasNewHighScore;
            private readonly AnimationHelper.PanelPosition _panelPosition;
            
            public Animation_HighScore_Increment(DefeatUiAnimationComponents.IHighScore components, IHighScoreIncrement animationData, 
                Action<TMP_Text,long> handleText,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _handleText = handleText;
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.HighScorePanel,
                    AnimationData.OffscreenPosition);
            }
            
            public void SetTargetScore(uint targetScore, bool hasNewHighScore)
            {
                _targetScore = targetScore;
                _hasNewHighScore = hasNewHighScore;
            }

            protected override void Initialize()
            {
                UIComponents.HighScorePanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                var text = UIComponents.HighScoreText.GetComponent<TMP_Text>();

                var sequence = DOTween.Sequence()
                    .AppendCallback(() => UIComponents.HighScorePanel.gameObject.SetActive(true))
                    .AppendCallback(() => UIComponents.HighScoreText.gameObject.SetActive(true))
                    .Append(UIComponents.HighScorePanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.MoveDuration))
                    .AppendCallback(() => VibrationComponent.TriggerHighScoreMoved())
                    .Append(TweenUtils.AnimateScore(_handleText, text, (long)_targetScore,
                        TweenUtils.GetDurationLog(_targetScore)));

                if (_hasNewHighScore)
                {
                    sequence
                        .Append(UIComponents.HighScoreText.DOScale(1, AnimationData.ScaleDuration))
                        .AppendInterval(AnimationData.BounceDelay)
                        .Append(UIComponents.HighScoreText.DOScale(AnimationData.BounceScale, AnimationData.BounceDuration))
                        .Append(UIComponents.HighScoreText.DOScale(1, AnimationData.BounceReturnTime))
                        .Append(UIComponents.SubHighScoreText.DOScale(0,0))
                        .AppendInterval(AnimationData.NewScoreTextDelay)
                        .AppendCallback(()=> UIComponents.SubHighScoreText.gameObject.SetActive(true))
                        .AppendCallback(() => VibrationComponent.TriggerNewHighScore())
                        .Append(UIComponents.SubHighScoreText.DOScale(AnimationData.NewScoreBounceScale, AnimationData.NewScoreBounceDuration))
                        .Append(UIComponents.SubHighScoreText.DOScale(1, AnimationData.NewScoreBounceReturnTime))
                        ;
                }
                
                sequence.AppendInterval(AnimationData.FinishDelay);
                
                return sequence;
            }
        }
        
        #endregion

        #region Buttons
        
        private class Animation_Buttons_Open : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IButtons,IButtonsOpen, IVibrationCaller>
        {
            private readonly AnimationHelper.PanelPosition _panelPosition;
            
            public Animation_Buttons_Open(DefeatUiAnimationComponents.IButtons components, IButtonsOpen animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _panelPosition =
                    new AnimationHelper.PanelPosition(UIComponents.ButtonsPanel, AnimationData.OffscreenPosition, AnimationData.OffscreenOffset);
            }
            
            
            protected override void Initialize()
            {
                UIComponents.ButtonsPanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(()=> UIComponents.ButtonsPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ButtonsPanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.MoveDuration))
                    .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }
        
        #endregion

        #region Coins

        private class Animation_Coins_OpenPanel : SequenceUIAnimationVibration<DefeatUiAnimationComponents.ICoins,ICoinsOpen, IVibrationCaller>
        {
            private readonly AnimationHelper.PanelPosition _panelPosition;

            public Animation_Coins_OpenPanel(DefeatUiAnimationComponents.ICoins components, ICoinsOpen animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.CoinsPanel, AnimationData.OffscreenPosition, AnimationData.OffscreenOffset);
            }

            protected override void Initialize()
            {
                UIComponents.CoinsPanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                        .AppendCallback(()=> UIComponents.CoinsPanel.gameObject.SetActive(true))
                        .Append(UIComponents.CoinsPanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.MoveDuration))
                        .AppendCallback(() => VibrationComponent.TriggerCoinsMoved())
                        .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }

        #endregion
        
        #endregion

        private IUIComponentsInitializer _animationInitializer;
        //
        private IUIAnimator _animationMainOpen;
        private IUIAnimator _animationMainClose;
        //
        private IUIAnimator _animationScoreCurrent;
        private IUIAnimator _animationScoreHigh;
        //
        private IUIAnimator _animationButtonsOpen;
        //
        private IUIAnimator _animationOpenCoinsPanel;
        
        
        private string _currentScoreLocalizedText;
        private string _highScoreLocalizedText;
        
        #region IDefeatViewAnimation

        public event Action OnScoreFinished;
        public event Action OnHighScoreFinished;
        public event Action OnButtonsFinished;
        public event Action OnCoinsFinished;

        #endregion
        
        #region IDefeatAnimationSounds

        public event Action OnPlayMusic;
        public event Action OnStopMusic;

        #endregion

        #region IDefeatAnimationVibration

        public event Action OnScoreMoved;
        public event Action OnHighScoreMoved;
        public event Action OnNewHighScore;
        public event Action OnCoinsMoved;
        public event Action OnTitleMoved;

        #endregion
        
        
        private void Start()
        {
            InitializeScoreTexts();
            
            _animationInitializer = new AnimationInitializer(UIComponents);
            //
            _animationMainOpen = new Animation_Main_Open(UIComponents,animationData.PanelOpenData, this);
            _animationMainClose = new Animation_Main_Close(UIComponents, animationData.PanelCloseData, this);
            //
            _animationButtonsOpen = new Animation_Buttons_Open(UIComponents, animationData.ButtonsOpenData, this);
            _animationScoreCurrent = new Animation_CurrentScore_Increment(UIComponents,animationData.CurrentScoreData,HandleCurrentScoreText, this);
            //
            _animationScoreHigh = new Animation_HighScore_Increment(UIComponents,animationData.HighScoreData,HandleHighScoreText, this);
            //
            _animationOpenCoinsPanel = new Animation_Coins_OpenPanel(UIComponents, animationData.CoinsOpenData, this);
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
                
                case DefeatObserverMessage.SendCoins:
                    HandleSendCoins();
                    break;
                
                //=== Enable / Disable ===
                case DefeatObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;  
                
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
        
        private void HandleInitializeData()
        {
            _animationInitializer.InitializeValues();
        }

        #region Buttons

        private void HandleSendButtons()
        {
            PlayAnimation(_animationButtonsOpen, OnButtonsFinished);
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
            
            var sendScore = (Animation_CurrentScore_Increment)_animationScoreCurrent;
            sendScore.SetTargetScore(finalScore);
            PlayAnimation(sendScore,OnScoreFinished);
            
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

            var sendScore = (Animation_HighScore_Increment)_animationScoreHigh;
            sendScore.SetTargetScore(finalScore, hasHighScore);
            PlayAnimation(sendScore,OnHighScoreFinished);
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
            HandleHighScoreText(UIComponents.HighScoreText.GetComponent<TMP_Text>(),0);
        }

        private int GetPointsMultiplier()
        {
            var instance = GameConfigManager.Instance;
            return instance ? GameConfigManager.Instance.GetGameplayData().PointsMultiplier : 500;
        }

        #endregion

        #region Coins

        private void HandleSendCoins()
        {
            PlayAnimation(_animationOpenCoinsPanel, OnCoinsFinished);
        }

        #endregion
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }

        #region IVibrationCaller

        public void TriggerScoreMoved() => OnScoreMoved?.Invoke();
        public void TriggerHighScoreMoved() => OnHighScoreMoved?.Invoke();
        public void TriggerNewHighScore() => OnNewHighScore?.Invoke();
        public void TriggerCoinsMoved() => OnCoinsMoved?.Invoke();
        public void TriggerTitleMoved() => OnTitleMoved?.Invoke();

        #endregion

    }
}