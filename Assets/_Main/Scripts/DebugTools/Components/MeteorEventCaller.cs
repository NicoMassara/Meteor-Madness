using _Main.Scripts.Managers;

namespace _Main.Scripts.DebugTools
{
    public class MeteorEventCaller
    {
        public static void SpawnSingle()
        {
            GameManager.Instance.EventManager.Publish(new ProjectileEvents.RequestSpawn
            {
                ProjectileType = ProjectileType.Meteor, RequestType = EventRequestType.Granted
            });
        }
        
        public static void SpawnRandom()
        {
            GameManager.Instance.EventManager.Publish(new MeteorEvents.SpawnRing());
        }
    }
}