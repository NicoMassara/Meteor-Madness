using System;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    [RequireComponent(typeof(MultiPageView))]
    [RequireComponent(typeof(MultiPageViewUI))]
    public class MultiPageSetup : ManagedBehavior
    {
        [SerializeField] private MultiPageTextDataSo startData;
        private MultiPageView _view;
        private MultiPageViewUI _ui;

        private void Awake()
        {
            _view = GetComponent<MultiPageView>();
            _ui = GetComponent<MultiPageViewUI>();
        }

        private void Start()
        {
            //View
            _view.OnFinished += View_OnFinishedHandler;
            _view.OnNextButtonTextChanged += (newText) => {_ui.SetNextButtonText(newText);};
            _view.OnPreviousButtonSetEnable += (isEnable) => {_ui.SetEnablePreviousButton(isEnable);};
            _view.OnPageChanged += (panelText) => {_ui.SetPanelText(panelText);};
            _view.NextButtonDefaultText = _ui.GetNextButtonText();
            //UI
            _ui.OnNextButtonPressed += ()=> _view.TryIncreasePageIndex();
            _ui.OnPreviousButtonPressed += ()=> _view.TryDecreasePageIndex();

            if (startData != null)
            {
                SetTextData(startData);
            }
        }

        private void View_OnFinishedHandler()
        {
            _ui.SetActiveMainPanel(false);
            Debug.Log("Multi Page Finished");
        }

        private void SetTextData(MultiPageTextDataSo newText)
        {
            _view.SetTextData(newText);
            _ui.SetActiveMainPanel(true);
        }
    }
}