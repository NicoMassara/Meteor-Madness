using _Main.Scripts.Managers;

namespace _Main.Scripts.DebugTools
{
    public class AbilityEventCaller
    {
        public static void AddAbility(AbilityType abilityType)
        {
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.Add{AbilityType = abilityType});
        }
        
        public static void SpawnRandom()
        {
            GameManager.Instance.EventManager.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.AbilitySphere, RequestType = EventRequestType.Granted
            });
        }
    }
}