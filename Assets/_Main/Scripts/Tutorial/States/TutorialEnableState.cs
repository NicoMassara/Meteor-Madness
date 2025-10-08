namespace _Main.Scripts.Tutorial.States
{
    public class TutorialEnableState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetEnable();
        }
    }
}