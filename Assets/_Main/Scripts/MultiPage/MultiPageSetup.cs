using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
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

            //View
            _view.OnFinished += View_OnFinishedHandler;
            _view.OnNextButtonTextChanged += (newText) => {_ui.SetNextButtonText(newText);};
            _view.OnPreviousButtonSetEnable += (isEnable) => {_ui.SetEnablePreviousButton(isEnable);};
            _view.OnPageChanged += (panelText, index) => {_ui.SetPanelText(panelText, index);};
            //UI
            _ui.OnNextButtonPressed += ()=> _view.TryIncreasePageIndex();
            _ui.OnPreviousButtonPressed += ()=> _view.TryDecreasePageIndex();
            
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
            _ui.SetActiveMainPanel(false);
            GameEventCaller.Publish(new MultiPageUIEvents.Finished{CreateId = createId});
        }

        private void SetTextData(IMultiPageData newText, ulong createId = 0)
        {
            _view.SetTextData(newText);
            _view.SetCreateId(createId);
            _ui.SetActiveMainPanel(true);
        }

        #region EventBus

        private void SetupEventBus()
        {
            GameEventCaller.Subscribe<MultiPageUIEvents.Create>(EventBus_MultiPage_Create);
        }

        private void EventBus_MultiPage_Create(MultiPageUIEvents.Create input)
        {
            SetTextData(input.Data, input.CreateId);
        }

        #endregion
        

    }
}