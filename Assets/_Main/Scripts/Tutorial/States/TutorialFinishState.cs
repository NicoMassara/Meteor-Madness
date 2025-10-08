namespace _Main.Scripts.Tutorial.States
{
    public class TutorialFinishState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetFinish();
        }
    }
}