namespace _Main.Scripts.Gameplay.GameMode.States
{
    public class GameModeEnableState<T> : GameModeStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
}