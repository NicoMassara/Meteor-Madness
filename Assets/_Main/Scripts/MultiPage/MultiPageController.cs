using System;
using _Main.Scripts.Managers.UpdateManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageController : ManagedBehavior
    {
        [TextArea]
        [SerializeField] private string[] texts;
        [SerializeField] private TMP_Text screenText;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text nextButtonText;
        [SerializeField] private string lastPageText = "Finish";
        
        private int TextCount => texts.Length;
        private int MaxTextIndex => TextCount - 1;
        private int _currentPageIndex;
        private string _initialNextButtonText;

        public event Action OnFinished;

        private void Awake()
        {
            nextButton.onClick.AddListener(NextButtonOnClickHandler);
            previousButton.onClick.AddListener(PreviousButtonOnClickHandler);
        }

        private void Start()
        {
            previousButton.gameObject.SetActive(false);
            
            if (TextCount > 0)
            {
                UpdateText();
            }
            
            _initialNextButtonText = nextButtonText.text;
        }

        public void RestartValues()
        {
            previousButton.gameObject.SetActive(false);
            if (TextCount > 0)
            {
                UpdateText();
                nextButtonText.text = "Next";
            }
            else
            {
                nextButtonText.text = lastPageText;
            }
        }

        private void UpdateText()
        {
            screenText.text = texts[_currentPageIndex];
        }

        private void IncreaseIndex()
        {
            _currentPageIndex++;
            _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, MaxTextIndex);

            if (_currentPageIndex == 1)
            {
                previousButton.gameObject.SetActive(true);
            }

            if (_currentPageIndex == MaxTextIndex)
            {
                nextButtonText.text = lastPageText;
            }

            UpdateText();
        }

        private void DecreaseIndex()
        {
            _currentPageIndex--;
            _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, MaxTextIndex);
            
            if (_currentPageIndex == 0)
            {
                previousButton.gameObject.SetActive(false);
            }
            
            if (_currentPageIndex < MaxTextIndex)
            {
                nextButtonText.text = _initialNextButtonText;
            }
            
            UpdateText();
        }

        private void NextButtonOnClickHandler()
        {
            if (_currentPageIndex == MaxTextIndex)
            {
                OnFinished?.Invoke();
            }
            else
            {
                IncreaseIndex();
            }
        }

        private void PreviousButtonOnClickHandler()
        {
            if (_currentPageIndex > 0)
            {
                DecreaseIndex();
            }
        }
    }
}