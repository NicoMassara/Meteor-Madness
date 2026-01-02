using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.GlobalValues.Tools.Observer;

namespace MeteorMadness.ScreenFlow.Defeat
{
    
    public class DefeatMotor : ObservableComponent
    {
        private DefeatScreenData _defeatScreenData;
        
        public void ExecuteDisable() => NotifyAll(DefeatObserverMessage.ExecuteDisable);
        public void StartDisable() => NotifyAll(DefeatObserverMessage.StartDisable);
        public void Enable() => NotifyAll(DefeatObserverMessage.Enable);
        public void LoadData() => NotifyAll(DefeatObserverMessage.LoadData);
        internal void LoadScoreData(DefeatScreenData defeatScreenData)
        {
            _defeatScreenData = defeatScreenData;
            NotifyAll(DefeatObserverMessage.InitializeData, _defeatScreenData.HasNewHighScore);
        }
        
        public void SendScore() => NotifyAll(DefeatObserverMessage.SendScore, _defeatScreenData.Score);
        public void SendHighScore() 
            => NotifyAll(DefeatObserverMessage.SendHighScore, _defeatScreenData.HighScore,_defeatScreenData.HasNewHighScore);
        public void SendButtons() => NotifyAll(DefeatObserverMessage.SendButtons);
        public void EnableButtons() => NotifyAll(DefeatObserverMessage.EnableButtons);
        public void SendAd() => NotifyAll(DefeatObserverMessage.SendAds);
        public void RestartGame() => NotifyAll(DefeatObserverMessage.RestartGame);
        public void LoadMainMenu() => NotifyAll(DefeatObserverMessage.LoadMainMenu);
        public void SendCoins() => NotifyAll(DefeatObserverMessage.SendCoins);
        public void CheckForNewCoins()
            => NotifyAll(DefeatObserverMessage.CheckNewCoins);
        public void UpdateCoins(uint stored, uint gained) 
            => NotifyAll(DefeatObserverMessage.UpdateCoins, stored, gained);
    }
}