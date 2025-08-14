using UnityEngine;

namespace _Main.Scripts.UI.FSM.Level
{
    public class LevelUIStartState<T> : LevelUIBaseState<T>
    {
        private GameObject canvasPanel;
        private float _timer;

        public LevelUIStartState(GameObject canvasPanel)
        {
            this.canvasPanel = canvasPanel;
        }

        public override void Awake()
        {
            _timer = GameValues.StartGameCount;
            Controller.ChangeCurrentPanel(canvasPanel);
        }

        public override void Execute()
        {
            _timer -= Time.deltaTime;
            
            Controller.UpdateCountdownText(Mathf.RoundToInt(_timer));

            if (_timer <= 0)
            {
                Controller.PlayTransition();
            }
        }

        public override void Sleep()
        {

        }
    }
}