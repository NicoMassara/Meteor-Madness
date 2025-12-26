using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.GlobalValues.Tools.Observer;

namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatMotor : ObservableComponent
    {
        private GeneratedId _scoreId;
        private GeneratedId _highScoreId;
        private bool _hasNewHighScore;
        
        public void ExecuteDisable() => NotifyAll(DefeatObserverMessage.ExecuteDisable);
        public void StartDisable() => NotifyAll(DefeatObserverMessage.StartDisable);
        public void Enable() => NotifyAll(DefeatObserverMessage.Enable);
        public void LoadData() => NotifyAll(DefeatObserverMessage.LoadData);
        public void LoadScoreData(GeneratedId score, GeneratedId highScore, bool hasNewHigh)
        {
            _scoreId = score;
            _highScoreId = highScore;
            _hasNewHighScore = hasNewHigh;
            
            NotifyAll(DefeatObserverMessage.InitializeData, _highScoreId,_scoreId,_hasNewHighScore);
        }
        
        public void SendScore() => NotifyAll(DefeatObserverMessage.SendScore, _scoreId);
        public void SendHighScore() => NotifyAll(DefeatObserverMessage.SendHighScore, _highScoreId,_hasNewHighScore);
        public void SendButtons() => NotifyAll(DefeatObserverMessage.SendButtons);
        public void EnableButtons() => NotifyAll(DefeatObserverMessage.EnableButtons);

        public void SaveHighScore()
        {
            if (_hasNewHighScore) 
                NotifyAll(DefeatObserverMessage.SaveHighScore);
        }

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