using System;
using _Main.Scripts.Localization;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using UnityEngine;

namespace _Main.Scripts.GameStats
{
    public class StatsUIView : BaseViewUI<StatsUiSelector,StatsUiComponents>, IObserver,
        StatsUIView.IStatsUIView
    {
        public interface IStatsUIView
        {
            public event Action OnBackButtonPressed;
            public event Action OnTextsLoaded;
        }
        
        public event Action OnBackButtonPressed;
        public event Action OnTextsLoaded;

        private void EnableButtons()
        {
            UIComponents.BackButton.onClick.AddListener(() => OnBackButtonPressed?.Invoke());
        }

        private void DisableButtons()
        {
            UIComponents.BackButton.onClick.RemoveListener(() => OnBackButtonPressed?.Invoke());
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case StatsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case StatsObserverMessage.StartDisable:
                    HandleStartDisable();
                   break;
                
                case StatsObserverMessage.LoadTextData:
                    HandleLoadTextData((StatsData)args[0]);
                    break;
            }
        }

        private void HandleLoadTextData(StatsData statsData)
        {
            UIComponents.DeflectAmountText.text = $"{GetLocalizedString("Stats.Deflect")}: {statsData.DeflectAmount}";
            UIComponents.CollisionAmountText.text = $"{GetLocalizedString("Stats.Collision")}: {statsData.CollisionAmount}";
            UIComponents.AbilityUseAmountText.text = $"{GetLocalizedString("Stats.AbilityCount")}: {statsData.AbilityUseAmount}";
            UIComponents.GamesPlayedText.text = $"{GetLocalizedString("Stats.GamesPlayed")}: {statsData.GamesPlayed}";
            
            OnTextsLoaded?.Invoke();
        }

        private void HandleEnable()
        {
            EnableButtons();
        }
        
        private void HandleStartDisable()
        {
            DisableButtons();
        }
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
    }
}