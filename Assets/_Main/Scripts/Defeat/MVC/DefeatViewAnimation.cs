using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Localization;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using DG.Tweening;
using Plugins.Demigiant.DOTween;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/MVC")]
    public class DefeatViewAnimation : BaseViewAnimation<DefeatUiAnimationSelector,DefeatUiAnimationComponents>
    {
        #region Animators

        private class MainAnimator : SequenceUIAnimator<DefeatUiAnimationComponents.IMainPanel>
        {
            public MainAnimator(DefeatUiAnimationComponents.IMainPanel components) 
                : base(components) { }

            protected override void Initialize()
            {
                
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence()
                        .AppendCallback(()=> Components.MainPanel.gameObject.SetActive(true));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence()
                    .AppendCallback(()=> Components.MainPanel.gameObject.SetActive(false));
            }
        }
        private class CurrentScoreAnimator : SequenceUIAnimator<DefeatUiAnimationComponents.IScore>
        {
            private string _localizedText;
            private readonly int _targetScore;

            public CurrentScoreAnimator(DefeatUiAnimationComponents.IScore components,
                string localizedText, int targetScore) : base(components)
            {
                _localizedText = localizedText;
                _targetScore = targetScore;
            }

            protected override void Initialize()
            {
                
            }

            protected override Sequence CreateFadeIn()
            {
                var scoreText = Components.Score.GetComponent<TMP_Text>();

                var currentScore = 0;
                
                var incrementer = DOTween.To(
                    () => currentScore,
                    x =>
                    {
                        currentScore = x;
                        scoreText.text = currentScore.ToString();
                    },
                    _targetScore,
                    1f
                );

                return DOTween.Sequence()
                    .Append(scoreText.DOCounter(_localizedText,0,_targetScore, 1f,"{0:D6}"));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence();
            }
        }
        private class HighScoreAnimator : SequenceUIAnimator<DefeatUiAnimationComponents.IHighScore>
        {
            private readonly string _localizedText;
            private readonly int _targetScore;
            private readonly bool _hasNewHighScore;


            public HighScoreAnimator(DefeatUiAnimationComponents.IHighScore components,
                string localizedText, int targetScore, bool hasNewHighScore)
                : base(components)
            {
                _localizedText = localizedText;
                _targetScore = targetScore;
                _hasNewHighScore = hasNewHighScore;
            }

            protected override void Initialize()
            {
                
            }

            protected override Sequence CreateFadeIn()
            {
                var scoreText = Components.HighScore.GetComponent<TMP_Text>();
                
                return DOTween.Sequence()
                    .Append(scoreText.DOCounter(_localizedText,0,_targetScore, 1f,"{0:D6}"));
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence();
            }
        }

        private class ButtonsAnimator : SequenceUIAnimator<DefeatUiAnimationComponents.IButtons>
        {
            public ButtonsAnimator(DefeatUiAnimationComponents.IButtons components) 
                : base(components) { }

            protected override void Initialize()
            {
                
            }

            protected override Sequence CreateFadeIn()
            {
                return DOTween.Sequence();
            }

            protected override Sequence CreateFadeOut()
            {
                return DOTween.Sequence();
            }
        }

        #endregion
        
        private IAnimator _mainAnimator;

        public event Action OnScoreFinished;
        public event Action OnHighScoreFinished;

        private void Start()
        {
            _mainAnimator = new MainAnimator(UIComponents);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //====== Score =======
                case DefeatObserverMessage.SendScore:
                    HandleSendScore((GeneratedId)args[0]);
                    break;
                case DefeatObserverMessage.SendHighScore:
                    HandleSendHighScore((GeneratedId)args[0],(bool)args[1]);
                    break;
                //====================
                
                //===== Enable / Disable
                case DefeatObserverMessage.Enable:
                    HandleEnable();
                    break;
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
            }
        }

        #region Enable / Disable
        
        private void HandleEnable()
        {
            SetAnimator(_mainAnimator);
        }
        
        private void HandleStartDisable()
        {
            ClearAnimator();
        }
        
        #endregion

        #region Score
        
        private void HandleSendScore(GeneratedId generatedId)
        {
            if (SecureValueManager.GetDoesContainValue<float>(generatedId, out var currentScore) == false)
            {
                return;
            }
            
            int finalScore = (int)currentScore;
            
            var sendScore = new CurrentScoreAnimator(UIComponents, GetLocalizedString("Gameplay.Score"), finalScore);
            sendScore.FadeIn(OnScoreFinished);
            
        }

        private void HandleSendHighScore(GeneratedId generatedId, bool hasHighScore)
        {
            if (SecureValueManager.GetDoesContainValue<float>(generatedId, out var highScore) == false)
            {
                return;
            }
            
            int finalScore = (int)highScore;
            
            var sendScore = new HighScoreAnimator(UIComponents,GetLocalizedString("Gameplay.HighScore"), 
                finalScore,hasHighScore);
            sendScore.FadeIn(OnHighScoreFinished);
        }
        
        #endregion
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
        
    }
}