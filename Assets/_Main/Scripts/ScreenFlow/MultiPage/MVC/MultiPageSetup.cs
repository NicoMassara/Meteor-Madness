using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Managers;
using MeteorMadness.ScreenFlow.MultiPage;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    [RequireComponent(typeof(MultiPageView))]
    [RequireComponent(typeof(MultiPageViewUI))]
    [RequireComponent(typeof(MultiPageViewAnimation))]
    public class MultiPageSetup : ManagedBehavior
    {
        [SerializeField] private MultiPageTextDataSo startData;
        private MultiPageView _view;
        private MultiPageViewUI _ui;
        private MultiPageViewAnimation _animation;
        

        private void Awake()
        {
            _view = GetComponent<MultiPageView>();
            _ui = GetComponent<MultiPageViewUI>();
            _animation = GetComponent<MultiPageViewAnimation>();

            //View
            _view.OnFinished += View_OnFinishedHandler;
            _view.OnNextButtonTextChanged += (newText) => {_ui.SetNextButtonText(newText);};
            _view.OnPreviousButtonSetEnable += (isEnable) => {_ui.SetEnablePreviousButton(isEnable);};
            _view.OnPageChanged += (panelText, index) => {_ui.SetPanelText(panelText, index);};
            //UI
            _ui.OnNextButtonPressed += () =>
            {
                _ui.DisableNextButton();
                _view.TryIncreasePageIndex();
            };
            _ui.OnPreviousButtonPressed += ()=> _view.TryDecreasePageIndex();
            //
            _animation.OnPanelOpened += () => _ui.DisableNextButton();
            _animation.OnPanelClosed += ()=> _view.TriggerFinish();
            
            SetupEventBus();
        }

        private void Start()
        {
            if (startData != null)
            {
                SetTextData(startData);
            }
        }
        
        private void View_OnFinishedHandler(ulong createId)
        {
            _animation.DisablePanel();
        }

        private void SetTextData(IMultiPageData newText, ulong createId = 0)
        {
            _view.SetTextData(newText);
            _view.SetCreateId(createId);
            _animation.EnablePanel();
        }

        #region EventBus

        private void SetupEventBus()
        {
            MultiPageUIEventSubscriber.Create(EventBus_MultiPage_Create);
        }

        private void EventBus_MultiPage_Create(MultiPageUIEvents.Create input)
        {
            SetTextData(input.Data, input.CreateId);
        }

        #endregion
    }
}