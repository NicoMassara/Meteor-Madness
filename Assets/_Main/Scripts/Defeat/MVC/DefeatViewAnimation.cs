using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
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
        DefeatViewAnimation.IDefeatViewAnimation, IDefeatAnimationSounds
    {
        public interface IDefeatViewAnimation : 
            BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>.IBaseViewAnimation
        {
            public event Action OnScoreFinished;
            public event Action OnHighScoreFinished;
            public event Action OnButtonsFinished;
        }
        
        #region Animation Data

        #region Panel
        
        [Serializable]
        private class PanelOpenData : UiAnimationData
        {
            public float fadeInDelay = 0.5f;
            public float fadeIntensity = 0.5f;
            public float fadeInDuration = 1.5f;
            public float scaleDelay = 0.5f;
            public float scaleDuration = 0.75f;
            public float bounceDelay = 0.05f;
            [Range(1.01f,2f)]
            public float bounceScale = 1.25f;
            public float bounceDuration = 0.1f;
            public float bounceReturnTime = 0.32f;
            public float finishDelay = 0.5f;
        }
        
        [SerializeField] private PanelOpenData panelOpenData;
        
        [Serializable]
        private class PanelCloseData : UiAnimationData
        {
            public float movementDelay = 0.5f;
            public float movementDuration = 0.25f;
            public AnimationHelper.Direction currentScoreOffscreenPosition = AnimationHelper.Direction.UpRight;
            public AnimationHelper.Direction highScoreOffscreenPosition = AnimationHelper.Direction.UpLeft;
            public AnimationHelper.Direction titleOffscreenPosition = AnimationHelper.Direction.Up;
            public AnimationHelper.Direction buttonsOffscreenPosition = AnimationHelper.Direction.Down;
            public float fadeDelay = 0.25f;
            public float backgroundFadeDuration = 0.3f;
            [Space]
            [Header("Offsets")]
            public Vector2 buttonsOffScreenOffset;
            public Vector2 titleOffScreenOffset;
        }
        
        [SerializeField] private PanelCloseData panelCloseData;
        
        #endregion

        #region Score

        [Serializable]
        private class CurrentScoreIncrement : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.UpRight;
            public float moveDuration = 0.3f;
            public float finishDelay = 0.5f;
        }
        
        [SerializeField] private CurrentScoreIncrement currentScoreData;
        
        [Serializable]
        private class HighScoreIncrement : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.UpLeft;
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
        }
        
        [SerializeField] private HighScoreIncrement highScoreData;

        #endregion

        #region Buttons

        [Serializable]
        private class ButtonsOpen : UiAnimationData
        {
            public AnimationHelper.Direction offscreenPosition = AnimationHelper.Direction.Down;
            public Vector2 offscreenOffset;
            public float moveDuration = 0.3f;
            public float finishDelay = 0.5f;
        }
        
        [SerializeField] private ButtonsOpen buttonsOpenData;

        #endregion
        
        #endregion
        
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
            }
        }

        #region Main Panel

        private class Animation_Main_Open : SequenceUIAnimator<DefeatUiAnimationComponents.IMainPanel,PanelOpenData>
        {
            public Animation_Main_Open(DefeatUiAnimationComponents.IMainPanel components, PanelOpenData animationData)
                : base(components, animationData) { }
            
            
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
                        .AppendInterval(AnimationData.fadeInDelay)
                        .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(true))
                        .AppendCallback(() => UIComponents.BackgroundImage.gameObject.SetActive(true))
                        .Append(UIComponents.BackgroundImage.DOFade(0, 0))
                        .Append(UIComponents.BackgroundImage
                            .DOFade(AnimationData.fadeIntensity, AnimationData.fadeInDuration).SetEase(Ease.InQuad))
                        .AppendInterval(AnimationData.scaleDelay)
                        .Append(UIComponents.Title.DOScale(0, 0))
                        .AppendCallback(() => UIComponents.Title.gameObject.SetActive(true))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.scaleDuration))
                        .AppendInterval(AnimationData.bounceDelay)
                        .Append(UIComponents.Title.DOScale(AnimationData.bounceScale, AnimationData.bounceDuration))
                        .Append(UIComponents.Title.DOScale(1, AnimationData.bounceReturnTime))
                        .AppendInterval(AnimationData.finishDelay)
                    ;
            }
        }
        private class Animation_Main_Close : SequenceUIAnimator<DefeatUiAnimationComponents.IMainPanel,PanelCloseData>
        {
            
            private readonly AnimationHelper.PanelPosition _scorePanel;
            private readonly AnimationHelper.PanelPosition _highScorePanel;
            private readonly AnimationHelper.PanelPosition _titlePanel;
            private readonly AnimationHelper.PanelPosition _buttonsPanel;
            
            public Animation_Main_Close(DefeatUiAnimationComponents.IMainPanel components, PanelCloseData animationData)
                : base(components, animationData)
            {
                _scorePanel = new AnimationHelper.PanelPosition(UIComponents.Score, AnimationData.currentScoreOffscreenPosition);
                _highScorePanel = new AnimationHelper.PanelPosition(UIComponents.HighScoreText, AnimationData.highScoreOffscreenPosition);
                _titlePanel = new AnimationHelper.PanelPosition(UIComponents.Title, AnimationData.titleOffscreenPosition, AnimationData.titleOffScreenOffset);
                _buttonsPanel = new AnimationHelper.PanelPosition(UIComponents.ButtonsPanel, AnimationData.buttonsOffscreenPosition, AnimationData.buttonsOffScreenOffset);
            }

            protected override void RestartValues()
            {
                UIComponents.Title.anchoredPosition = _titlePanel.StartPos;
                UIComponents.ButtonsPanel.anchoredPosition = _buttonsPanel.StartPos;
            }

            protected override Sequence CreateAnimation()
            {
                var movementDuration = AnimationData.movementDuration;
                
                return DOTween.Sequence()
                        .AppendInterval(AnimationData.movementDelay)
                        .Append(UIComponents.Score.DOAnchorPos(_scorePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.HighScorePanel.DOAnchorPos(_highScorePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.Title.DOAnchorPos(_titlePanel.OffScreenPos, movementDuration))
                        .Join(UIComponents.ButtonsPanel.DOAnchorPos(_buttonsPanel.OffScreenPos, movementDuration))
                        //.AppendInterval(AnimationData.fadeDelay)
                        //.Append(UIComponents.BackgroundImage.DOFade(0, AnimationData.backgroundFadeDuration))
                        .Join(UIComponents.BackgroundImage.DOFade(0, AnimationData.backgroundFadeDuration))
                        .AppendCallback(() => UIComponents.MainPanel.gameObject.SetActive(false))
                    ;
            }
        }

        #endregion

        #region Score
        
        private class Animation_CurrentScore_Increment : SequenceUIAnimator<DefeatUiAnimationComponents.IScore,CurrentScoreIncrement>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private uint _targetScore;
            private readonly AnimationHelper.PanelPosition _panelPosition;

            public Animation_CurrentScore_Increment(DefeatUiAnimationComponents.IScore components, CurrentScoreIncrement animationData, Action<TMP_Text,long> handleText)
                : base(components, animationData)
            {
                _handleText = handleText;
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.Score,
                    AnimationData.offscreenPosition);
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
                        .Append(UIComponents.Score.DOAnchorPos(_panelPosition.StartPos, AnimationData.moveDuration))
                        .Append(TweenUtils.AnimateScore(_handleText, text, _targetScore, TweenUtils.GetDurationLog(_targetScore)))
                        .AppendInterval(AnimationData.finishDelay)
                    ;
            }
        }
        private class Animation_HighScore_Increment : SequenceUIAnimator<DefeatUiAnimationComponents.IHighScore,HighScoreIncrement>
        {
            private readonly Action<TMP_Text,long> _handleText;
            private uint _targetScore;
            private bool _hasNewHighScore;
            private readonly AnimationHelper.PanelPosition _panelPosition;
            
            public Animation_HighScore_Increment(DefeatUiAnimationComponents.IHighScore components, HighScoreIncrement animationData, 
                Action<TMP_Text,long> handleText) 
                : base(components, animationData)
            {
                _handleText = handleText;
                _panelPosition = new AnimationHelper.PanelPosition(UIComponents.HighScorePanel,
                    AnimationData.offscreenPosition);
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
                    .AppendCallback(() => UIComponents.HighScoreText.gameObject.SetActive(true))
                    .AppendCallback(() => UIComponents.HighScorePanel.gameObject.SetActive(true))
                    .Append(UIComponents.HighScorePanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.moveDuration))
                    .Append(TweenUtils.AnimateScore(_handleText, text, (long)_targetScore,
                        TweenUtils.GetDurationLog(_targetScore)));

                if (_hasNewHighScore)
                {
                    sequence
                        .Append(UIComponents.HighScoreText.DOScale(1, AnimationData.scaleDuration))
                        .AppendInterval(AnimationData.bounceDelay)
                        .Append(UIComponents.HighScoreText.DOScale(AnimationData.bounceScale, AnimationData.bounceDuration))
                        .Append(UIComponents.HighScoreText.DOScale(1, AnimationData.bounceReturnTime))
                        .Append(UIComponents.SubHighScoreText.DOScale(0,0))
                        .AppendInterval(AnimationData.newScoreTextDelay)
                        .AppendCallback(()=> UIComponents.SubHighScoreText.gameObject.SetActive(true))
                        .Append(UIComponents.SubHighScoreText.DOScale(AnimationData.newScoreBounceScale, AnimationData.newScoreBounceDuration))
                        .Append(UIComponents.SubHighScoreText.DOScale(1, AnimationData.newScoreBounceReturnTime))
                        ;
                }
                
                sequence.AppendInterval(AnimationData.finishDelay);
                
                return sequence;
            }
        }
        
        #endregion

        #region Buttons
        
        private class Animation_Buttons_Open : SequenceUIAnimator<DefeatUiAnimationComponents.IButtons,ButtonsOpen>
        {
            private readonly AnimationHelper.PanelPosition _panelPosition;
            
            public Animation_Buttons_Open(DefeatUiAnimationComponents.IButtons components, ButtonsOpen animationData)
                : base(components, animationData)
            {
                _panelPosition =
                    new AnimationHelper.PanelPosition(UIComponents.ButtonsPanel, AnimationData.offscreenPosition, AnimationData.offscreenOffset);
            }
            
            
            protected override void Initialize()
            {
                UIComponents.ButtonsPanel.anchoredPosition = _panelPosition.OffScreenPos;
            }

            protected override Sequence CreateAnimation()
            {
                return DOTween.Sequence()
                    .AppendCallback(()=> UIComponents.ButtonsPanel.gameObject.SetActive(true))
                    .Append(UIComponents.ButtonsPanel.DOAnchorPos(_panelPosition.StartPos, AnimationData.moveDuration))
                    .AppendInterval(AnimationData.finishDelay)
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
            
            _animationInitializer = new AnimationInitializer(UIComponents);
            //
            _animationMainOpen = new Animation_Main_Open(UIComponents,panelOpenData);
            _animationMainClose = new Animation_Main_Close(UIComponents, panelCloseData);
            //
            _animationButtonsOpen = new Animation_Buttons_Open(UIComponents, buttonsOpenData);
            _animationScoreCurrent = new Animation_CurrentScore_Increment(UIComponents,currentScoreData,HandleCurrentScoreText);
            //
            _animationScoreHigh = new Animation_HighScore_Increment(UIComponents,highScoreData,HandleHighScoreText);
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
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}