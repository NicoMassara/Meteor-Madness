using _Main.Scripts.Managers;

namespace _Main.Scripts.DebugTools
{
    public class ShieldEventCaller
    {
        public static void SetEnableShield(bool isEnabled)
        {
            GameManager.Instance.EventManager.Publish(new ShieldEvents.SetEnable{IsEnabled = isEnabled});
        }
    }
}