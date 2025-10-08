namespace _Main.Scripts.Tutorial.States
{
    public class TutorialMovementState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetMovement();
        }
    }
}