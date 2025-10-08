namespace _Main.Scripts.Tutorial.States
{
    public class TutorialDisableState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetDisable();
        }
    }
}