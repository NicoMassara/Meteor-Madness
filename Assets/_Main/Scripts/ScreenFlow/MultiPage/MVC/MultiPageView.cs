using UnityEngine;
using System;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Managers;
using NicolasMassara.CustomUpdateManager;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageView : ManagedBehavior
    {
        private const string NextButtonCode = "UIButton.Next";
        private IMultiPageData _data;
        private int _currentPageIndex;
        private ulong _createId;
        
        public event Action<ulong> OnFinished;
        public event Action<string> OnNextButtonTextChanged;
        public event Action<bool> OnPreviousButtonSetEnable;
        public event Action<string, int> OnPageChanged;
        public void SetTextData(IMultiPageData data)
        {
            _data = data;
            RestartValues();
        }

        public void TryIncreasePageIndex()
        {
            if (_currentPageIndex == _data.MaxTextIndex)
            {
                OnFinished?.Invoke(_createId);
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
            if (_data.TextCount > 1)
            {
                UpdateText();
                OnNextButtonTextChanged?.Invoke(NextButtonCode);
            }
            else
            {
                OnNextButtonTextChanged?.Invoke(_data.LastButtonCode);
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
                OnNextButtonTextChanged?.Invoke(_data.LastButtonCode);
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
                OnNextButtonTextChanged?.Invoke(NextButtonCode);
            }
            
            UpdateText();
        }

        private void UpdateText()
        {
            OnPageChanged?.Invoke(_data.TextsCode, _currentPageIndex);
        }

        public void SetCreateId(ulong createId)
        {
            _createId = createId;
        }

        public void TriggerFinish()
        {
            MultiPageUIEventCaller.Finished(_createId);
        }
    }
}