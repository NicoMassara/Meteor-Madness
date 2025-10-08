namespace _Main.Scripts.Tutorial.States
{
    public class TutorialStartState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetStart();
        }
    }
}