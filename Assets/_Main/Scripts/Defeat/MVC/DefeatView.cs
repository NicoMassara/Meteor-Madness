using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    public class DefeatView : MonoBehaviour, IObserver,
        DefeatView.IDefeatView
    {
        public interface IDefeatView
        {
            public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
            public event Action OnDataInitialized;
        }

        #region IDefeatView

        public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
        public event Action OnDataInitialized;

        #endregion
        
        
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
                    HandleInitializeData((GeneratedId)args[0],(GeneratedId)args[1],(bool)args[2]);
                    break;
                case DefeatObserverMessage.SaveHighScore:
                    HandleSaveHighScore();
                    break;
            }
        }

        private void HandleInitializeData(GeneratedId highScoreId, GeneratedId currentScoreId, bool hasNewHighScore)
        {
            if (hasNewHighScore)
            {
                GameManager.Instance.SaveRuntimeHighScore(highScoreId, currentScoreId);
            }
            
            OnDataInitialized?.Invoke();
        }

        private void HandleSaveHighScore()
        {
            GameManager.Instance.SaveHighScore(GameManager.Instance.GetHighScoreSecuredId());
            GameManager.Instance.SaveStats();
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
    }
}