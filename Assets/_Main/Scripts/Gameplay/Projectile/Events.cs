using System;
using _Main.Scripts.Projectile;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileEvents
    {
        
    }

    public class ProjectileDebugEvents
    {
        public static event Action<BatchDebugData> OnBatchCreated;

        public static void TriggerBatchCreated(BatchDebugData batch) => OnBatchCreated?.Invoke(batch);
        
        public static event Action<HistoryDebugData> OnHistoryChanged;

        public static void TriggerHistoryChanged(HistoryDebugData history) => OnHistoryChanged?.Invoke(history);
    }
}