using _Main.Scripts.Managers;

namespace _Main.Scripts.DebugTools
{
    public class GameValuesEventCaller
    {
        public static void SetDamageType(DamageTypes damageType)
        {
            GameManager.Instance.SetMeteorDamage(damageType);
        }

        public static void UpdateLevel(int level)
        {
            GameManager.Instance.EventManager.Publish(new GameModeEvents.UpdateLevel{CurrentLevel = level});
        }
    }
}