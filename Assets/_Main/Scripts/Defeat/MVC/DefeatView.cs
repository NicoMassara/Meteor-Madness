using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/MVC")]
    public class DefeatView : MonoBehaviour, IObserver,
        DefeatView.IDefeatView
    {
        public interface IDefeatView
        {
            public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
            public event Action OnDataInitialized;
        }
        
        public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
        public event Action OnDataInitialized;
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Disable ===/
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case DefeatObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                
                //=== Load Data ===/
                case DefeatObserverMessage.LoadData:
                    HandleDataLoaded();
                    break;
                case DefeatObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
            }
        }
        

        private void HandleExecuteDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Granted);
        }

        private void HandleStartDisable()
        {
            GameManager.Instance.ClearScoreData();
        }
        
        private void HandleDataLoaded()
        {
            OnDataLoaded?.Invoke(
                GameManager.Instance.CurrentScoreSecuredId, 
                GameManager.Instance.GetHighScoreSecuredId(),
                GameManager.Instance.GetHasNewHighScore());

        }
        
        private void HandleInitializeData()
        {
            OnDataInitialized?.Invoke();
        }
    }
}