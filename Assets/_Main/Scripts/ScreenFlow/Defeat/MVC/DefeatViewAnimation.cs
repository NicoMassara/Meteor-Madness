using System;
using DG.Tweening;
using MeteorMadness.Animations;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Base;
using MeteorMadness.ScreenFlow.Defeat.So;
using Plugins.Demigiant.DOTween;
using TMPro;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatViewAnimation : BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>,
        DefeatViewAnimation.IDefeatViewAnimation, IDefeatAnimationSounds, IDefeatAnimationVibration, DefeatViewAnimation.IVibrationCaller
    {
        public interface IDefeatViewAnimation : IBaseViewAnimation
        {
            public event Action OnScoreFinished;
            public event Action OnHighScoreFinished;
            public event Action OnCoinsFinished;
            public event Action OnButtonsFinished;
        }

        private interface IVibrationCaller : IAnimationVibrationComponent
        {
            public void TriggerVibration(DefeatAnimationVibrationType vibrationType);
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
                UIComponents.ScorePanel.gameObject.SetActive(false);
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
                UIComponents.ScorePanel.gameObject.SetActive(false);
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
                        .AppendInterval(AnimationData.ScaleDelay)
                        .Append(UIComponents.Title.DOScale(0, 0))
                        .AppendCallback(() => UIComponents.Title.gameObject.SetActive(true))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.ScaleDuration))
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TextAppear))
                        .AppendInterval(AnimationData.BounceDelay)
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TitleBounce))
                        .Append(UIComponents.Title.DOScale(AnimationData.BounceScale, AnimationData.BounceDuration))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.BounceReturnTime))
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
                _scorePanel = new AnimationHelper.PanelPosition(UIComponents.ScorePanel,
                    AnimationData.CurrentScoreOffscreenPosition, AnimationData.PointsOffScreenOffset);
                
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
                        .Append(UIComponents.ScorePanel.DOAnchorPos(_scorePanel.OffScreenPos, movementDuration))
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.ClosePanel))
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
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.ScorePanel,
                    AnimationData.OffscreenPosition, AnimationData.OffscreenOffset);

                UIComponents.ScorePanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            public void SetTargetScore(uint targetScore)
            {
                _targetScore = targetScore;
            }

            protected override void Initialize()
            {
                UIComponents.ScorePanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                var text = UIComponents.ScoreText.GetComponent<TMP_Text>();
                
                return DOTween.Sequence()
                        .AppendCallback(()=> UIComponents.ScorePanel.gameObject.SetActive(true))
                        .Append(UIComponents.ScorePanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.MoveDuration))
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TextAppear))
                        .Append(TweenUtils.AnimateScore(_handleText, text, _targetScore, TweenUtils.GetDurationLog(_targetScore)))
                        .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }
        private class Animation_HighScore_Increment : SequenceUIAnimationVibration<DefeatUiAnimationComponents.IHighScore,IHighScoreIncrement,IVibrationCaller>
        {
            
            public Animation_HighScore_Increment(DefeatUiAnimationComponents.IHighScore components, IHighScoreIncrement animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
            }

            protected override void Initialize()
            {
                UIComponents.SubHighScoreText.gameObject.SetActive(false);
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                        .AppendInterval(AnimationData.NewScoreTextDelay)
                        .AppendCallback(()=> UIComponents.SubHighScoreText.gameObject.SetActive(true))
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TitleBounce))
                        .Append(UIComponents.SubHighScoreText.DOScale(AnimationData.NewScoreBounceScale, AnimationData.NewScoreBounceDuration))
                        .Append(UIComponents.SubHighScoreText.DOScale(1, AnimationData.NewScoreBounceReturnTime))
                        .AppendInterval(AnimationData.FinishDelay);
            }
        }
        
        private class Animation_HighScore_Bounce: SequenceUIAnimationVibration<DefeatUiAnimationComponents.IHighScore,IHighScoreBounce,IVibrationCaller>
        {
            private readonly float _startScale;
            
            public Animation_HighScore_Bounce(DefeatUiAnimationComponents.IHighScore components, IHighScoreBounce animationData,
                IVibrationCaller vibrationComponent)
                : base(components, animationData, vibrationComponent)
            {
                _startScale = components.SubHighScoreText.localScale.x;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .Append(UIComponents.SubHighScoreText.DOScale(AnimationData.TargetScale, AnimationData.ScaleDuration))
                    .Append(UIComponents.SubHighScoreText.DOScale(AnimationData.MinScale, AnimationData.BounceDuration/2))
                    .Append(UIComponents.SubHighScoreText.DOScale(_startScale,AnimationData.BounceDuration/2))
                    .AppendInterval(AnimationData.LoopDelay)
                    .SetLoops(-1, LoopType.Restart)
                    ;
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
                    .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TextAppear))
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
                        .AppendCallback(() => VibrationComponent.TriggerVibration(DefeatAnimationVibrationType.TextAppear))
                        .AppendInterval(AnimationData.FinishDelay)
                    ;
            }
        }

        #endregion
        
        #endregion

        private IUIComponentsInitializer _animationInitializer;
        //
        private IUiAnimation _animationMainOpen;
        private IUiAnimation _animationMainClose;
        //
        private IUiAnimation _animationScoreCurrent;
        private IUiAnimation _animationScoreHigh;
        private IUiAnimation _animationBounceHighScore;
        //
        private IUiAnimation _animationButtonsOpen;
        //
        private IUiAnimation _animationOpenCoinsPanel;
        
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
        public event Action<DefeatAnimationVibrationType> OnVibration;

        #endregion
        
        
        private void Start()
        {
            _animationInitializer = new AnimationInitializer(UIComponents);
            //
            _animationMainOpen = new Animation_Main_Open(UIComponents,animationData.PanelOpenData, this);
            _animationMainClose = new Animation_Main_Close(UIComponents, animationData.PanelCloseData, this);
            //
            _animationButtonsOpen = new Animation_Buttons_Open(UIComponents, animationData.ButtonsOpenData, this);
            _animationScoreCurrent = new Animation_CurrentScore_Increment(UIComponents,animationData.CurrentScoreData,HandleCurrentScoreText, this);
            _animationBounceHighScore = new Animation_HighScore_Bounce(UIComponents,animationData.HighScoreBounceData, this);
            //
            _animationScoreHigh = new Animation_HighScore_Increment(UIComponents,animationData.HighScoreData, this);
            //
            _animationOpenCoinsPanel = new Animation_Coins_OpenPanel(UIComponents, animationData.CoinsOpenData, this);

            OnButtonsFinished += StartHighScoreBounce;
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Score ===
                case DefeatObserverMessage.SendScore:
                    HandleSendScore((uint)args[0]);
                    break;
                case DefeatObserverMessage.SendHighScore:
                    HandleSendHighScore((uint)args[0],(bool)args[1]);
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
            //
            PlayAnimation(_animationMainOpen, () =>
            {
                TriggerOnPanelOpened();
                OnPlayMusic?.Invoke();
            });
        }
        
        private void HandleStartDisable()
        {
            StopAnimation(_animationBounceHighScore);
            PlayAnimation(_animationMainClose, () =>
            {
                TriggerOnPanelClosed();
                OnStopMusic?.Invoke();
            }, true);
        }
        
        #endregion

        #region Score
        
        private void HandleSendScore(uint currentScore)
        {
            uint finalScore = (uint)(currentScore * GetPointsMultiplier());
            
            //Debug.Log($"Target Current Score: {finalScore}, Inner: {currentScore}");
            
            var sendScore = (Animation_CurrentScore_Increment)_animationScoreCurrent;
            sendScore.SetTargetScore(finalScore);
            PlayAnimation(sendScore,OnScoreFinished);
            
        }

        private void HandleSendHighScore(uint highScore, bool hasHighScore)
        {

            if (hasHighScore)
            {
                PlayAnimation(_animationScoreHigh,OnHighScoreFinished);
            }
            else
            {
                OnHighScoreFinished?.Invoke();
            }
        }
        

        private void HandleCurrentScoreText(TMP_Text text, long score)
        {
            text.text = $"{score:D6}";
        }
        
        private void HandleHighScoreText(TMP_Text text, long score)
        {
            text.text = $"{score:D6}";
        }

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }

        #endregion

        #region Coins

        private void HandleSendCoins()
        {
            PlayAnimation(_animationOpenCoinsPanel, OnCoinsFinished);
        }

        #endregion

        #region IVibrationCaller
        
        public void TriggerVibration(DefeatAnimationVibrationType vibrationType) => OnVibration?.Invoke(vibrationType);

        #endregion
        
        private void StartHighScoreBounce()
        {
            PlayAnimation(_animationBounceHighScore);
        }

    }
    
}