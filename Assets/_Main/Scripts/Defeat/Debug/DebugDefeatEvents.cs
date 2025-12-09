using System;

namespace _Main.Scripts.Defeat
{
    public class DebugDefeatEvents
    {
        public static event Action OnDefeatScreenAnimationFinished;
        public static event Action OnDefeatScreenClosed;
        
        public static void TriggerDefeatScreenAnimationFinished()
        {
            OnDefeatScreenAnimationFinished?.Invoke();
        }
        
        public static void TriggerDefeatScreenClosed()
        {
            OnDefeatScreenClosed?.Invoke();
        }
    }
}