using System;
using _Main.Scripts.Projectile;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileEvents
    {
        
    }

    public class ProjectileDebugEvents
    {
        public static event Action<DefaultBatchDebugData> OnBatchCreated;

        public static void TriggerBatchCreated(DefaultBatchDebugData defaultBatch) => OnBatchCreated?.Invoke(defaultBatch);
        
        public static event Action<HistoryDebugData> OnHistoryChanged;

        public static void TriggerHistoryChanged(HistoryDebugData history) => OnHistoryChanged?.Invoke(history);
    }
}