using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Observer;

namespace _Main.Scripts.Tutorial.MVC
{
    public class TutorialView : ManagedBehavior, IObserver
    {
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                
            }
        }
    }
}