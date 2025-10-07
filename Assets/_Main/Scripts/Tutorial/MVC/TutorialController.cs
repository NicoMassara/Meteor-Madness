namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialController
    {
        private readonly TutorialMotor _motor;
        
        public TutorialController(TutorialMotor motor)
        {
            _motor = motor;
        }
    }
}