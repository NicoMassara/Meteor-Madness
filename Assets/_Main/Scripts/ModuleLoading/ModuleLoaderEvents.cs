using System;

namespace _Main.Scripts
{
    public class ModuleLoaderEvents
    {
        public static event Action OnModulesLoaded;

        public static void TriggerOnModulesLoaded()
        {
            OnModulesLoaded?.Invoke();
        }
    }
}