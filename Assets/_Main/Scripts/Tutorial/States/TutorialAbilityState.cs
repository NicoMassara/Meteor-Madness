namespace _Main.Scripts.Tutorial.States
{
    public class TutorialAbilityState<T> : TutorialStateBase<T>
    {
        public override void Awake()
        {
            Controller.SetAbility();
        }
    }
}