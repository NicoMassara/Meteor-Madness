using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Abilities
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class AbilitySpawnDebugData
    {
        
        public int MinSpawnLevel { get; set; }
        public bool IsRunningSpawnTimer { get; set; }
        public AbilityType LastAbilitySpawned { get; set; }
        public bool HasSentAbility { get; set; }
        public float NextSpawnDelay { get; set; }
        
        public AbilitySpawnDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay, DebugGUISortingOrder.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.AbilitySpawner, DebugGUISortingOrder.SubGroup.Ability)
                ?.AddEntry(
                    () => $"Min Level: {MinSpawnLevel}",
                    () => $"Is Running Timer: {IsRunningSpawnTimer}",
                    () => $"Next Spawn Delay: {NextSpawnDelay:F3}",
                    () => $"Last Spawned: {LastAbilitySpawned}",
                    () => $"Has Sent: {HasSentAbility}"
                );
        }
    }
#endif
}