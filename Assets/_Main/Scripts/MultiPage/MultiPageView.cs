using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using System;
using _Main.Scripts.Interfaces;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageView : ManagedBehavior
    {
        private IMultiPageData _data;
        private int _currentPageIndex;
        public string NextButtonDefaultText { get; set; }

        public event Action OnFinished;
        public event Action<string> OnNextButtonTextChanged;
        public event Action<bool> OnPreviousButtonSetEnable;
        public event Action<string> OnPageChanged;
        public void SetTextData(IMultiPageData data)
        {
            _data = data;
            RestartValues();
        }

        public void TryIncreasePageIndex()
        {
            if (_currentPageIndex == _data.MaxTextIndex)
            {
                OnFinished?.Invoke();
            }
            else
            {
                IncreaseIndex();
            }
        }

        public void TryDecreasePageIndex()
        {
            if (_currentPageIndex > 0)
            {
                DecreaseIndex();
            }
        }
        
        private void RestartValues()
        {
            _currentPageIndex = 0;
            OnPreviousButtonSetEnable?.Invoke(false);
            if (_data.TextsArray.Length > 1)
            {
                UpdateText();
                OnNextButtonTextChanged?.Invoke(NextButtonDefaultText);
            }
            else
            {
                OnNextButtonTextChanged?.Invoke(_data.LastPageNextButtonText);
            }
        }

        private void IncreaseIndex()
        {
            _currentPageIndex++;
            _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, _data.MaxTextIndex);
            //

            if (_currentPageIndex == 1)
            {
                OnPreviousButtonSetEnable?.Invoke(true);
            }

            if (_currentPageIndex == _data.MaxTextIndex)
            {
                OnNextButtonTextChanged?.Invoke(_data.LastPageNextButtonText);
            }

            UpdateText();
        }

        private void DecreaseIndex()
        {
            _currentPageIndex--;
            _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, _data.MaxTextIndex);
            //
            
            if (_currentPageIndex == 0)
            {
                OnPreviousButtonSetEnable?.Invoke(false);
            }
            
            if (_currentPageIndex == _data.MaxTextIndex-1)
            {
                OnNextButtonTextChanged?.Invoke(NextButtonDefaultText);
            }
            
            UpdateText();
        }

        private void UpdateText()
        {
            OnPageChanged?.Invoke(_data.TextsArray[_currentPageIndex]);
        }
    }
}