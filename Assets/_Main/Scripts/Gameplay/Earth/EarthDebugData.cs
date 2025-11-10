using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Gameplay.Earth
{
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
    public class EarthDebugData
    {
        public float EarthHealth;
        public float RotationSpeed;
        public float ShakeIntensity;
        
        public EarthDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.Earth)
                ?.AddEntry(
                    ()=> $"Health: {EarthHealth}",
                    ()=> $"Rotation: {RotationSpeed}",
                    ()=> $"Shake: {ShakeIntensity}"
                );
        }
    }
    
#endif

}